using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Game.Core
{
    //This is used to declare our damagehandler script and the CustomEditor
    [CustomEditor(typeof(ConfigManager))]
    public class ConfigurationManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //Run through the base code of the OnInspectorGUI then use ours...
            base.OnInspectorGUI();
            ConfigManager manager = target as ConfigManager;
            if (GUILayout.Button("Refresh config list"))
            {
                HashSet<DefaultConfiguration> filtered = new HashSet<DefaultConfiguration>();
                foreach (string asset_guid in AssetDatabase.FindAssets($"t:{nameof(DefaultConfiguration)}"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(asset_guid);
                    DefaultConfiguration cfg = AssetDatabase.LoadAssetAtPath<DefaultConfiguration>(path);
                    //Debug.Log($"uid:{asset_guid}; path:{path}; disabled:{cfg.disabled}");
                    if (!cfg.disabled)
                        filtered.Add(cfg);
                }
                // Чтобы по возможности сохранить порядок элементов так сложно, вместо простейшего решения
                // manager.allConfigList = new List<Configuration>(filtered);
                List<DefaultConfiguration> newConfigsList = new List<DefaultConfiguration>(manager.allConfigList);
                HashSet<DefaultConfiguration> exclude = new HashSet<DefaultConfiguration>(newConfigsList);
                exclude.ExceptWith(filtered);
                foreach (var item in exclude)
                    newConfigsList.Remove(item); // Это жесть со сложностью o(n^2) но это пофигу, потому что миссиг референсы и масштабные отключения маловероятны
                Debug.Log($"ConfigManager removed configs:{exclude.Count}");
                filtered.ExceptWith(new HashSet<DefaultConfiguration>(newConfigsList));
                foreach (var item in filtered)
                    newConfigsList.Add(item);
                Debug.Log($"ConfigManager added configs:{filtered.Count}");

                if ((exclude.Count != 0) | (filtered.Count != 0)) {
                    manager.allConfigList = newConfigsList.ToArray();
                    EditorUtility.SetDirty(manager);
                }

                // Проверить на дубликаты ключей
                var check = new Dictionary<ConfigManager.HashKey, DefaultConfiguration>(newConfigsList.Count);
                {
                    DefaultConfiguration item;
                    for (int i = 0; i < manager.allConfigList.Length; ++i)
                    {
                        item = manager.allConfigList[i];
                        var key = new ConfigManager.HashKey() { id = item is IdentifiableConfiguration identifiable ? identifiable.id : null, type = item.GetType() };
                        if (!check.TryAdd(key, item))
                            Debug.LogError($"ConfigManager is contains duplicate item with allConfigList[{i}] with type={key.type} id={(key.id != null ? key.id : "<null>")}; first='{AssetDatabase.GetAssetPath(check[key])}'; second='{AssetDatabase.GetAssetPath(item)}'");
                    }
                }
            }
        }
    }
}
