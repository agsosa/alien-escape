using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviourSingleton<EnvironmentController>
{
    public Transform AlienSaveSpotCenter;
    public Transform UFOEntrance;

    public delegate void OnEnvironmentExplosionDelegate(Transform origin, EnvironmentExplosiveType type);
    public event OnEnvironmentExplosionDelegate OnEnvironmentExplosion;
    public void InvokeEnvironmentExplosionEvent(Transform origin, EnvironmentExplosiveType type)
    {
        if (OnEnvironmentExplosion != null) OnEnvironmentExplosion(origin, type);
    }
}
