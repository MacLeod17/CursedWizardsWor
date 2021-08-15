using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSpawner : MonoBehaviour
{
    public enum eType
    {
        TimerRepeat,
        TimerOneTime,
        Event
    }

    public GameObject[] spawnGameObjects;
    public string[] objectTags = { "Enemy" };
    public int maxObjectCount = 10;

    public float spawnTimeMin = 2;
    public float spawnTimeMax = 5;
    public bool IsSpawnChild = true;
    public eType type = eType.TimerRepeat;

    public string onSpawnEvent;
    public string onActivateEvent;
    public string onDeactivateEvent;

    public bool active = true;

    [SerializeField]
    float spawnTimer;
    [SerializeField]
    [Tooltip("Number of objects of given type(s) currently in the scene")]
    int spawnCount;

    void Start()
    {
        spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax);

        if (!string.IsNullOrEmpty(onSpawnEvent)) EventManager.Instance.Subscribe(onSpawnEvent, OnSpawn);
        if (!string.IsNullOrEmpty(onActivateEvent)) EventManager.Instance.Subscribe(onActivateEvent, OnActivate);
        if (!string.IsNullOrEmpty(onDeactivateEvent)) EventManager.Instance.Subscribe(onDeactivateEvent, OnDeactivate);

        foreach (string typeTag in objectTags)
        {
            spawnCount += GameObject.FindGameObjectsWithTag(typeTag).Length;
        }
    }

    void Update()
    {
        if (!active) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0 && (type == eType.TimerRepeat || type == eType.TimerOneTime && spawnCount == 0))
        {
            spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax);
            OnSpawn();
        }
    }

    public void OnSpawn()
    {
        spawnCount = 0;
        foreach (string typeTag in objectTags)
        {
            spawnCount += GameObject.FindGameObjectsWithTag(typeTag).Length;
        }

        if (spawnCount < maxObjectCount)
        {
            spawnCount++;
            Transform parent = (IsSpawnChild) ? transform : null;
            Instantiate(spawnGameObjects[Random.Range(0, spawnGameObjects.Length)], transform.position, transform.rotation, parent);
        }
    }

    public void OnActivate()
    {
        active = true;
    }

    public void OnDeactivate()
    {
        active = false;
    }
}
