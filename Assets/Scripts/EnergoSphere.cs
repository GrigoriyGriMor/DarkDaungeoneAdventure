using System.Collections.Generic;
using UnityEngine;

public class EnergoSphere : MonoBehaviour
{
    [SerializeField] private List<Transform> enemyTransforms = new List<Transform>();
    [SerializeField] private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    [SerializeField] private List<Material> materials = new List<Material>();

    public int numOfEnemies;

    // Кэшируем transform объекта для оптимизации
    private Transform _selfTransform;

    [System.Obsolete]
    private void Start()
    {
        _selfTransform = transform;

        // Для каждого врага создаем LineRenderer и настраиваем его
        foreach (Transform enemy in enemyTransforms)
        {
            GameObject go = Instantiate(new GameObject("LineRenderer"), _selfTransform);
            LineRenderer lr = go.AddComponent<LineRenderer>();
            lineRenderers.Add(lr);

            // Устанавливаем начальные позиции линии
            lr.SetPosition(0, _selfTransform.position);
            lr.SetPosition(1, enemy.position + Vector3.up);

            // Назначаем материалы и ширину линии
            lr.SetMaterials(materials);
            lr.SetWidth(0.15f, 0.15f);

            numOfEnemies++;
        }

        // Подписываемся на событие смерти воина
        Warrior1.deadEvent += OnDeadWarrior;
    }

    [System.Obsolete]
    private void OnDeadWarrior(Transform warrior)
    {
        // При смерти соответствующий враг заменяется на null
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            if (enemyTransforms[i] == warrior)
            {
                enemyTransforms[i] = null;
            }
        }

        numOfEnemies--;
        if (numOfEnemies <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Обновляем позиции линий, если враг существует, иначе отключаем LineRenderer
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            if (enemyTransforms[i] != null && lineRenderers[i] != null)
            {
                lineRenderers[i].SetPosition(0, _selfTransform.position);
                lineRenderers[i].SetPosition(1, enemyTransforms[i].position + Vector3.up);
            }
            else if (enemyTransforms[i] == null && lineRenderers[i] != null)
            {
                lineRenderers[i].gameObject.SetActive(false);
            }
        }
    }
}