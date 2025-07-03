using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallState currentState = BallState.BeforeFixed;
    [SerializeField] private BallColor ballColor;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float deathHeight = -20f;
    private float wallEndHeight = 18f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material[] materials;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        SetColor((BallColor)Random.Range(0, 5));
    }

    public void FixedUpdate()
    {
        if (transform.position.y <= deathHeight)
            Destroy(this.gameObject);
        if (_gameManager.gameStarted && !_gameManager.gameOver && IsFixed())
        {
            MoveAlongRamp();
        }
    }

    BallColor GetColor() { return ballColor; }

    void SetColor(BallColor color)
    {
        ballColor = color;
        meshRenderer.material = materials[(int)color];
    }

    void MoveAlongRamp()
    {
        RaycastHit hit;
        //3 is separate layer for game field (ramp), so raycast would ONLY hit against that ramp and not other items
        int layerMask = 1 << 3;
        Vector3 hitDirection;
        bool notStickToWall = transform.position.y < wallEndHeight;
        if (notStickToWall) hitDirection = Vector3.down;
        else hitDirection = Vector3.forward + Vector3.down; //to move along vertical wall
        Debug.DrawRay(transform.position, hitDirection * 10f, Color.red);
        if (Physics.Raycast(transform.position, hitDirection, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 direction = hit.point - transform.position;
            Vector3 slope = Vector3.ProjectOnPlane(direction, surfaceNormal).normalized;
            transform.position += slope * (movementSpeed * Time.deltaTime);
        }
    }

    public bool IsFixed()
    {
        return currentState == BallState.Fixed;
    }

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

    enum BallColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Pink
    }

    IEnumerator DelayedKinematic(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == BallState.Fixed)
            rigidBody.isKinematic = true;
    }
}
