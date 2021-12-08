using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointNEW : MonoBehaviour
{
    private bool canUse = true;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private ParticleSystem particle;

    [SerializeField] private AudioClip clip; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<APPlayerController>() && (other.GetComponent<APPlayerController>().CurrentSpawnPos() != spawnPoint.position))
        {
            if (particle != null) particle.Play();

            if (clip != null && SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(clip);
             canUse = false;
            other.GetComponent<APPlayerController>().SetNewSpawnPos(spawnPoint.position);
        }
    }
}
