using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter plateCounter;
    [SerializeField] private Transform CounterTopPoint;
    [SerializeField] private Transform PlateVisualPrefab;

    private List<GameObject> plateVisualGameObjects;

    private void Awake()
    {
        plateVisualGameObjects = new List<GameObject>();
    }

    private void Start()
    {
        plateCounter.OnPlateSpawned += OnPlateSpawned;
        plateCounter.OnPlateDestroyed += OnPlateDestroyed;
    }

    private void OnPlateSpawned()
    {
        Transform plateVisualTransform = Instantiate(PlateVisualPrefab, CounterTopPoint);
        float plateOffsetY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0f, plateOffsetY * plateVisualGameObjects.Count, 0f);
        plateVisualGameObjects.Add(plateVisualTransform.gameObject);
    }

    private void OnPlateDestroyed()
    {
        if (plateVisualGameObjects.Count > 0)
        {
            GameObject plateVisualGameObject = plateVisualGameObjects[plateVisualGameObjects.Count - 1];
            plateVisualGameObjects.Remove(plateVisualGameObject);
            Destroy(plateVisualGameObject);
        }
    }
}
