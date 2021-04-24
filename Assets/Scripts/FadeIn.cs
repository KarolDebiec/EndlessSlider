using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeIn : MonoBehaviour
{
    public float MaxFade = 1;
    private float fade = 0;
    private float time;
    public float timeToCount;
    public bool startTimer;
    void Start()
    {
        time = 0;
        fade = 0;
    }

    void Update()
    {
        if (startTimer)
        {
            time += Time.deltaTime;
            fade = MaxFade * (time/ timeToCount);
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(fade);
            if(fade>= MaxFade)
            {
                startTimer = false;
                time = 0;
                fade = 0;
            }
        }
    }
    void OnEnable()
    {
        startTimer = true;
    }
}
