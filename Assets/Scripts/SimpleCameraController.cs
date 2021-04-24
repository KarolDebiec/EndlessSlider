using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offSet;
    public Vector3 destructionPos;
    public Vector3 afterDeathOffset;
    public bool animationTrigger;
    public float speed;
    public float rotSpeed;
    public float speedMultiplier;
    public float rotSpeedMultiplier;
    public Vector3 targetRotDeg;
    public Quaternion startingRot;
    private Quaternion targetRot;
    public bool stopMov;
    private PlayerController playerCtrl;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerCtrl = player.GetComponent<PlayerController>();
        offSet = gameObject.transform.position;
        startingRot = gameObject.transform.rotation;
        targetRot = Quaternion.Euler(targetRotDeg);
    }
    void FixedUpdate()
    {
        if(!animationTrigger)
        {
            gameObject.transform.position = offSet + player.transform.position;
            rotSpeedMultiplier = speedMultiplier = playerCtrl.speed/15;
        }
        else
        {   
            if(!stopMov)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(destructionPos.x - afterDeathOffset.x, destructionPos.y + afterDeathOffset.y, destructionPos.z + afterDeathOffset.z), Time.fixedDeltaTime * speed* speedMultiplier);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.fixedDeltaTime * rotSpeed* rotSpeedMultiplier);
                if(transform.position== new Vector3(destructionPos.x - afterDeathOffset.x, destructionPos.y + afterDeathOffset.y, destructionPos.z + afterDeathOffset.z) || transform.rotation == targetRot)
                {
                    stopMov = true;
                }
            }
        }
    }
}