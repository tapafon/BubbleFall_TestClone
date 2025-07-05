using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public bool enableRegularSpawn = false;
    public bool isCurrentlyDroppingIsolatedBalls = false;
    [SerializeField] private GameObject ballGroup;
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform regularSpawnPoint;
    [SerializeField] private float maxSpawnHeight;

    public void TriggerRegularSpawn()
    {
        if (enableRegularSpawn)
        {
            Instantiate(ballGroup, regularSpawnPoint.position, Quaternion.identity, transform);
        }
    }

    public void InitialSpawn()
    {
        for (float i = initialSpawnPoint.position.y; i < maxSpawnHeight; i += 1.8f)
        {
            var pos = new Vector3(initialSpawnPoint.position.x, i, initialSpawnPoint.position.z);
            Instantiate(ballGroup, pos, Quaternion.identity, transform);
        }
    }

    public void ReleaseBalls()
    {
        var balls = GetComponentsInChildren<BallController>();
        foreach (var ball in balls)
        {
            ball.Release();
        }
    }

    public void DropIsolatedBallIslands()
    {
        isCurrentlyDroppingIsolatedBalls = true;
        var balls = GetComponentsInChildren<BallController>().ToList();
        var fixedOnTop = GetMostRecentFixedBall(balls).GetNeighbours(true);
        foreach (var ball in balls)
        {
            if (!fixedOnTop.Contains(ball)) ball.Release();
        }
        isCurrentlyDroppingIsolatedBalls = false;
    }

    BallController GetMostRecentFixedBall(List<BallController> balls)
    {
        BallController result = null;
        ulong maxid = 0;
        foreach (var ball in balls)
        {
            if (!ball.IsFixed()) continue;
            if (ulong.Parse(ball.name)>=maxid)
            {
                result = ball;
                maxid = ulong.Parse(ball.name);
            }
        }
        return result;
    }
}
