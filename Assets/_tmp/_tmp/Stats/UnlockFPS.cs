using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockFPS : MonoBehaviour
{
    private int targetFrameRate = 290;
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
