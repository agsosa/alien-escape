using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

// Falta musica, sonido, sensibility sliders

public class UISettings : MonoBehaviour
{
    public delegate void OnSettingsUpdatedDelegate();
    public event OnSettingsUpdatedDelegate OnSettingsUpdated;

    public Button ApplySettingsBtn;
    public Button ResetSettingsBtn;
    public Button PotatoModeBtn;

    public Slider MovementSensibilitySlider;
    public Slider CamSensibilitySlider;
    public Toggle EnableMusicToggle;
    public Toggle EnableSoundsToggle;
    public TMP_Dropdown LanguageDropdown;
    public TMP_Dropdown TextureQualityDropdown;
    public TMP_Dropdown DetailsDistanceDropdown;
    public Toggle EnableCameraFilterToggle;
    public Toggle DamageIndicatorToggle;
    public Toggle CameraShakeToggle;
    public Toggle ShadowsToggle;
    public Toggle FogToggle;

    private void Start()
    {
        SceneManager.Instance.OnSceneLoaded += SceneLoadHandle;
        PotatoModeBtn.onClick.AddListener(() => PotatoMode());
        ApplySettingsBtn.onClick.AddListener(() =>
        {
            SaveSettings();
            ApplySettings();
            gameObject.SetActive(false);
        });
        ResetSettingsBtn.onClick.AddListener(() => ResetDefault());

        gameObject.SetActive(false);
    }

    void SceneLoadHandle(SceneLoadType load)
    {
        if (load == SceneLoadType.FIRST_LOAD)
        {
            Debug.LogError("Initializing Settings due to FIRST_LOAD done");
            RefreshUI();
            ApplySettings();
        }

        if (load == SceneLoadType.GAME)
        {
            ApplySensitivity();
        }
    }

    void ApplySensitivity()
    {
       // Rewired.ReInput.mapping.GetInputBehavior(0, 0).customControllerAxisSensitivity = 1 * DataManager.Instance.PlayerData.SavedSettings.MovementSensibility;
    }

    private void OnDisable()
    {
        
    }

    public void RefreshUI() // Set components to SavedSettings data
    {
        Debug.LogError("Refreshing settings UI");

        SavedSettings SavedSettings = DataManager.Instance.PlayerData.SavedSettings;

        FogToggle.isOn = SavedSettings.EnableFog;
        ShadowsToggle.isOn = SavedSettings.EnableShadows;
        CameraShakeToggle.isOn = SavedSettings.EnableCameraShake;
        DamageIndicatorToggle.isOn = SavedSettings.EnableDamageIndicator;
        EnableCameraFilterToggle.isOn = SavedSettings.EnableCameraFilter;
        EnableMusicToggle.isOn = SavedSettings.EnableMusic;
        EnableSoundsToggle.isOn = SavedSettings.EnableSounds;

        CamSensibilitySlider.value = SavedSettings.CameraSensibility;
        MovementSensibilitySlider.value = SavedSettings.MovementSensibility;

        // TODO: Add new languages
        LanguageDropdown.value = Lean.Localization.LeanLocalization.CurrentLanguage == "English" ? 0 : 1;
        TextureQualityDropdown.value = (int)SavedSettings.TextureQuality;
        DetailsDistanceDropdown.value = (int)SavedSettings.DetailsDistance;
    }

    public void SaveSettings() // Take values from UI and save to prefs
    {
        Debug.LogError("Saving settings");

        SavedSettings SavedSettings = DataManager.Instance.PlayerData.SavedSettings;

        SavedSettings.EnableFog = FogToggle.isOn;
        SavedSettings.EnableShadows = ShadowsToggle.isOn;
        SavedSettings.EnableCameraShake = CameraShakeToggle.isOn;
        SavedSettings.EnableDamageIndicator = DamageIndicatorToggle.isOn;
        SavedSettings.EnableCameraFilter = EnableCameraFilterToggle.isOn;
        SavedSettings.EnableMusic = EnableMusicToggle.isOn; // TODO: FALTA APPLY SETTINGS
        SavedSettings.EnableSounds = EnableSoundsToggle.isOn;

        SavedSettings.CameraSensibility = CamSensibilitySlider.value; // TODO: FALTA APPLY SETTINGS
        SavedSettings.MovementSensibility = MovementSensibilitySlider.value; // TODO: FALTA APPLY SETTINGS

        Lean.Localization.LeanLocalization.CurrentLanguage = LanguageDropdown.options[LanguageDropdown.value].text;
        SavedSettings.TextureQuality = (SettingLevel)TextureQualityDropdown.value;
        SavedSettings.DetailsDistance = (SettingLevel)DetailsDistanceDropdown.value;

        DataManager.Instance.SaveData();

        if (OnSettingsUpdated != null) OnSettingsUpdated();
    }

    public void ApplySettings() // Take values from prefs and apply to game
    {
        Debug.LogError("Applying settings");

        SavedSettings SavedSettings = DataManager.Instance.PlayerData.SavedSettings;

        if (GameSceneManager.Instance != null)
        {
            ApplySensitivity();
        }

        // Sound & music
        if (!SavedSettings.EnableMusic) MasterAudio.MuteAllPlaylists(); else MasterAudio.UnmuteAllPlaylists();
        MasterAudio.MixerMuted = !SavedSettings.EnableSounds;

        // Shadows
        QualitySettings.shadows = SavedSettings.EnableShadows ? ShadowQuality.HardOnly : ShadowQuality.Disable;

        // texture quality
        QualitySettings.masterTextureLimit = Mathf.Abs((int)SavedSettings.TextureQuality - 2);

        // lod bias
        QualitySettings.lodBias = SavedSettings.DetailsDistance == SettingLevel.HIGH ? 1 : 0.8f;

        // camera
        CameraController cam = CameraController.Instance;
        cam.GetComponent<Camera>().farClipPlane = SavedSettings.DetailsDistance == SettingLevel.LOW ? 85 : 1000;
        cam.GetComponent<MobileColorGrading>().enabled = SavedSettings.EnableCameraFilter;
        cam.AirParticles.gameObject.SetActive(SavedSettings.EnableCameraFilter);

        // FOG
        RenderSettings.fog = SavedSettings.EnableFog;
    }

    public void ResetDefault()
    {
        FogToggle.isOn = true;
        ShadowsToggle.isOn = false;
        CameraShakeToggle.isOn = true;
        DamageIndicatorToggle.isOn = true;
        EnableCameraFilterToggle.isOn = true;
        EnableMusicToggle.isOn = true;
        EnableSoundsToggle.isOn = true;
        CamSensibilitySlider.value = 1;
        MovementSensibilitySlider.value = 1;
        TextureQualityDropdown.value = (int)SettingLevel.HIGH;
        DetailsDistanceDropdown.value = (int)SettingLevel.HIGH;
    }

    public void PotatoMode()
    {
        FogToggle.isOn = false;
        ShadowsToggle.isOn = false;
        EnableCameraFilterToggle.isOn = false;
        TextureQualityDropdown.value = (int)SettingLevel.LOW;
        DetailsDistanceDropdown.value = (int)SettingLevel.MEDIUM;
    }
}
