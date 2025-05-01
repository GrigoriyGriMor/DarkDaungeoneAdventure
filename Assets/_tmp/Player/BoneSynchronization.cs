using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RigSynchronization : MonoBehaviour
{
    [Header("Основной Rig персонажа")]
    public Transform RigRoot;

    private List<SkinnedMeshRenderer> RigMeshes = new List<SkinnedMeshRenderer>();
    private Dictionary<string, Transform> BoneMap = new Dictionary<string, Transform>();

    private void Start()
    {
        if (RigRoot == null)
        {
            Debug.LogError("RigRoot is not assigned.");
            return;
        }

        CacheBones();
        UpdateRigMeshes();
        SynchronizeBones();
    }

    /// <summary>
    /// Кэшируем все кости в словарь для быстрого поиска
    /// </summary>
    private void CacheBones()
    {
        BoneMap.Clear();
        foreach (var bone in RigRoot.GetComponentsInChildren<Transform>())
        {
            if (!BoneMap.ContainsKey(bone.name))
            {
                BoneMap.Add(bone.name, bone);
            }
            else
            {
                Debug.LogWarning($"Duplicate bone name detected: {bone.name}");
            }
        }
    }

    /// <summary>
    /// Обновляем список всех мешей с костями
    /// </summary>
    public void UpdateRigMeshes()
    {
        RigMeshes.Clear();
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            RigMeshes.Add(skinnedMeshRenderer);
        }
    }

    /// <summary>
    /// Переназначаем кости для всех мешей
    /// </summary>
    public void SynchronizeBones()
    {
        if (RigRoot == null)
        {
            Debug.LogError("RigRoot is not assigned.");
            return;
        }

        foreach (var skinnedMeshRenderer in RigMeshes)
        {
            List<string> missingBones = new List<string>();

            Transform[] newBones = new Transform[skinnedMeshRenderer.bones.Length];

            for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            {
                Transform bone = skinnedMeshRenderer.bones[i];
                if (bone != null && BoneMap.TryGetValue(bone.name, out Transform newBone))
                {
                    newBones[i] = newBone;
                }
                else
                {
                    newBones[i] = null;
                    missingBones.Add(bone != null ? bone.name : $"(null bone at index {i})");
                }
            }

            skinnedMeshRenderer.bones = newBones;

            if (missingBones.Count > 0)
            {
                Debug.LogWarning($"[{skinnedMeshRenderer.name}] Missing bones:\n- {string.Join("\n- ", missingBones)}");
            }
        }
    }

#if UNITY_EDITOR
    // Кнопка для редактора — позволяет вызывать синхронизацию прямо в инспекторе
    [ContextMenu("Update and Synchronize Rig")]
    private void UpdateAndSynchronize()
    {
        if (RigRoot == null)
        {
            Debug.LogError("RigRoot is not assigned.");
            return;
        }

        CacheBones();
        UpdateRigMeshes();
        SynchronizeBones();

        Debug.Log("Rig updated and bones synchronized!");
    }
#endif
}
