using UnityEngine;

namespace Game.Core
{
    // Общий предок всех конфигурационных файлов. Он нужен чтобы менеджер конфигов мог его найти по типу в AssetManager
    public abstract class DefaultConfiguration : ScriptableObject
    {
        /// <summary> В будущем при сборке конфиги будут сами подтягиваться в менеджер и/или манифест
        /// и для того, чтобы кого-то из них отключить заводим поле здесь.</summary>
        [SerializeField] public bool disabled;
    }
    /// TODO Переименоваать
    public abstract class IdentifiableConfiguration : DefaultConfiguration//с ID на случай одинаковых типов с разным наполнением
    {
        /// <summary> Есть много конфигов, которые отличаются только Id-шником,
        /// и которые будут связываться друг с другом по id-шникам.</summary>
        /// <returns></returns>
        [SerializeField] public string id;
    }    
}
