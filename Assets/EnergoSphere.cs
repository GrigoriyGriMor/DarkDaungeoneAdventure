using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnergoSphere : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemysTransforms = new List<Transform>();
    [SerializeField] private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    [SerializeField] private List<Material> _material = new List<Material>();

    public int numOfEnemies;

    [System.Obsolete]
    private void Start()
    {
        foreach (Transform t in _enemysTransforms)
        {
            var go = Instantiate(new GameObject(), this.transform);
            var lr = go.AddComponent<LineRenderer>();
            _lineRenderers.Add(lr);
            lr.SetPosition(0, this.transform.position);
            lr.SetPosition(1,t.position+ new Vector3 (0,1,0));
            lr.SetMaterials(_material);
            lr.SetWidth(0.5f, 0.5f);
            numOfEnemies++;
        }
        Warrior1.deadEvent += OnDeadWarrior;
    }

    [System.Obsolete]
    private  void OnDeadWarrior(Transform warrior)
    {
        for(var i = 0; i< _enemysTransforms.Count; i++)
        {
            if (_enemysTransforms[i] == warrior)
            {
                _enemysTransforms[i] = null;
               // _lineRenderers[i].gameObject.SetActive(false);
            }
        }

        numOfEnemies--;
        if(numOfEnemies == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _enemysTransforms.Count; i++)
        {
            if (_lineRenderers[i] != null && _enemysTransforms[i]!= null)
            {
                _lineRenderers[i].SetPosition(0, this.transform.position);
                _lineRenderers[i].SetPosition(1, _enemysTransforms[i].position + new Vector3(0, 1, 0));
            }
            else if (_enemysTransforms[i] == null)
            {
                _lineRenderers[i].gameObject.SetActive(false);
            }
        }
    }
}
