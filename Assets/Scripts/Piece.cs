using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private Renderer rend;
    private float time = 0;
    public float timeToCount;
    public bool stopTimer;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    
    void Update()
    {
        if(!stopTimer)
        {
            time += Time.deltaTime;
            if (time < timeToCount)
            {
                rend.material.SetFloat("Vector1_E8FB88F7", Mathf.InverseLerp(0, timeToCount, time));
            }
            else
            {
                stopTimer = false;
                Destroy(gameObject);
            }
        }
    }
}
