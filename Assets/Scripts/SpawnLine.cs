using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine : MonoBehaviour
{
    [SerializeField] private GameObject ballsParent;

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball) ball.Fix(0f);
    }

    private void OnTriggerStay(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball) ball.Fix(0f);
    }
}
