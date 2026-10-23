/*
 * Basic camera script from tutorial. Considered making new one with cinemachine, but decided that this works fine.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position; 
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;  
        transform.LookAt(player.transform.position);
    }
}