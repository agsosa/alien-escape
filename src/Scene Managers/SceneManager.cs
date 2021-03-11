using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    public delegate void OnSceneLoadedDelegate(SceneLoadType loadedtype);
    public event OnSceneLoadedDelegate OnSceneLoaded;

    public bool IsLoadingScene = false;
    public bool IsGameScene { get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SCENE_GAME_STR; } }
    public bool IsMainMenuScene { get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SCENE_MAIN_MENU_STR; } }

    static readonly string SCENE_MAIN_MENU_STR = "MainMenuScene";
    static readonly string SCENE_GAME_STR = "GameScene";
    static readonly string SCENE_MAP_STR = "MapScene";
    static readonly string SCENE_PRELOAD_STR = "PreLoadScene";

    IEnumerator SceneLoadCoroutine;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(SceneLoadType type)
    {
        if (IsLoadingScene)
        {
            Debug.LogError("FATAL ERROR: A SCENE IS ALREADY LOADING!");
            return;
        }

        IsLoadingScene = true;

        Debug.Log("Loading scene with type = " + type.ToString());

        switch(type)
        {
            case SceneLoadType.FIRST_LOAD:
                SceneLoadCoroutine = FirstLoadScenesCoroutine();
                break;
            case SceneLoadType.MAIN_MENU:
                SceneLoadCoroutine = LoadMainMenuSceneCoroutine();
                break;
            case SceneLoadType.GAME:
                SceneLoadCoroutine = LoadGameSceneCoroutine();
                break;
            default:
                Debug.LogError("FATAL ERROR: SCENE TYPE IS NOT VALID!");
                return;
        }

        StartCoroutine(SceneLoadCoroutine);
    }

    IEnumerator FirstLoadScenesCoroutine()
    {
        GlobalUIManager.Instance.SetLoadingScreen(true, null);

        Debug.Log("Loading Map Scene");

        var b = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SCENE_MAIN_MENU_STR, LoadSceneMode.Additive);

        var m = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SCENE_MAP_STR, LoadSceneMode.Additive);

        GlobalUIManager.Instance.SetLoadingScreen(true, m);

        Debug.Log("Loading MAIN MENU SCENE");

        yield return new WaitUntil(() => m.isDone && b.isDone);

        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(SCENE_MAP_STR));

        Debug.Log("Main Menu scene loaded");

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SCENE_PRELOAD_STR);

        MasterAudio.StartPlaylist("Main Songs");

        GlobalUIManager.Instance.SetLoadingScreen(false, null);
        IsLoadingScene = false;

        if (DataManager.Instance.PlayerData.TotalGames <= 0)
        {
            GlobalUIManager.Instance.ShowConfirmationPopup(GameUtils.GetTranslatedText("LowFPSTip2"));
        }

        if (OnSceneLoaded != null) OnSceneLoaded(SceneLoadType.FIRST_LOAD);

    }

    IEnumerator LoadMainMenuSceneCoroutine()
    {
        GlobalUIManager.Instance.SetLoadingScreen(true, null);

        Debug.Log("Unloading Game scene");

        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(SCENE_GAME_STR).isLoaded)
        {
            var b = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SCENE_GAME_STR);
            yield return new WaitUntil(() => b.isDone);
        }

        Debug.Log("Loading MAIN MENU SCENE");

        var a = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SCENE_MAIN_MENU_STR, LoadSceneMode.Additive);

        GlobalUIManager.Instance.SetLoadingScreen(true, a);

        yield return new WaitUntil(() => a.isDone);
        Debug.Log("Main Menu scene loaded");

        GlobalUIManager.Instance.SetLoadingScreen(false, null);
        IsLoadingScene = false;

        if (OnSceneLoaded != null) OnSceneLoaded(SceneLoadType.MAIN_MENU);
    }

    IEnumerator LoadGameSceneCoroutine()
    {
        GlobalUIManager.Instance.SetLoadingScreen(true, null);

        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(SCENE_MAIN_MENU_STR).isLoaded)
        {
            Debug.Log("Unloading Main menu scene");
            var b = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SCENE_MAIN_MENU_STR);
            yield return new WaitUntil(() => b.isDone);
        }

        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(SCENE_GAME_STR).isLoaded)
        {
            Debug.Log("Unloading game scene");
            var b = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SCENE_GAME_STR);
            yield return new WaitUntil(() => b.isDone);
        }

        Debug.Log("Loading GAME SCENE");

        var a = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SCENE_GAME_STR, LoadSceneMode.Additive);

        GlobalUIManager.Instance.SetLoadingScreen(true, a);

        yield return new WaitUntil(() => a.isDone);

        Debug.Log("Game scene loaded");

        GlobalUIManager.Instance.SetLoadingScreen(false, null);

        IsLoadingScene = false;

        if (OnSceneLoaded != null) OnSceneLoaded(SceneLoadType.GAME);
    }
}
