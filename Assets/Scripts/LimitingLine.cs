using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitingLine : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball)
        {
            if (ball.IsFixed())
            {
                gameManager.EndGame();
            }
        }
    }
}
