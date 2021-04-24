using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; }
    public SaveState state;
    private GameController gameCtrl;
    private void Awake()
    {
        gameCtrl = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();
        gameCtrl.highScore = state.HighScore;
    }

    public void Save()
    {
       PlayerPrefs.SetString("save",SaveHelper.Serialize<SaveState>(state));
    }
    public void Load()
    {
        if(PlayerPrefs.HasKey("save"))
        {
            state = SaveHelper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("Created new save");
        }
    }
    public void ChangeValueOfHighScoreSave(float toChange)
    {
        state.HighScore = toChange;
    }
}
