using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class GameController : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;// from 1-6 are prefabs that are supposed to rest at start // 0 is a tunnel for changing skyboxes
    public GameObject lastFloor;
    public GameObject FloorPrefab;
    public float offset;
    public GameObject player;
    private PlayerController playerCtrl;
    private SoundController soundCtrl;
    private GameObject mCamera;
    private SimpleCameraController cameraCtrl;
    private float distance;
    public GameObject activeStage;
    public int stageNumber;
    //////////////////////////////////////////
    public float shiftingBracket;
    //////////////////////////////////////////
    public bool gameStarted;
    public bool dead;
    public bool canClickStart;
    //////////////////////////////////////////
    private Vector3 playerStartingPos;
    private Vector3 playerDeathPos;
    //////////////////////////////////////////
    public float score;
    public float scoreMultiplier;
    public Text scoreDisplay; 
    public float highScore=0;
    public Text highScoreText;
    public float deathPenalty;
    //////////////////////////////////////////
    public float intervalForNextStage; // time to wait for next stage 
    public float distanceToResp;// distance between player and next stage resp point
    //////////////////////////////////////////
    public GameObject deathPanel;
    public GameObject tapToStartPanel;
    //////////////////////////////////////////  
    public float speedGainMultiplier=1;
    public float maxSpeed=50;
    public float SpeedLimit = 100;
    //////////////////////////////////////////  
    public Material startingSkybox;
    //////////////////////////////////////////
    public SaveManager saveManagerScript;
    //////////////////////////////////////////
    public int floorType;

    public bool increasingSpeed;
    public VisualEffect warpEffect;
    void Start()
    {
        Time.timeScale = 1;
        player = GameObject.FindWithTag("Player");
        playerCtrl = player.GetComponent<PlayerController>();
        soundCtrl = GameObject.FindWithTag("Audio").GetComponent<SoundController>();
        mCamera = GameObject.FindWithTag("MainCamera");
        cameraCtrl = mCamera.GetComponent<SimpleCameraController>();
        saveManagerScript = GameObject.FindWithTag("saveManager").GetComponent<SaveManager>();
        distance = (-(lastFloor.transform.position.z)) - (-(player.transform.position.z));
        playerStartingPos=player.transform.position;
        canClickStart = true;
        tapToStartPanel.SetActive(true);
        scoreDisplay.text = "0";
    }

    
    void Update()
    {
        //////////////////////////////////////////
        if (!dead && gameStarted)
        {
            scoreMultiplier = playerCtrl.speed * (float)0.1;
            score += scoreMultiplier * Time.deltaTime;
            scoreDisplay.text = score.ToString("F0");
        }
        ////////////////////////////////////////// spawning new floors
        if (!dead && ((-(lastFloor.transform.position.z)) - (-(player.transform.position.z)) < distance+(-offset)))
        {
            lastFloor=Instantiate(FloorPrefab,new Vector3(lastFloor.transform.position.x, lastFloor.transform.position.y, lastFloor.transform.position.z + (-offset)), Quaternion.identity);
        }
        //////////////////////////////////////////
        if(!dead && gameStarted && playerCtrl.speed<maxSpeed && playerCtrl.speed<SpeedLimit)
        { 
            playerCtrl.speed += Time.deltaTime* speedGainMultiplier;
        }
        //////////////////////////////////////////
        ///////not efficient tbh
        if (maxSpeed> playerCtrl.speed && gameStarted && SpeedLimit > playerCtrl.speed)
        {
            increasingSpeed = true;
            warpEffect.SetFloat("WarpAmount",(float)1);
        }
        else
        {
            increasingSpeed = false;
            warpEffect.SetFloat("WarpAmount", 0);
        }

    }
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !gameStarted && canClickStart)
        {
            tapToStartPanel.SetActive(false);
            gameStarted = true;
        }
        //////////////////////////////////////////input controller to move player left and right///////////////////////// temporary cs this is for pc
        if (Input.GetMouseButton(0)&& gameStarted)
        {
            if (Input.mousePosition.x < (Screen.width / 2) && shiftingBracket > player.transform.position.x)
            {
                player.transform.Translate(Vector3.right * Time.fixedDeltaTime * playerCtrl.shiftingSpeed, Space.World);
            }
            if (Input.mousePosition.x >= (Screen.width / 2) && -shiftingBracket < player.transform.position.x)
            {
                player.transform.Translate(Vector3.left * Time.fixedDeltaTime * playerCtrl.shiftingSpeed, Space.World);
            }
        }
        //////////////////////////////////////////input controller to move player left and right ///////////////////////// this is for android
        /*
        if (Input.touchCount > 0 && !gameStarted)
        {
            gameStarted = true;
        }
        if (Input.touchCount > 0&&gameStarted)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < (Screen.width / 2) && shiftingBracket > player.transform.position.x)
                {
                    player.transform.Translate(Vector3.right * Time.fixedDeltaTime * playerCtrl.shiftingSpeed, Space.World);
                }
                if (touch.position.x >= (Screen.width / 2)&& -shiftingBracket < player.transform.position.x)
                {
                    player.transform.Translate(Vector3.left * Time.fixedDeltaTime * playerCtrl.shiftingSpeed, Space.World);
                }
            }
        }
        */
    }
    IEnumerator NextStageCoroutine(GameObject nextStagePrefab, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(activeStage);
        activeStage=Instantiate(nextStagePrefab, new Vector3(0,0, player.transform.position.z- (distanceToResp*(float)0.15* scoreMultiplier)), Quaternion.identity);
    }
    public void RestartGame()  //restarts game after death
    {
        scoreDisplay.text = "0";
        ClearScene();
        RespawnScene();
    }
    public void continueGame()  //lets you restart from you death point
    {
        if(score< deathPenalty)
        {
            score = 0;
        }
        else
        {
            score -= deathPenalty;
        }
        scoreDisplay.text = score.ToString("F0");
        ClearScene();
        SpawnFromLastStage();
    }
    public void SpawnFromLastStage()
    {
        deathPanel.SetActive(false);
        tapToStartPanel.SetActive(true);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        for (int i = (50 + (int)playerDeathPos.z); i > (-100 + (int)playerDeathPos.z); i -= 10)
        {
            lastFloor = Instantiate(FloorPrefab, new Vector3(lastFloor.transform.position.x, lastFloor.transform.position.y, i), Quaternion.identity);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        player.transform.position = new Vector3(playerStartingPos.x, playerStartingPos.y, playerDeathPos.z+20);
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        player.GetComponent<Renderer>().enabled = true;
        player.GetComponent<Collider>().enabled = true;
        playerCtrl.startTimer = false;
        playerCtrl.time = 0;
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        cameraCtrl.animationTrigger = false;
        cameraCtrl.stopMov = false;
        mCamera.transform.position = cameraCtrl.offSet;
        mCamera.transform.rotation = cameraCtrl.startingRot;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        distance = (-(lastFloor.transform.position.z)) - (-(player.transform.position.z));
        if(playerCtrl.deathSpeed> playerCtrl.startingSpeed+5)
        {
            playerCtrl.speed = playerCtrl.deathSpeed - 5;
        }
        else
        {
            playerCtrl.speed = playerCtrl.startingSpeed;
        }
        //////////////////////////////creates first obstacle prefab/////////////////////////////////////////////////
        activeStage = Instantiate(activeStage, new Vector3(0, 0, (float)playerDeathPos.z - 60), Quaternion.identity);
       // stageNumber = stageNumber-1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        dead = false;
        canClickStart = true;
        Time.timeScale = 1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    public void ClearScene()  //clearing scene after death
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        foreach (GameObject floor in floors)
        {
            Destroy(floor);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("piece");
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Destroy(activeStage);
    }
    public void RespawnScene() //respawnig a start scene
    {
        deathPanel.SetActive(false);
        tapToStartPanel.SetActive(true);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        for (int i = 10;i>-100 ;i-=10)
        {
            lastFloor = Instantiate(FloorPrefab, new Vector3(lastFloor.transform.position.x, lastFloor.transform.position.y, i), Quaternion.identity);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        player.transform.position = playerStartingPos;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        player.GetComponent<Renderer>().enabled = true;
        player.GetComponent<Collider>().enabled = true;
        playerCtrl.startTimer = false;
        playerCtrl.time = 0;
        player.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        cameraCtrl.animationTrigger = false;
        cameraCtrl.stopMov = false;
        mCamera.transform.position = cameraCtrl.offSet;
        mCamera.transform.rotation = cameraCtrl.startingRot;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        distance = (-(lastFloor.transform.position.z)) - (-(player.transform.position.z));
        playerCtrl.speed = playerCtrl.startingSpeed;
        //////////////////////////////creates first obstacle prefab/////////////////////////////////////////////////
        activeStage = Instantiate(obstaclePrefabs[1], new Vector3(0, 0, -30), Quaternion.identity);
        stageNumber = 1;
        RenderSettings.skybox = startingSkybox;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        score = 0;
        dead = false;
        canClickStart = true;
        Time.timeScale = 1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    public void NextStage() //spawnig next stage with obstacles
    {
        stageNumber++;
        if(stageNumber % 5== 0 && maxSpeed<SpeedLimit)
        {
            maxSpeed *= (float)1.15;
        }
        if(stageNumber % 20 == 0 )
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[0], intervalForNextStage));
        }
        else if (stageNumber >= 1 && stageNumber < 5)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(1,11)], intervalForNextStage));
        }
        else if (stageNumber >= 5 && stageNumber < 10)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(11, 21)], intervalForNextStage));
        }
        else if (stageNumber >= 10 && stageNumber < 15)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(21, 31)], intervalForNextStage));
        }
        else if (stageNumber >= 15 && stageNumber < 20)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(31, 41)], intervalForNextStage));
        }
        else if (stageNumber >= 20 && stageNumber < 25)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(41, 51)], intervalForNextStage));
        }
        else if (stageNumber >= 25 && stageNumber < 30)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(51, 61)], intervalForNextStage));
        }
        else if (stageNumber >= 30 && stageNumber < 35)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(61, 71)], intervalForNextStage));
        }
        else if (stageNumber >= 35 && stageNumber < 40)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(71, 81)], intervalForNextStage));
        }
        else if (stageNumber >= 40 && stageNumber < 45)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(81, 91)], intervalForNextStage));
        }
        else if (stageNumber >= 45 && stageNumber < 50)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(91, 101)], intervalForNextStage));
        }
        else if (stageNumber >= 50 && stageNumber < 55)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(101, 111)], intervalForNextStage));
        }
        else if (stageNumber >= 55 && stageNumber < 60)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(111, 121)], intervalForNextStage));
        }
        else if (stageNumber >= 60)
        {
            StartCoroutine(NextStageCoroutine(obstaclePrefabs[Random.Range(121, 151)], intervalForNextStage));
        }
    }
    void OnApplicationQuit()
    {
        if (highScore < score)
        {
            highScore = score;
            saveManagerScript.ChangeValueOfHighScoreSave(highScore);
            saveManagerScript.Save();
        }
    }
    public void Death()
    {
        if(highScore<score)
        {
            highScore = score;
            saveManagerScript.ChangeValueOfHighScoreSave(highScore);
            saveManagerScript.Save();
        }
        highScoreText.text = highScore.ToString("F0");
        playerDeathPos = player.transform.position;
        gameStarted = false;
        canClickStart = false;
        dead = true;
        deathPanel.SetActive(true);
    }
}
