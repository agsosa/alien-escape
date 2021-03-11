using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TranslateText : MonoBehaviour
{
    public LocalizatedCaps Caps = LocalizatedCaps.UPPERCASE; 
    public string LeanLocalizatorKey = "";

    TextMeshProUGUI TMP;
    Text TXT;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        TXT = GetComponent<Text>();
    }

    private void OnEnable()
    {
        if (GlobalUIManager.Instance != null)
        {
            GlobalUIManager.Instance.Settings.OnSettingsUpdated += DOTranslate;
        }
        DOTranslate();
    }

    private void OnDisable()
    {
        if (GlobalUIManager.Instance != null)
        {
            try
            {
                GlobalUIManager.Instance.Settings.OnSettingsUpdated -= DOTranslate;
            }
            catch (Exception ex)
            {
                Debug.LogError("Occured exception " + ex.Message + " while trying to unsuscribe a TranslateText from OnSettingsUpdate");
            }
        }
    }

    void DOTranslate()
    {
        string local = GameUtils.GetTranslatedText(LeanLocalizatorKey);

        if (string.IsNullOrEmpty(local))
        {
            return;
        }

        if (Caps == LocalizatedCaps.LOWERCASE) local = local.ToLower();
        if (Caps == LocalizatedCaps.UPPERCASE) local = local.ToUpper();

        if (TMP != null)
        {
            TMP.SetText(local);
        }

        if (TXT != null)
        {
            TXT.text = local;
            
        }
    }
}
