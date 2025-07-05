using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float launchSpeed = 3f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private SphereCollider sphereTrigger;
    [SerializeField] private Material[] materials;
    private List<BallController> _neighbours = new List<BallController>();
    private List<BallController> _allNeighbours = new List<BallController>();
    private GameManager _gameManager;
    private BallSpawner _ballSpawner;
    private bool _wasLaunched = false;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _ballSpawner = FindObjectOfType<BallSpawner>();
        SetColor((BallColor)Random.Range(0, 5));
        name = $"{_gameManager.nextID++}";
        StartCoroutine(ScheduledPurge());
    }

    public void FixedUpdate()
    {
        if (transform.position.y <= deathHeight)
            Destroy(this.gameObject);
        if (_gameManager.gameStarted && !_gameManager.gameOver && IsFixed()) MoveAlongRamp();
        StartCoroutine(DelayedClear());
    }

    void ReleaseMyselfAndNeighbours()
    {
        if (!_wasLaunched) return;
        var neighbours = GetNeighbours();
        if (neighbours.Count < 3)
        {
            _wasLaunched = false;
            return;
        }
        foreach (BallController neighbour in neighbours.ToList()) neighbour.Release();
        Release();
        _ballSpawner.DropIsolatedBallIslands();
        _wasLaunched = false;
    }

    public void SetColor(BallColor color)
    {
        ballColor = color;
        meshRenderer.material = materials[(int)color];
        if (currentState == BallState.ToBeLaunched)
        {
            rigidBody.isKinematic = true;
            sphereTrigger.radius = 0.5f;
        }
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
        return currentState == BallState.Fixed && rigidBody.isKinematic;
    }

    public bool IsReleased()
    {
        return currentState == BallState.AfterFixed;
    }

    public void PrepareLaunch()
    {
        currentState = BallState.ToBeLaunched;
        sphereTrigger.radius = 0.5f;
    }

    public void Launch(Vector3 position)
    {
        if (currentState != BallState.ToBeLaunched) return;
        currentState = BallState.BeforeFixed;
        _wasLaunched = true;
        rigidBody.isKinematic = false;
        var direction = position - transform.position;
        direction.Normalize();
        rigidBody.AddForce(direction * launchSpeed, ForceMode.Impulse);
        sphereTrigger.radius = 0.55f;
    }

    public void Fix(float delay = 2f)
    {
        if (currentState != BallState.BeforeFixed) return;
        if (_wasLaunched) delay = 0.05f;
        currentState = BallState.Fixed;
        rigidBody.velocity = Vector3.zero;
        rigidBody.drag = 2f;
        rigidBody.angularDrag = 2f;
        StartCoroutine(DelayedKinematic(delay));
        sphereTrigger.radius = 0.6f;
    }

    public void Release(bool force = false)
    {
        if (currentState != BallState.Fixed && !force) return;
        currentState = BallState.AfterFixed;
        if (_gameManager.gameStarted && !_gameManager.gameOver) _gameManager.score += 10;
        _neighbours.Clear();
        _allNeighbours.Clear();
        sphereTrigger.enabled = false;
        rigidBody.drag = 0f;
        rigidBody.angularDrag = 0.05f;
        rigidBody.isKinematic = false;
        StartCoroutine(Unstuck());
    }

    private void OnDestroy()
    {
        _neighbours.Clear();
        _allNeighbours.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball)
        {
            FixAnyNeighbour(ball);
            if (ball.GetColor()==ballColor && !_neighbours.Contains(ball))
            {
                _neighbours.Add(ball);
            }
            if (!_allNeighbours.Contains(ball)) _allNeighbours.Add(ball);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ball = other.gameObject.GetComponent<BallController>();
        if (ball)
        {
            if (_neighbours.Contains(ball)) _neighbours.Remove(ball);
            if (_allNeighbours.Contains(ball)) _allNeighbours.Remove(ball);
        }
    }

    public List<BallController> GetNeighbours(bool all = false, List<BallController> visited = null)
    {
        var tempNeighbours = _neighbours;
        if (all) tempNeighbours = _allNeighbours;
        if (visited == null) visited = new List<BallController>();
        visited.Add(this);
        tempNeighbours.Add(this);
        foreach (BallController neighbour in tempNeighbours.ToList())
        {
            if (visited.Contains(neighbour)) continue;
            if (!neighbour || neighbour.IsReleased())
            {
                if (tempNeighbours.Contains(neighbour)) tempNeighbours.Remove(neighbour);
                continue;
            }
            var temp = neighbour.GetNeighbours(all, visited);
            foreach (var t in temp)
            {
                if (tempNeighbours.Contains(t)) continue; //to prevent duplicating of same neighbours
                tempNeighbours.Add(t);
            }
        }
        // string msg = "From " + name + ": ";
        // foreach (BallController neighbour in tempNeighbours)
        // {
        //      msg += neighbour.name + ", ";
        // }
        // Debug.Log(msg);
        return tempNeighbours;
    }

    public BallColor GetColor() { return ballColor; }

    void FixAnyNeighbour(BallController ball)
    {
        if (currentState == BallState.Fixed) ball.Fix();
    }

    enum BallState
    {
        BeforeFixed,
        Fixed,
        AfterFixed,
        ToBeLaunched,
    }

    public enum BallColor
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
        yield return new WaitForSecondsRealtime(delay);
        if (currentState == BallState.Fixed)
        {
            rigidBody.isKinematic = true;
        }
    }

    IEnumerator Unstuck()
    {
        yield return new WaitForSeconds(1f);
        var velocity = Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(rigidBody.velocity.y) +
                       Mathf.Abs(rigidBody.velocity.z);
        Vector3 hitDirection;
        if (transform.position.y < wallEndHeight) hitDirection = Vector3.up;
        else hitDirection = Vector3.back + Vector3.up; //to push against vertical wall
        if (velocity < movementSpeed + 1f)
            rigidBody.AddForce(hitDirection * (launchSpeed * 1.5f), ForceMode.Impulse);
    }

    IEnumerator DelayedClear()
    {
        yield return new WaitForFixedUpdate(); //that executes AFTER OnTriggerXXX
        if (_gameManager.gameStarted && !_gameManager.gameOver && IsFixed()) ReleaseMyselfAndNeighbours();
        _neighbours.RemoveAll(ball => !ball); //removes non-existent balls
        _allNeighbours.RemoveAll(ball => !ball); //removes non-existent balls
    }

    IEnumerator ScheduledPurge()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitForFixedUpdate(); //that executes AFTER OnTriggerXXX
            if (_wasLaunched) continue; //that executes AFTER ReleaseMyselfAndNeighbours
            if (_ballSpawner.isCurrentlyDroppingIsolatedBalls) continue; //that executes AFTER DropIsolatedBallIslands
            _neighbours.Clear();
            _allNeighbours.Clear();
            if (_gameManager.gameOver) break;
        }
    }
}
