using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleWarpIn : MonoBehaviour
{
    public float warp = 100;
    public float speed = 100;
    public bool startTimer;
    private Renderer rend;
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.material.SetFloat("Vector1_1A7BC3A0", warp);
        startTimer = true;
    }

    void Update()
    {
        if (startTimer)
        {
            warp -= Time.deltaTime * speed;
            rend.material.SetFloat("Vector1_1A7BC3A0", warp);
            if (warp <= 1)
            {
                rend.material.SetFloat("Vector1_1A7BC3A0", 1);
                startTimer = false;
            }    
        }
    }
}
