using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    public static System.Random RNG = new System.Random();

    public static int currAlienSittingIdleIndex = 0;

    public static float GetMaxSecondsPerGame()
    {
        return RemoteSettings.Instance.MAX_SECONDS_PER_GAME + SkinManager.Instance.GetSkinPrefab(DataManager.Instance.PlayerData.UsingSkinID).ExtraSeconds;
    }

    public static bool IsSkinLocked(Skin s)
    {
        PlayerData pData = DataManager.Instance.PlayerData;

        if (s.ID == 0) return false;
        if (pData.AllCharsUnlocked == 10000) return false;
        if (pData.PrematureUnlockedCharacters.Contains(s.ID)) return false;
        if (s.RequiredSavedAliens < pData.TotalSavedAliens) return false;
        if (s.RequiredStolenBriefcases < pData.TotalBriefcases) return false;

        return true;
    }

    public static string GetTranslatedText(string LeanLocalizationKey)
    {
        return Lean.Localization.LeanLocalization.GetTranslationText(LeanLocalizationKey);
    }

    public static string GetFormattedInteger(int number)
    {
        return number.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
    }

    public static string GetFormattedTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        return t.ToString(@"mm\:ss");
    }

    public static float PingPongMinMax(float time, float aMin, float aMax)
    {
        return Mathf.PingPong(time, aMax-aMin) + aMin;
    }

    public static float GetPercentage(float value, float max, float multiplier)
    {
        return value / max * multiplier;
    }

    public static Color PingPongColors(float time_modifier, Color color1, Color color2)
    {
        var pingPong = Mathf.PingPong(Time.time * time_modifier, 1);
        var color = Color.Lerp(color1, color2, pingPong);
        return color;
    }
}
