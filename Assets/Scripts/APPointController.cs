using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APPointController : MonoBehaviour
{
    [SerializeField] private int point = 1;
    [SerializeField] private GameObject pointVisual;
    [SerializeField] private ParticleSystem pointUpParticle;
    [SerializeField] private AudioClip pointAudioClip;

    private bool itemBeUsed = false;

    private Vector3 startPos;
    [Range(-50, 0)][SerializeField] private float distanceLeftX = -1;
    [Range(0, 50)] [SerializeField] private float distanceRightX = 1;
    [Range(-50, 0)] [SerializeField] private float distanceLeftY = -1;
    [Range(0, 50)] [SerializeField] private float distanceRightY = 1;
    [SerializeField] private float respawnTime = 10;

    private void Start()
    {
        startPos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(UnityEngine.Random.Range(startPos.x + distanceLeftX, startPos.x + distanceRightX), transform.position.y,
            UnityEngine.Random.Range(startPos.z + distanceLeftY, startPos.z + distanceRightY));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!itemBeUsed && other.gameObject.GetComponent<APPlayerController>())
        {
            StartCoroutine(RespawnTime()); 
            APGameController.Instance.UpgradePoint(point);

            //if (SoundManagerAllControll.Instance && pointAudioClip != null) SoundManagerAllControll.Instance.ClipPlay(pointAudioClip);
            if (pointUpParticle != null) pointUpParticle.Play();

            pointVisual.SetActive(false);
        }
    }

    private IEnumerator RespawnTime()
    {
        itemBeUsed = true;
        yield return new WaitForSeconds(respawnTime);

        gameObject.transform.position = new Vector3(UnityEngine.Random.Range(startPos.x + distanceLeftX, startPos.x + distanceRightX), transform.position.y,
            UnityEngine.Random.Range(startPos.z + distanceLeftY, startPos.z + distanceRightY));

        pointVisual.SetActive(true);
        itemBeUsed = false;
    }

/*#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(transform.position.x + (distanceRightX + distanceLeftX), 0, transform.position.z + (distanceRightY + distanceLeftY)), new Vector3(Mathf.Abs(distanceLeftX) + Mathf.Abs(distanceRightX), 1, Mathf.Abs(distanceLeftY) + Mathf.Abs(distanceRightY))); //Рисуем куб
    }
#endif*/
}