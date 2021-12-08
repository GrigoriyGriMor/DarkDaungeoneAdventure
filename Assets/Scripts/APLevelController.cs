using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class APLevelController : MonoBehaviour
{
    [SerializeField] private LevelBlock[] blocks = new LevelBlock[3];
    [SerializeField] private Transform player;

    [SerializeField] private float minDistance;

    private void FixedUpdate()
    {
        for (int i = 0; i < blocks.Length; i++)
            if (Vector3.Distance(player.position, blocks[i].ActivatorPoint.position) < minDistance)
            {
                if (!blocks[i].blockObj.activeInHierarchy)
                    blocks[i].blockObj.SetActive(true);
            }
            else
            {
                if (blocks[i].blockObj.activeInHierarchy)
                    blocks[i].blockObj.SetActive(false);
            }
    }
}

[Serializable]
public class LevelBlock
{
    public GameObject blockObj;
    public Transform ActivatorPoint;
}
