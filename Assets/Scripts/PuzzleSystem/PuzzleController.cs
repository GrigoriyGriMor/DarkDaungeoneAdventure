using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleController : MonoBehaviour
{
    [SerializeField] private UnityEvent _puzzleIsReady = new();

    [SerializeField] private PuzzleObject[] _objList = new PuzzleObject[0]; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
