using DG.Tweening;
using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourSingleton<CameraController>
{
    public ParticleSystem SpeedEffect;
    public Color SpeedEffectNormalColor;
    public Color SpeedEffectEnergyActiveColor;
    public float SpeedEffectNormalEmissionRate;
    public float SpeedEffectEnergyActiveEmissionRate;

    public ParticleSystem AirParticles;

    bool SpeedEffectPlaying = false;

    public Tweener ShakeTween;

    private void Start()
    {
        SetSpeedEffectState(false);
        EnergyDeactivatedHandler();
    }

    private void OnEnable()
    {
        SceneManager.Instance.OnSceneLoaded += _OnSceneLoadedHandler;
    }

    private void OnDisable()
    {
        SceneManager.Instance.OnSceneLoaded -= _OnSceneLoadedHandler;
    }

    void _OnSceneLoadedHandler(SceneLoadType loaded)
    {
        try
        {
            GameSceneManager.Instance.GameState.OnEnergyStateActivate -= EnergyActivatedHandler;
            GameSceneManager.Instance.GameState.OnEnergyStateDeactivate -= EnergyDeactivatedHandler;
            EnvironmentController.Instance.OnEnvironmentExplosion -= EnvironmentExplosionHandler;
        }
        catch
        {

        }

        if (loaded == SceneLoadType.GAME)
        {
            GameSceneManager.Instance.GameState.OnEnergyStateActivate += EnergyActivatedHandler;
            GameSceneManager.Instance.GameState.OnEnergyStateDeactivate += EnergyDeactivatedHandler;
            EnvironmentController.Instance.OnEnvironmentExplosion += EnvironmentExplosionHandler;
        }
    }

    private void Update()
    {
        if (GameSceneManager.Instance != null)
        {
            vThirdPersonController motor = GameSceneManager.Instance.GameState.Player.Motor;
            if (motor.ActuallySprintingAndMoving() || GameSceneManager.Instance.GameState.IsEnergyStateActive) // TODO: Cambiar a animator check velocity
            {
                if (!IsSpeedEffectActive()) SetSpeedEffectState(true);
            }
            else
            {
                if (IsSpeedEffectActive()) SetSpeedEffectState(false);
            }
        }
    }

    public void SetSpeedEffectState(bool show)
    {
        if (show)
        {
            SpeedEffectPlaying = true;
            SpeedEffect.Play();
        }
        else
        {
            SpeedEffectPlaying = false;
            SpeedEffect.Stop();
        }
    }

    public bool IsSpeedEffectActive()
    {
        return SpeedEffectPlaying;
    }

    public void EnergyActivatedHandler()
    {
        var m = SpeedEffect.main;
        var e = SpeedEffect.emission;
        m.startColor = SpeedEffectEnergyActiveColor;
        e.rateOverTime = SpeedEffectEnergyActiveEmissionRate;
    }

    public void EnergyDeactivatedHandler()
    {
        var m = SpeedEffect.main;
        var e = SpeedEffect.emission;
        m.startColor = SpeedEffectNormalColor;
        e.rateOverTime = SpeedEffectNormalEmissionRate;
    }

    void EnvironmentExplosionHandler(Transform origin, EnvironmentExplosiveType type)
    {
        float str = type == EnvironmentExplosiveType.MINE ? 5.35f : 7;
        // ShakeTween = vThirdPersonCamera.instance._camera.DOShakeRotation(0.25f, str, 20);
        ShakeCamera(str);
    }

    public void ShakeCamera(float strength)
    {
        if (DataManager.Instance.PlayerData.SavedSettings.EnableCameraShake)
        {
            ShakeTween = vThirdPersonCamera.instance.targetLookAt.DOShakeRotation(0.45f, strength, 20);
        }
    }

    public void StopShaking()
    {
        if (ShakeTween != null)
        {
         //   Debug.LogError("Stoping shake");
            ShakeTween.Kill();
        }
    }
}
