using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform balls;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        EnhancedTouchSupport.Enable();
        ballPrefab.GetComponent<BallController>().PrepareLaunch();
    }

    void Update()
    {
        if (_gameManager.gameStarted && !_gameManager.gameOver) foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Ended)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var ball = this.GetComponentInChildren<BallController>();
                    if (ball)
                    {
                        ball.Launch(hit.point);
                        ball.transform.SetParent(balls);
                        StartCoroutine(SpawnNewBall());
                    }
                }
            }
        }
    }

    IEnumerator SpawnNewBall()
    {
        yield return new WaitForSeconds(0.2f);
        Instantiate(ballPrefab, transform.position, Quaternion.identity, transform);
    }
}
