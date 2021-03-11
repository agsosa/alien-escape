using OPS.AntiCheat.Field;
using System.Collections.Generic;
using UnityEngine;

public enum SettingLevel
{
    LOW = 0,
    MEDIUM = 1,
    HIGH = 2,
}

public class SavedSettings
{
    public bool EnableCameraFilter = true;
    public bool EnableDamageIndicator = true;
    public bool EnableCameraShake = true;
    public bool EnableMusic = true;
    public bool EnableSounds = true;
    public float MovementSensibility = 1f;
    public float CameraSensibility = 1f;
    public bool EnableFog = true;
    public bool EnableShadows = false;
    public SettingLevel TextureQuality = SettingLevel.HIGH;
    public SettingLevel DetailsDistance = SettingLevel.HIGH;
}

public class PlayerData
{
    public bool Agreement = false;

    public ProtectedInt32 TotalBriefcases = new ProtectedInt32(0);
    public ProtectedInt32 TotalSavedAliens = new ProtectedInt32(0);
    public int BestScore = 0;

    public int UsingSkinID = 0;

    public int TotalGames = 0;
    public int TotalKilledSoldiers = 0;
    public float TotalTimePlayed = 0; // Time.time
    public int TotalGodModeActivations = 0;
    public int TotalMinesExploded = 0;
    public int TotalDeaths = 0;
    public int TotalMedKitsUsed = 0;
    public int TotalHealsUsed = 0;

    public List<int> PrematureUnlockedCharacters = new List<int>(); // Characters ID unlocked by video reward

    public ProtectedInt32 AllCharsUnlocked = new ProtectedInt32(5000); // 5000 = disabled, 10000 = enabled
    public ProtectedInt32 NoAds = new ProtectedInt32(5000); // 5000 = disabled, 10000 = enabled

    public SavedSettings SavedSettings = new SavedSettings();
}