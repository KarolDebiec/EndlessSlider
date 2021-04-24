using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToSize : MonoBehaviour
{
    public Vector3 startingSize;
    public Vector3 endSize;
    public float changeSizeSpeed;
    private float changeSizeProgress =0;
    private bool loop = true;
    void Start()
    {
        gameObject.transform.localScale = startingSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(loop)
        {
            if (startingSize != endSize)
            {
                changeSizeProgress +=  (Time.deltaTime * changeSizeSpeed);
                if(changeSizeProgress > 1)
                {
                    loop = false;
                    changeSizeProgress = 1;
                }
                gameObject.transform.localScale = Vector3.Lerp(startingSize, endSize, changeSizeProgress);
            }
            else
            {
                loop = false;
            }
        }
    }
}
