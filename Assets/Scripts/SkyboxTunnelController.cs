using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SkyboxTunnelController : MonoBehaviour
{
    public Material[] skyboxes;
    private int activeSkybox = 0;
    public GameObject outGate;
    private Renderer rend;
    private float time = 0;
    public float timeToOpen;
    public bool openDoor=false;
    private GameController gameCtrl;
    public GameObject[] rings;
    public float rotatorionSpeed;
    public bool isRandomSpeed;
    public float speedMin;
    public float speedMax;
    private float[] rotationSpeeds = new float[30];
    void Start()
    {
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        rend = outGate.GetComponent<Renderer>();
        rend.material.SetFloat("Vector1_B772A28E", 0);
        rend.material.SetFloat("Vector1_7F9E56E1", -(float)0.1);
        rend.material.SetFloat("Vector1_A29D21FD", 0);
        int i = 0;
        if(isRandomSpeed)
        {
            foreach (GameObject ring in rings)
            {
                rotationSpeeds[i] = Random.Range(speedMin, speedMax);
                i++;
            }
        }
        else
        {
            foreach (GameObject ring in rings)
            {
                rotationSpeeds[i] = rotatorionSpeed;
                i++;
            }
        }
    }

    public void changeSkybox()
    {
        int x = activeSkybox;
        do
        {
            x = Random.Range(0, skyboxes.Length);
        }
        while (x == activeSkybox);
        activeSkybox = x;
        int y = gameCtrl.floorType;
        RenderSettings.skybox = skyboxes[activeSkybox];
        do//changes the texture of floors
        {
            gameCtrl.floorType = Random.Range(0, 3);
        }
        while (y == gameCtrl.floorType);
    }
       
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            changeSkybox();
            rend.material.SetFloat("Vector1_A29D21FD", (float)0.04);
            openDoor = true;

        }
    }
    void Update()
    {
        int i = 0;
        foreach (GameObject ring in rings)
        {
            ring.transform.Rotate(0.0f, 0.0f, rotationSpeeds[i] * Time.deltaTime, Space.World);
            i++;
        }

        if (openDoor)
        {
            time += Time.deltaTime;
            if (time < timeToOpen)
            {
                rend.material.SetFloat("Vector1_B772A28E", Mathf.InverseLerp(0, timeToOpen, time)*(float)1.5);
                rend.material.SetFloat("Vector1_7F9E56E1", Mathf.InverseLerp(0, timeToOpen, time) * (float)0.2);
            }
        }
    }
}
