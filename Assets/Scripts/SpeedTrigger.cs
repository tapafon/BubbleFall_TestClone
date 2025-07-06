
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private int _currentBalls = -1;

    void FixedUpdate()
    {
        if (_currentBalls == 0) gameManager.movementSpeed = 2f;
        else gameManager.movementSpeed = 0.25f;
        _currentBalls = 0; //OnTriggerStay is checked every FixedUpdate

    }

    private void OnTriggerStay(Collider other)
    {
        var ball = other.GetComponent<BallController>();
        if (ball)
        {
            if (!ball.IsReleased()) _currentBalls++;
        }
    }
}
