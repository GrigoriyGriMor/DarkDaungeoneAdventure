using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public partial class ConfigManager : AbstractManager
    {
        public struct HashKey
        {
            public string id;
            public Type type;
        }

        [SerializeField] public DefaultConfiguration[] allConfigList;
        // Один длинный дикшенари для хранения выгоднее чем более сложные конструкции. потому что вне зависимости от размера имеет константное время доступа на чтение, а хэш тоже считается очень быстро.
        private Dictionary<HashKey, DefaultConfiguration> hashed = null;
        // Здесь ленивай инициализация, потому что некоторые менеджеры обращались за конфигами раньше, чем Init доходил до этого менеджера. И так надёжнее, чем плясать пляски вокруг порядка инициализации или dependency injection
        private Dictionary<HashKey, DefaultConfiguration> Hashed() {
            return hashed ??= InitHash();
        }

        private Dictionary<HashKey, DefaultConfiguration> InitHash()
        {
            hashed = new Dictionary<HashKey, DefaultConfiguration>(allConfigList.Length);

            DefaultConfiguration item;
            for (int i = 0; i < allConfigList.Length; i++)
            {
                item = allConfigList[i];

                HashKey key = new HashKey() { id = item is IdentifiableConfiguration identifiable ? identifiable.id : null, type = item.GetType() };
                
                if (!hashed.TryAdd(key, item))
                    throw new Exception($"ConfigManager is contains duplicate item with allConfigList[{i}] with type={key.type} id={(key.id != null ? key.id : "<null>")}");
            }
            
            return hashed;
        }

        public T GetConfiguration<T>() where T : DefaultConfiguration
        {
            if (Hashed().TryGetValue(new HashKey() { id = null, type = typeof(T) }, out var item))
                return (T)item;
            throw new Exception($"getting config of type {typeof(T)} is missing in configManager!");
        }
        public T GetConfiguration<T>(string id) where T : IdentifiableConfiguration {
            if (Hashed().TryGetValue(new HashKey() { id = id, type = typeof(T) }, out var item))
                return (T)item;
            throw new Exception($"getting config of type {typeof(T)} and id='{id}' is missing in configManager!");
        }
    }
}