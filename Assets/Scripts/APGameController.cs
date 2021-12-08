using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APGameController : MonoBehaviour
{
    private static APGameController instance;
    public static APGameController Instance => instance;

    [SerializeField] private GameObject inputSystem;

    [SerializeField] private Text PointText;
    private int point = 0;

    [SerializeField] private Text TimerText;
    [SerializeField] private int maxGameTimeInMinuts = 2;
    private float second = 0;
    private bool timeGo = false;
    private bool useTimer = false;

    public bool gameIsPlayed = false;

    [SerializeField] private AudioClip backclip;

    private void Awake()
    {
        instance = this;
      //  PointText.text = 0.ToString();

        if (second <= 0 && maxGameTimeInMinuts > 0)
        {
            maxGameTimeInMinuts -= 1;
            second = 59;
        }

       // TimerText.text = $"{maxGameTimeInMinuts}:{second}";
    }

    private void Start()
    {
        if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.BackgroundClipPlay(backclip);
        GameStarted();
    }

    public void GameStarted()
    {
        gameIsPlayed = true;
        timeGo = true;
        inputSystem.SetActive(true);
    }

    public void GameEnded()
    {
        gameIsPlayed = false;
        timeGo = false;
        inputSystem.SetActive(false);
    }

    public void GameRestart()
    {

    }

    public void UpgradePoint(int i)
    {
        point += i;
       // PointText.text = point.ToString();
    }

    private void FixedUpdate()
    {
        if (timeGo)
        {
            second -= Time.deltaTime;

            if (Mathf.CeilToInt(second) <= 0 && maxGameTimeInMinuts > 0)
            {
                maxGameTimeInMinuts -= 1;
                second = 59;
            }
            else
            {
                if (maxGameTimeInMinuts <= 0 && Mathf.CeilToInt(second) <= 0 && maxGameTimeInMinuts <= 0)
                {
                //    TimerText.text = "TIME IS UP";
                    GameEnded();
                }
            }

            if (Mathf.CeilToInt(second) >= 10)
               // TimerText.text = $"{maxGameTimeInMinuts}:" + Mathf.CeilToInt(second).ToString();
            //else
            {
                if (Mathf.CeilToInt(second) == 5 && !useTimer)
                {
               //     TimerText.text = $"{maxGameTimeInMinuts}:0" + Mathf.CeilToInt(second).ToString();
                    StartCoroutine(TimerAnim());
                }
              //  else
                  //  TimerText.text = $"{maxGameTimeInMinuts}:0" + Mathf.CeilToInt(second).ToString();
            }    
        }
    }

    private IEnumerator TimerAnim()
    {
        useTimer = true;
        int i = 0;

        while (i < 5)
        {
            TimerText.fontSize = 90;
            yield return new WaitForSeconds(0.5f);

            TimerText.fontSize = 80;

            yield return new WaitForSeconds(0.5f);
            i++;
        }
    }
}
