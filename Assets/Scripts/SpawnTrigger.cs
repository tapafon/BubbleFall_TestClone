using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private BallSpawner ballSpawner;
    private int _currentBalls = -1;

    void FixedUpdate()
    {
        if (_currentBalls == 0) ballSpawner.TriggerRegularSpawn();
        _currentBalls = 0; //OnTriggerStay is checked every FixedUpdate
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<BallController>()) _currentBalls++;
    }
}
