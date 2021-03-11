using DarkTonic.MasterAudio;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviourSingleton<GameUIController>
{
    public Image TimeLeftBarBackground;
    public TextMeshProUGUI TimeWarning;
    Color TimeLeftBarBackgroundNormalColor;
    Color TimeLeftBarBackgroundNoAlphaColor;
    Color TimeWarningNormalColor;
    Color TimeWarningNoAlphaColor;

    public TextMeshProUGUI CountdownText;
    public Color CountdownText_NumberColor;
    public Color CountdownText_GoColor;

    [Header("Buttons")]
    public Button PauseBtn;
    public Image dimmed;

    public GameObject PauseMenu;
    public Button ResumeGameBtn, SettingsBtn, MainMenuBtn;

    public GameObject UseEnergyButton;
    public GameObject UseDrinkButton;
    public GameObject UseMedKitButton;
    public TextMeshProUGUI DrinkCountText;
    public TextMeshProUGUI MedKitCountText;
    public TextMeshProUGUI CollectedColaCanText;
    public TextMeshProUGUI CollectedMedKitText;

    [Header("Stats Panel")]
    public TextMeshProUGUI TimeRemaining;
    public TextMeshProUGUI SavedAliensCount;
    public Slider TimeRemainingSlider;
    public TextMeshProUGUI StolenBriefcasesCount;
    public TextMeshProUGUI Score;

    [Header("Health Panel")]
    public GameObject InvulnerableText;
    public Slider HealthBar;
    public Slider EnergyBar;
    public InvaderHealthDisplayUI _InvaderHealthDisplayUI;
    public Transform InvadersHealthDisplayParent;

    [Header("Misc")]
    public CanvasGroup BloodVignette;
    public float BloodVignette_MaxAlpha = 0.25f;
    public float BloodVignette_FadeOutSpeed = 1f;
    public float BloodVignette_ShowTime = 0.5f;
    public List<GameObject> OnGameFinishedHideObjects = new List<GameObject>();
    public FinishedGamePopup FinishedGamePopup;
    public ScoreGainUI ScoreGainUI;

    [Header("Update Rates")]
    public float UITimeLeftUpdateRate = 1f;
    public float InvaderHealthDisplayUpdateRate = 0.5f;

    // Internal variables
    float _bloodVignetteHoldTime = 0;

    #region Callbacks
    private void Start()
    {
        TimeLeftBarBackgroundNormalColor = TimeLeftBarBackground.color;
        TimeLeftBarBackgroundNoAlphaColor = new Color(TimeLeftBarBackgroundNormalColor.r, TimeLeftBarBackgroundNormalColor.g, TimeLeftBarBackgroundNormalColor.b, 0);
        TimeWarningNoAlphaColor = new Color(TimeWarningNoAlphaColor.r, TimeWarningNoAlphaColor.g, TimeWarningNoAlphaColor.b, 0);
        TimeWarningNormalColor = TimeWarning.color;
        dimmed.DOFade(1, 0);
        dimmed.gameObject.SetActive(true);

        // Set delegates
        GameSceneManager.Instance.GameState.OnTotalHealthChange += UpdateHealthEventHandler;
        GameSceneManager.Instance.GameState.OnEnergyValueUpdate += UpdateEnergyHandler;
        GameSceneManager.Instance.GameState.OnSavedAliensCountUpdate += UpdateSavedAliensUIEventHandler;
        GameSceneManager.Instance.GameState.OnStolenBriefcasesCountUpdate += UpdateStolenBriefcasesHandler;
        GameSceneManager.Instance.GameState.OnDrinkCountUpdate += UpdateDrinkUIEventHandler;
        GameSceneManager.Instance.GameState.OnMedKitsCountChange += UpdateMedKitUIEventHandler;
        GameSceneManager.Instance.OnScoreGain += OnScoreGainHandler;
        GameSceneManager.Instance.OnGameEnd += GameEndEventHandler;

        /*    public Button PauseBtn;

    public GameObject PauseMenu;
    public Button ResumeGameBtn, SettingsBtn, MainMenuBtn;*/

        // Pause menu
        PauseBtn.onClick.AddListener(() => {
            Time.timeScale = 0;
            PauseMenu.SetActive(true);
            ADManager.Instance.TryShowInterstitial();
        });
        ResumeGameBtn.onClick.AddListener(() => {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);
        });
        SettingsBtn.onClick.AddListener(() => {
            GlobalUIManager.Instance.ShowSettingsWindow();
        });
        MainMenuBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.Instance.LoadScene(SceneLoadType.MAIN_MENU);
        });
    }

    public delegate void OnCountdownFinishedDelegate();
    public async void DOInitialCountdown(OnCountdownFinishedDelegate OnFinished)
    {
        CountdownText.gameObject.SetActive(true);
        dimmed.DOFade(0, 3.5f);

        CountdownText.color = CountdownText_NumberColor;

        CountdownText.SetText("3");

        MasterAudio.PlaySound("Countdown");

        await Task.Delay(700);

        CountdownText.SetText("2");

        MasterAudio.PlaySound("Countdown");

        await Task.Delay(700);

        CountdownText.SetText("1");

        MasterAudio.PlaySound("Countdown");

        await Task.Delay(700);

        CountdownText.color = CountdownText_GoColor;
        CountdownText.SetText("GO!");
        MasterAudio.PlaySound("CountdownFinal");


        Tween t = CountdownText.DOFade(0, 1);
        t.OnComplete(() => CountdownText.gameObject.SetActive(false));

        dimmed.gameObject.SetActive(false);

        if (OnFinished != null) OnFinished();
    }

    private void OnDisable()
    {
        // Clear delegates
        GameSceneManager.Instance.GameState.OnTotalHealthChange -= UpdateHealthEventHandler;
        GameSceneManager.Instance.GameState.OnEnergyValueUpdate -= UpdateEnergyHandler;
        GameSceneManager.Instance.GameState.OnSavedAliensCountUpdate -= UpdateSavedAliensUIEventHandler;
        GameSceneManager.Instance.GameState.OnDrinkCountUpdate -= UpdateDrinkUIEventHandler;
        GameSceneManager.Instance.GameState.OnStolenBriefcasesCountUpdate -= UpdateStolenBriefcasesHandler;
        GameSceneManager.Instance.OnScoreGain -= OnScoreGainHandler;
        GameSceneManager.Instance.GameState.OnMedKitsCountChange -= UpdateMedKitUIEventHandler;
        GameSceneManager.Instance.OnGameEnd -= GameEndEventHandler;
    }

    float nextUpdateTime = 0;
    private void Update()
    {
        // Blood vignette fadeout
        if (Time.time > _bloodVignetteHoldTime)
        {
            BloodVignette.alpha = Mathf.Max(0, Mathf.Lerp(BloodVignette.alpha, 0, Time.deltaTime * BloodVignette_FadeOutSpeed));
        }

        // God mode text in health bar
        if (GameSceneManager.Instance.GameState.IsPlayerInvulnerable())
        {
            if (!InvulnerableText.activeSelf) InvulnerableText.SetActive(true);
        }
        else
        {
            if (InvulnerableText.activeSelf) InvulnerableText.SetActive(false);
        }

        // Energy button
        if (GameSceneManager.Instance.GameState.CanUseEnergy())
        {
            if (!UseEnergyButton.activeSelf)
            {
                MasterAudio.PlaySound("EnergyFull");
                UseEnergyButton.SetActive(true);
            }
        }
        else
        {
            if (UseEnergyButton.activeSelf) UseEnergyButton.SetActive(false);
        }

        if (GameSceneManager.Instance.GameState.CanDrink())
        {
            if (!UseDrinkButton.activeSelf) UseDrinkButton.SetActive(true);
        }
        else
        {
            if (UseDrinkButton.activeSelf) UseDrinkButton.SetActive(false);
        }

        if (GameSceneManager.Instance.GameState.CurrentAliveInvaders != RemoteSettings.Instance.MAX_INVADERS && GameSceneManager.Instance.GameState.CurrentMedKits > 0)
        {
            if (!UseMedKitButton.activeSelf) UseMedKitButton.SetActive(true);
        }
        else
        {
            if (UseMedKitButton.activeSelf) UseMedKitButton.SetActive(false);
        }

        float t_remaining = GameSceneManager.Instance.GameState.GetTimeRemaining();
        // Update rate
        if (Time.time > nextUpdateTime)
        {
            if (GameSceneManager.Instance.GameState.CurrentPhase == GamePhase.IN_PROGRESS)
            {
                float pct = GameUtils.GetPercentage(t_remaining, GameUtils.GetMaxSecondsPerGame(), 1); ;
                TimeRemainingSlider.value = Mathf.Max(0, pct);
                TimeRemaining.SetText(GameUtils.GetFormattedTime(t_remaining));
                if (t_remaining <= 5)
                {
                    MasterAudio.PlaySound("TimeRunningOut");
                }
            }

             nextUpdateTime += UITimeLeftUpdateRate;
        }

        if (t_remaining <= 10)
        {
            if (!TimeWarning.gameObject.activeSelf) TimeWarning.gameObject.SetActive(true);
            TimeLeftBarBackground.color = GameUtils.PingPongColors(3, TimeLeftBarBackgroundNormalColor, TimeLeftBarBackgroundNoAlphaColor);
            TimeWarning.color = GameUtils.PingPongColors(3, TimeWarningNormalColor, TimeWarningNoAlphaColor);
        }
        else
        {
            if (TimeLeftBarBackground.color != TimeLeftBarBackgroundNormalColor) TimeLeftBarBackground.color = TimeLeftBarBackgroundNormalColor;
            if(TimeLeftBarBackground.color != TimeLeftBarBackgroundNormalColor) TimeLeftBarBackground.color = TimeLeftBarBackgroundNormalColor;
            if (TimeWarning.gameObject.activeSelf) TimeWarning.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Methods
    public void RegisterInvader(InvaderController i)
    {
        InvaderHealthDisplayUI n = Instantiate(_InvaderHealthDisplayUI, InvadersHealthDisplayParent, false);
        n.Initialize(i, InvaderHealthDisplayUpdateRate);
    }
    #endregion

    #region Event Handlers

    void OnScoreGainHandler(ScoreGainType type, int score_amount, float time)
    {
        ScoreGainUI.ShowScoreGain(type, score_amount, time);
        Score.SetText(GameSceneManager.Instance.GameState.Score.ToString());
    }

    void UpdateHealthEventHandler(float old_hp, float new_hp)
    {
        HealthBar.value = GameUtils.GetPercentage(new_hp, GameSceneManager.Instance.GameState.TotalMaxHealth, 100);
        if (new_hp < old_hp && DataManager.Instance.PlayerData.SavedSettings.EnableDamageIndicator)
        {

            BloodVignette.alpha = BloodVignette_MaxAlpha;
            _bloodVignetteHoldTime = Time.time + BloodVignette_ShowTime;
        }
    }

    void UpdateEnergyHandler(float old_energy, float new_energy)
    {
       // Debug.LogError("Update old hp = " + old_hp + " new hp = " + new_hp);

        EnergyBar.value = GameUtils.GetPercentage(new_energy, 100, 100);
    }
    
    void UpdateSavedAliensUIEventHandler(int old, int new_value)
    {
        SavedAliensCount.SetText(new_value.ToString());
    }

    void UpdateDrinkUIEventHandler(int old_value, int new_value)
    {
        if (new_value > old_value && GameSceneManager.Instance.GameState.CurrentPhase == GamePhase.IN_PROGRESS) CollectedColaCanText.gameObject.SetActive(true);

        DrinkCountText.SetText(new_value.ToString());
    }

    void UpdateMedKitUIEventHandler(int old_value, int new_value)
    {
        if (new_value > old_value && GameSceneManager.Instance.GameState.CurrentPhase == GamePhase.IN_PROGRESS) CollectedMedKitText.gameObject.SetActive(true);

        MedKitCountText.SetText(new_value.ToString());
    }

    void GameEndEventHandler(bool IsBestScore, int oldBestScore, EndGameReason reason)
    {
        foreach(GameObject go in OnGameFinishedHideObjects)
        {
            go.SetActive(false);
        }
        Debug.LogError("GameEndEventHandler");
        FinishedGamePopup.Show(IsBestScore, oldBestScore, reason);
    }

    void UpdateStolenBriefcasesHandler(int old_value, int new_value)
    {
        StolenBriefcasesCount.SetText(new_value.ToString());
    }
    #endregion
}
