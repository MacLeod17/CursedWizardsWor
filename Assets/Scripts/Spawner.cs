using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum eType
    {
        TimerRepeat,
        TimerOneTime,
        Event
    }

    public GameObject spawnGameObject;
    public float spawnTimeMin = 2;
    public float spawnTimeMax = 5;
    public bool IsSpawnChild = true;
    public eType type = eType.TimerRepeat;

    public string onSpawnEvent;
    public string onActivateEvent;
    public string onDeactivateEvent;

    public bool active = true;

    float spawnTimer;
    int spawnCount;

    void Start()
    {
        spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax);

        if (!string.IsNullOrEmpty(onSpawnEvent)) EventManager.Instance.Subscribe(onSpawnEvent, OnSpawn);
        if (!string.IsNullOrEmpty(onActivateEvent)) EventManager.Instance.Subscribe(onActivateEvent, OnActivate);
        if (!string.IsNullOrEmpty(onDeactivateEvent)) EventManager.Instance.Subscribe(onDeactivateEvent, OnDeactivate);
    }

    void Update()
    {
        if (!active) return;

        if (transform.childCount == 0)// && Game.Instance.State == Game.eState.Game)
        {
            spawnTimer -= Time.deltaTime;
        }

        if (spawnTimer <= 0 && (type == eType.TimerRepeat || type == eType.TimerOneTime && spawnCount == 0))
        {
            spawnTimer = Random.Range(spawnTimeMin, spawnTimeMax);
            OnSpawn();
        }
    }

    public void OnSpawn()
    {
        spawnCount++;
        Transform parent = (IsSpawnChild) ? transform : null;
        Instantiate(spawnGameObject, transform.position, transform.rotation, parent);
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
