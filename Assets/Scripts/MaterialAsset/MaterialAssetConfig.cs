using Game.Core;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "MaterialAsset/Configs/Material Asset Data")]
    public class MaterialAssetConfig : DefaultConfiguration
    {
        public Material _defaultMat;

        public MaterialAssetData[] materialAssets;

        public Material GetMaterial(string matName)
        {
            for (int i = 0; i < materialAssets.Length; i++)
                if (matName == materialAssets[i].matName)
                    return materialAssets[i].material;

            return _defaultMat;
        }
    }
}