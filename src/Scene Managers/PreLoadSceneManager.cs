using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// TODO: Pasar todo lo de agreement a globaluiscenemanager

public class PreLoadSceneManager : MonoBehaviourSingleton<PreLoadSceneManager>
{
    public static bool IsTest = false;

    public string TermsUrl = "http://bit.ly/2kkgWFA";
    public string PrivacyUrl = "http://bit.ly/2lTUFPr";

    public GameObject AgreementUI;
    public Button AgreeBtn, TermsBtn, PrivacyBtn;

    public override void Awake()
    {
        base.Awake();
        UnityThreadHelper.initUnityThread();
        AgreementUI.SetActive(false);
    }

    async void Start()
    {
        Debug.unityLogger.logEnabled = IsTest;
        Application.targetFrameRate = 60;

        if (DataManager.Instance.PlayerData.Agreement)
        {
            Destroy(AgreementUI.gameObject);
            DOFirstLoad();
        }
        else
        {
            await Task.Delay(200);
            AgreeBtn.onClick.AddListener(() =>
            {
                DataManager.Instance.PlayerData.Agreement = true;
                DataManager.Instance.SaveData();
                AgreementUI.SetActive(false);
                DOFirstLoad();
            });
            TermsBtn.onClick.AddListener(() => {
                Application.OpenURL(TermsUrl);
            });
            PrivacyBtn.onClick.AddListener(() => Application.OpenURL(PrivacyUrl));
            AgreementUI.SetActive(true);
        }

    }

    void DOFirstLoad()
    {
        ADManager.Instance.Initialize();
        SceneManager.Instance.LoadScene(SceneLoadType.FIRST_LOAD);
    }
}
