using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

public class MainMenuSceneManager : MonoBehaviourSingleton<MainMenuSceneManager>
{
    public Button PlayButton, SettingsButton, GiveRateButton, AutomaticallyLowerGraphics, ConsumeAllIAPBtn;
    public Transform MainMenuCameraTransform;
    public TextMeshProUGUI TotalSavedAliensCount, TotalStolenBriefcaseCount;
    public SkinSelector SkinSelector;

    private void Start()
    {
        ConsumeAllIAPBtn.onClick.AddListener(() =>
        {
            foreach (Product p in CodelessIAPStoreListener.Instance.StoreController.products.all)
            {
                CodelessIAPStoreListener.Instance.StoreController.ConfirmPendingPurchase(p);
            }

            GlobalUIManager.Instance.ShowConfirmationPopup("Consumed all products");
        });

        GiveRateButton.onClick.AddListener(() =>
        {
    #if UNITY_ANDROID
            Application.OpenURL("market://details?id=games.battlemark.alien_escape_3d.storm.area51");
#elif UNITY_IPHONE
          //  Application.OpenURL("itms-apps://itunes.apple.com/app/idgames.battlemark.alien_escape_3d.storm.area51");
#endif
        });

        PlayButton.onClick.AddListener(() => {
            if (DataManager.Instance.PlayerData.TotalGames > 0)
            {
                ADManager.Instance.TryShowInterstitial();
            }
            SceneManager.Instance.LoadScene(SceneLoadType.GAME);
         });
        SettingsButton.onClick.AddListener(() => GlobalUIManager.Instance.ShowSettingsWindow());
        TotalSavedAliensCount.SetText(GameUtils.GetFormattedInteger(DataManager.Instance.PlayerData.TotalSavedAliens.Value));
        TotalStolenBriefcaseCount.SetText(GameUtils.GetFormattedInteger(DataManager.Instance.PlayerData.TotalBriefcases.Value));
    }

    bool CamTransformSet = false;
    float NextUpdate = 0;
    private void Update()
    {
        if (!CamTransformSet && Time.time > NextUpdate)
        {
            if (Camera.main != null)
            {
                Camera.main.transform.position = MainMenuCameraTransform.position;
                Camera.main.transform.rotation = MainMenuCameraTransform.rotation;
                CamTransformSet = true;
            }
            NextUpdate = Time.time + 0.1f;
        }
    }
}
