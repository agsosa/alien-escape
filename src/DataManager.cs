using OPS.AntiCheat.Prefs;
using System.Collections;
using System.Collections.Generic;
using TinyJson;
using UnityEngine;

// REMEMBER TO CALL SAVEDATA()

public class DataManager : MonoBehaviourSingleton<DataManager>
{
    public PlayerData PlayerData;
    public string key = "DNTCHT_1";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    private void OnDisable()
    {
        SaveData();        
    }

    public void SaveData()
    {
        if (PlayerData == null)
        {
            Debug.LogError("No player data found. Initializing...");
            PlayerData = new PlayerData();
        }
        //Debug.LogError("Player Data Saved = " + PlayerData.ToJson());
        ProtectedPlayerPrefs.SetString(key, PlayerData.ToJson());
    }

    public void LoadData()
    {
        Debug.LogError("Loading player data");
        string stringValue = ProtectedPlayerPrefs.GetString(key);
     //   Debug.LogError("stringValue = " + stringValue);
        if (!string.IsNullOrEmpty(stringValue))
        {
            PlayerData = stringValue.FromJson<PlayerData>();
           // Debug.LogError("Player Data Loaded = " + PlayerData.ToJson());
        }
        else
        {
            SaveData();
        }
    }
}
