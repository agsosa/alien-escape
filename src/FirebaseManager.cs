using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Implementar remote settings

public class FirebaseManager : MonoBehaviourSingleton<FirebaseManager>
{
    public bool IsFirebaseReady = false;
    FirebaseApp FirebaseApp;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Firebase
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp = Firebase.FirebaseApp.DefaultInstance;
                IsFirebaseReady = true;

                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                IsFirebaseReady = false;
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
}
