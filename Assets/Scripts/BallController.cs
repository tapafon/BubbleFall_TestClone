using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallState currentState = BallState.BeforeFixed;
    [SerializeField] private Rigidbody rigidBody;

    public void Fix(float delay = 2f)
    {
        if (currentState != BallState.BeforeFixed) return;
        currentState = BallState.Fixed;
        rigidBody.velocity = Vector3.zero;
        rigidBody.drag = 2f;
        rigidBody.angularDrag = 2f;
        StartCoroutine(DelayedKinematic(delay));
    }

    public void Release()
    {
        if (currentState != BallState.Fixed) return;
        currentState = BallState.AfterFixed;
        rigidBody.drag = 0f;
        rigidBody.angularDrag = 0.05f;
        rigidBody.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == BallState.Fixed)
        {
            var ball = other.gameObject.GetComponent<BallController>();
            if (ball) ball.Fix();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState == BallState.Fixed)
        {
            var ball = other.gameObject.GetComponent<BallController>();
            if (ball) ball.Fix();
        }
    }

    enum BallState
    {
        BeforeFixed,
        Fixed,
        AfterFixed,
    }

    IEnumerator DelayedKinematic(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        rigidBody.isKinematic = true;
    }
}
