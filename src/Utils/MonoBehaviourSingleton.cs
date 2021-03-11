using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * TO ADD A DontDestroyOnLoad in Awake, just write the Awake method in the child and call base.Awake();
 * 
 */

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    #region SINGLETON
    public static T Instance { get; protected set; }

    public virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Duplicated singleton instance detected! Removing the latest instance.");
            Destroy(this);
        }
        else
        {
            Debug.Log("Initializing Singleton");
            Instance = (T)this;
        }
    }
    #endregion
}