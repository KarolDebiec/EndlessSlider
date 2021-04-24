using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private GameObject player;
    private Renderer rend;
    private GameController gameCtrl;
    public float offset = 30;
    public Texture[] floorTextures;
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        player = GameObject.FindWithTag("Player");
        rend.material.SetTexture("Texture2D_5B08C417", floorTextures[gameCtrl.floorType]);
    }
    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.z<gameObject.transform.position.z + (-offset))
        {
            Destroy(gameObject);
        }
    }
}
