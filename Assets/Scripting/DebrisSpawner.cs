using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] GameObject debrisPrefab = null;
    [SerializeField] float spawnInterval = 3.0f;

    Coroutine spawnRoutine = null;
    WaitForSeconds delay = null;

    private void Awake()
    {
        delay = new WaitForSeconds(spawnInterval);
    }

    private void OnEnable()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            GameObject spawnedObject = Instantiate(debrisPrefab);

            spawnedObject.transform.position = transform.position;
            spawnedObject.transform.rotation = Quaternion.identity;

            yield return delay;
        }
    }
}
