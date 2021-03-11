using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUIManager : MonoBehaviourSingleton<GlobalUIManager>
{
    public Slider LoadingProgressbar;
    public GameObject LoadingScreen;
    public UISettings Settings;
    public TextMeshProUGUI FPSCount;
    public GameObject LowFPSTip;

    [Header("Error popup")]
    public GameObject ErrorPopup;
    public TextMeshProUGUI ErrorMsg;
    public TextMeshProUGUI ErrorCode;

    [Header("Confirmation popup")]
    public GameObject ConfirmationPopup;
    public TextMeshProUGUI ConfirmationMsg;

    AsyncOperation currLoadingOperation;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoadingScreen(bool show, AsyncOperation op)
    {
        LowFPSTip.SetActive(false);
        lowFpsStrikes = 0;
        currLoadingOperation = op;
        LoadingScreen.SetActive(show);
        LoadingProgressbar.value = 0;
    }

    public void ShowSettingsWindow()
    {
        LowFPSTip.SetActive(false);
        Settings.gameObject.SetActive(true);
    }

    public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;
    int lowFpsStrikes = 0;
   // public string formatedString = "{value}";
    void FPSCountUpdate()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;

            if (fps < 28)
            {
                lowFpsStrikes++;
            }

            if (lowFpsStrikes == 7)
            {
               // LowFPSTip.gameObject.SetActive(true);
            }

            FPSCount.SetText(System.Math.Round(fps, 1).ToString("0"));
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }
    }





    public void ShowErrorPopup(string text, int code_number)
    {
        MasterAudio.PlaySound("Error");
        ErrorMsg.SetText(text);
        ErrorCode.SetText(string.Format("Error Code: {0}", code_number.ToString()));
        ErrorPopup.SetActive(true);
    }

    public void ShowConfirmationPopup(string text)
    {
        MasterAudio.PlaySound("Confirmation");
        ConfirmationMsg.SetText(text);
        ConfirmationPopup.SetActive(true);
    }

    private void Update()
    {
        if (LoadingScreen.activeSelf)
        {
            if (currLoadingOperation != null)
            {
                LoadingProgressbar.value = currLoadingOperation.progress;
            }
        }
        if (FPSCount.gameObject.activeSelf) FPSCountUpdate();
    }
}
