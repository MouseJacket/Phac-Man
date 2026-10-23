/*
This is the main script controlling AI behavior. The original pacman AI was pretty complex, and I wanted to 
emulate the way that it works.

Default Behaviors:
    Blinky (Red ghost) Charges straight for the player
    Pinky (Pink ghost) aims for a space ahead of the player in the direction that they are moving
    Inky (Teal Ghost) triangulates the space that Pinky aims for with the center of the board, trying to flank
    Clyde (Orange ghost) Gets close to player, then retreats to corner
    
Scatter:
    every 15 seconds (By default), all ghosts retreat to their corner for 5 seconds to give player time to recouperate
    
Frightened:
    When player eats a power pill, ghosts run away
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    [Header("Blinky (Red), Pinky (Pink), Inky (Teal), Clyde (Orange)")]//Order template that variables in aggregates follow
    [SerializeField] private NavMeshAgent[] ghosts; //Ghost Navmeshes
    [SerializeField] private Transform[] retreats; //In scatter mode, ghosts seek out specific corners on the board
    [SerializeField] private float clydeDistance; // Distance clyde gets to the player before running away
    [SerializeField] private bool clydeRetreating; //Is clyde retreating
    [SerializeField] private float pinkyDistance; //Multiplier for how far infront of player pinky is aiming
    [SerializeField] private float inkySpeed; //If player is going under this speed, inky goes after player
    [SerializeField] private float inkyDistance; //Distance inky stays from player when stalking
    [SerializeField] private float scatterTime; // How long each scatter lasts for
    [SerializeField] private float scatterInterval; //How often do ghosts scatter
    [SerializeField] private int behavior; //0 == default, 1 == scatter, 2 == frightened
    [SerializeField] private Transform center; //Center of the map
    [SerializeField] private Material vulnerableMat; //Material ghosts get when vulnerable
    private Vector3 inkyTarget;//Inky's target for triangulation strat
    private Vector3 pinkyTarget;//Pinky's target to cut off player
    private NavMeshAgent blinky, pinky, inky, clyde;
    private Transform player;
    private bool retreating; //Are ghosts currently retreating
    private float clydeSavedTime; //Saved time interval so I may avoid coroutines with Clyde;
    private float allSavedTime; //Time ghosts started retreating
    private Rigidbody pRb; // Players rigidbody
    private PlayerController pController; // Player controller
    
    
    private void Start()
    {// Setting up variables
        blinky = ghosts[0];
        pinky = ghosts[1];
        inky = ghosts[2];
        clyde = ghosts[3];
        player = FindFirstObjectByType<PlayerController>().gameObject.transform;
        pController = player.gameObject.GetComponent<PlayerController>();
        pRb = player.gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (behavior == 0 && ((int)Time.time % scatterInterval == 0))
        {// Scatter interval
            behavior = 1;
        }
        switch (behavior)
        {
            case 0: // default
                blinky.SetDestination(player.position); //Blinky
                if (Vector3.Distance//Clyde
                        (clyde.gameObject.transform.position, player.gameObject.transform.position) < clydeDistance
                    && !clydeRetreating)
                {//Clyde starts retreating
                    clydeRetreating = true;
                    clyde.SetDestination(retreats[3].position);
                    clydeSavedTime = Time.time;
                } // Clyde retreats until enough time has passed
                else if (clydeRetreating)
                {
                    if (Time.time > clydeSavedTime + scatterTime)
                    {
                        clydeRetreating = false;
                        clyde.SetDestination(player.position);
                    }
                    else//Clyde targets player again
                    {
                        clyde.SetDestination(retreats[3].position);
                    }
                }
                else
                {
                    clyde.SetDestination(player.position);
                }
                //Pinky targets a space in front of player depending on how fast player is moving
                pinkyTarget = player.position + (pRb.linearVelocity * pinkyDistance);//Pinky
                Debug.DrawLine(pinky.transform.position,pinkyTarget,Color.pink);
                pinky.SetDestination(pinkyTarget);
                //Inky takes pinkys calculations and triangulates them with center of map
                Vector3 inkyDirection = (pinkyTarget - center.position).normalized;//Inky
                float inkyMagnitude = Vector3.Distance(pinkyTarget, center.position) / 2;
                inkyTarget = pinkyTarget - (inkyDirection * inkyMagnitude);
                if (pRb.linearVelocity.magnitude < inkySpeed || 
                    Vector3.Distance(player.position, inky.transform.position)<inkyDistance)
                {// If player is close or moving too slow, Inky targets them
                    inky.SetDestination(player.position);
                }
                else
                {
                    inky.SetDestination(inkyTarget);
                }
                Debug.DrawLine(inky.transform.position,inkyTarget,Color.turquoise);
                break;
            
            case 1: // scatter
                Debug.Log("Retreating");
                if (!retreating)
                {
                    allSavedTime = Time.time;
                    retreating = true;
                }

                if (Time.time > allSavedTime + scatterTime)
                {
                    retreating = false;
                    behavior = 0;
                }
                for (int i = 0; i < 4; i++)
                {
                    Debug.Log($"{ghosts[i].name} traveling to {retreats[i].name}");
                    ghosts[i].SetDestination(retreats[i].position);
                }
                break;
            case 2: // frightened
                Debug.Log("Frightened");
                for (int i = 0; i < 4; i++)
                {
                    Run(ghosts[i]);
                }
                break;
        }
    }
    
    private void Run(NavMeshAgent ghost)
    { // Ghosts pick a spot opposite of player and move towards it
        Vector3 ghostPos = ghost.gameObject.transform.position;
        Vector3 direction = (player.position - ghostPos).normalized;
        ghost.SetDestination(ghostPos - direction);
    }

    public IEnumerator SuperPellet(float seconds)
    { //Ghosts become vulnerable for a few seconds
        behavior = 2;
        List<Material> ghostMats = new List<Material>();
        for (int i = 0; i < 4; i++)
        {
            MeshRenderer renderer = ghosts[i].gameObject.GetComponent<MeshRenderer>();
            ghostMats.Add(renderer.material);
            renderer.material = vulnerableMat;
        }
        yield return new WaitForSeconds(seconds);
        behavior = 0;
        for (int i = 0; i < 4; i++)
        {
            MeshRenderer renderer = ghosts[i].gameObject.GetComponent<MeshRenderer>();
            renderer.material = ghostMats[i];
        }

        pController.invulnerable = false;
    }
}
