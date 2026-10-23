/*
 * Really basic script allowing player to teleport from one side of the map to the other
 */


using System;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    [SerializeField] private TransitionScript outPort;
    private Transform outPos;
    private float offset;
    [SerializeField] private float offsetMult;

    private void Start()
    { // sets transform and distance
        outPos = outPort.transform;
        offset = (transform.position-outPos.position).normalized.x; //allows player to use warpgate without teleporting back instantly
    }

    void OnTriggerEnter(Collider other)
    { //Sets position to new 
        float y = other.transform.position.y;
        float z = other.transform.position.z;
        float x = outPos.position.x+(offset*offsetMult);
        other.transform.position = new Vector3(x, y, z);
    }
}
