using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine : MonoBehaviour
{
    [SerializeField] private GameObject ballsParent;

    // private void OnCollisionEnter(Collision other)
    // {
    //     var ball = other.gameObject.GetComponent<BallController>();
    //     if (ball) ball.Fix();
    // }
    //
    // private void OnCollisionStay(Collision other)
    // {
    //     var ball = other.gameObject.GetComponent<BallController>();
    //     if (ball) ball.Fix();
    // }

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball) ball.Fix(0f);
        // var balls = ballsParent.GetComponentsInChildren<BallController>();
        // foreach (var ball in balls)
        // {
        //     ball.Fix();
        // }
    }

    private void OnTriggerStay(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball) ball.Fix(0f);
        // var balls = ballsParent.GetComponentsInChildren<BallController>();
        // foreach (var ball in balls)
        // {
        //     ball.Fix();
        // }
    }
}
