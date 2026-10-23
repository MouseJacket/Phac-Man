/*
 * Player controller script. Largely taken directly from tutorial, but modified slightly to fit this project.
 */


using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
     private Rigidbody _rb; 
     private float _moveX;
     private float _moveY;
     private ScoreManager _scoreMan;
     public bool invulnerable;
     private AIManager _aiMan;
     [SerializeField] private float invulnerableTime;
     [SerializeField] private float speed;
     [SerializeField] private Transform center;
     
     
    void Start()
    {//Sets up player controller
        _rb = GetComponent<Rigidbody>();
        _scoreMan = FindFirstObjectByType<ScoreManager>();
        invulnerable = false;
        _aiMan = FindFirstObjectByType<AIManager>();
    }

    void OnMove(InputValue movementValue)
    {//sets movement variables of player controller using input system;
        Vector2 movementVector = movementValue.Get<Vector2>();
        _moveX = movementVector.x; 
        _moveY = movementVector.y; 
    }

    void FixedUpdate() 
    {//moves player controller based on movement variables
        Vector3 movement = new Vector3 (_moveX, 0.0f, _moveY);
        _rb.AddForce(movement * speed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {//Pauses game
            _scoreMan.Pause();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {//Restarts game
            SceneManager.LoadScene(0); 
        }

    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Pellet")) 
        {//Collects Pellet
            other.gameObject.SetActive(false);
            _scoreMan.score++;
            if (other.name.Contains("Super"))
            {// If super pellet, activate invulnerability;
                _scoreMan.score += 14;
                invulnerable = true;
                StartCoroutine(_aiMan.SuperPellet(invulnerableTime));
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (invulnerable)
            {//If invulnerable, catches ghost
                _scoreMan.score += 100;
                other.transform.position = center.position;
            }
            else
            {//Otherwise, Lose
                _scoreMan.Lose();
            }
        }
    }
}