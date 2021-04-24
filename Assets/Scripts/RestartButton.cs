using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    private GameController gameCtrl;
    void Start()
    {
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    public void restartClick()
    {
        gameCtrl.RestartGame();
    }
    public void continueClick()
    {
        gameCtrl.continueGame();
    }
}
