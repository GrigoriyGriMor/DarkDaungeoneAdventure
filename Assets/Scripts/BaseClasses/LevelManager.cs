using UnityEngine;

namespace BaseClasses
{
    public class LevelManager : SingeltonManager<LevelManager>
    {
        [SerializeField]
        private PostProcessingManager _postProcessManager = null;
        
        static public PostProcessingManager PostProcessManager { get => Instance.GetManager(Instance._postProcessManager); }
    }
}