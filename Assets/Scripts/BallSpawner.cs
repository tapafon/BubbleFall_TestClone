using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public bool enableRegularSpawn = false;
    [SerializeField] private GameObject ballGroup;
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform regularSpawnPoint;
    [SerializeField] private float maxSpawnHeight;

    public void InitialSpawn()
    {
        for (float i = initialSpawnPoint.position.y; i < maxSpawnHeight; i += 1.8f)
        {
            var pos = new Vector3(initialSpawnPoint.position.x, i, initialSpawnPoint.position.z);
            Instantiate(ballGroup, pos, Quaternion.identity, transform);
        }
    }
}
