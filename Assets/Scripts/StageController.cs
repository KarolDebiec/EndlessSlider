using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private GameController gameCtrl;
    void Start()
    {
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
