using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SimpleCameraController SCCScript;
    private GameController gameCtrl;
    public float cubeSize = 0.2f;
    public int cubesInRow = 5;
    public float pieceShotSpeedMultiplier = 20;
    public GameObject piecePref;

    private float cubesPivotDistance;
    private Vector3 cubesPivot;

    public float shiftingSpeed;

    public float speed; //controlled in gamecontroller script
    public float startingSpeed;
    public float deathSpeed;

    private Rigidbody rb;
    private Collider m_Collider;
    private Renderer rend;
    public bool boom;

    public bool startTimer;
    public float timeToCount;
    public float time;

    void Start()
    {
        SCCScript = GameObject.FindWithTag("MainCamera").GetComponent<SimpleCameraController>();
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        Time.timeScale = 1;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        m_Collider = GetComponent<Collider>();
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
        startingSpeed = speed;
    }

    void Update()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        if (boom)
        {
            SCCScript.destructionPos=gameObject.transform.position;
            explode();
            boom = false;
            startTimer = true;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        if (startTimer)
        {
            time += Time.deltaTime;
            if (time < timeToCount)
            {
                //Debug.Log(1-Mathf.InverseLerp(0, timeToCount, time));
                Time.timeScale= 1-Mathf.InverseLerp(0, timeToCount, time);
                if((1 - Mathf.InverseLerp(0, timeToCount, time))<0.15)
                {
                    startTimer = false;
                    Time.timeScale = 0.15f;
                    time = 0;
                }
            }
            else
            {
                startTimer = false;
                time = 0;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////
    }

    void FixedUpdate()
    {
        if (gameCtrl.gameStarted)
        {
            transform.Translate(Vector3.back * Time.fixedDeltaTime * speed, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle")
        {
            SCCScript.destructionPos = gameObject.transform.position;
            SCCScript.animationTrigger=true;
            startTimer = true;
            explode();
            deathSpeed=speed;
            speed = 0;
            gameCtrl.Death();
        }
        if (other.gameObject.tag == "stageEnd")
        {
            gameCtrl.NextStage();
        }
    }

    public void explode()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //make object disappear
        rend.enabled = false;
        m_Collider.enabled = false;

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }
    }

    void createPiece(int x, int y, int z)
    {

        //create piece
        GameObject piece;
        piece=Instantiate(piecePref, transform.position + new Vector3(cubeSize * x, cubeSize * y + (float)(0.05), cubeSize * z) - cubesPivot, Quaternion.identity);

        //set piece position and scale
        //piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y + (float)(0.05), cubeSize * z) - cubesPivot;
        //piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        piece.GetComponent<Rigidbody>().AddForce(-transform.forward * speed * pieceShotSpeedMultiplier);
    }



}
