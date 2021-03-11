using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// TODO: Hacer un evento para Scene ready
// TODO: Mover todo lo que se pueda a GameState

    // NOTE: EXECUTION ORDER: BELOW DEFAULT TIME!
public class GameSceneManager : MonoBehaviourSingleton<GameSceneManager>
{

    // Run time references
    [HideInInspector] public Player RewiredPlayer;

    // Variables
    public GameState GameState = new GameState();

    // Events
    public delegate void GenericGameSceneDelegate();
    public delegate void GameEndDelegate(bool IsBestScore, int oldBestScore, EndGameReason reason);
    public event GameEndDelegate OnGameEnd;
    public event GenericGameSceneDelegate OnGameStart;

    public delegate void ScoreGainedDelegate(ScoreGainType type, int score, float time);
    public event ScoreGainedDelegate OnScoreGain;

    public List<int> LockedSkinsBefore = new List<int>();

    public override void Awake()
    {
        base.Awake();

        RewiredPlayer = ReInput.players.GetPlayer(0);
    }

    private void Start()
    {
        SpawnManager.Instance.Initialize();

        LockedSkinsBefore.AddRange(SkinManager.Instance.SkinPrefabs.Where(s => GameUtils.IsSkinLocked(s)).Select(s => s.ID));

        NextEnergyRegenerationTime = Time.time + RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_REGENERATION_WAIT_SECONDS;
    }

    bool gameStartedInit = false;
    private void LateUpdate()
    {
        if (!gameStartedInit)
        {
            StartGame();
            gameStartedInit = true;
        }
    }

    public float NextEnergyRegenerationTime = 0;
    private void Update()
    {
        // Time control
        if (GameState.GetTimeRemaining() <= 0 && GameState.CurrentPhase == GamePhase.IN_PROGRESS)
        {
            EndGame(EndGameReason.OUT_OF_TIME);
        }

        // regeneration of energy over time
        // TODO: Only generate inside the base
        if (Time.time > NextEnergyRegenerationTime && GameState.CanRegenerateEnergy())
        {
            float amount = GameState.CurrentPhase == GamePhase.IN_PROGRESS ? RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_REGENERATION_AMOUNT : RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_REGENERATION_AMOUNT_BEFORE_START;
            float t = GameState.CurrentPhase == GamePhase.IN_PROGRESS ? RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_REGENERATION_WAIT_SECONDS : RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_REGENERATION_WAIT_SECONDS_BEFORE_START;
            GameState.Energy += amount;
            NextEnergyRegenerationTime = Time.time + t;
        }
    }

    public void StartGame() // TODO: CALL WHEN SCENE FULLY LOADED
    {
        Debug.LogError("StartGame");

        Skin pSkin = SkinManager.Instance.GetSkinPrefab(DataManager.Instance.PlayerData.UsingSkinID);
        GameState.CurrentAvailableDrinks = pSkin.Heals;
        GameState.CurrentMedKits = pSkin.Revives;

        GameUIController.Instance.DOInitialCountdown(() =>
        {
            GameState.TimeStarted = Time.time;
            GameState.CurrentPhase = GamePhase.IN_PROGRESS;

            if (OnGameStart != null) OnGameStart();
        });
    }

    public void EndGame(EndGameReason reason)
    {
        Debug.LogError("Ending game with reason " + reason.ToString());

        GameState.Player.Motor.StopAllMotions(true);
        GameState.CurrentPhase = GamePhase.ENDED;

        // Add playerdata and save
        bool isBestScore = false;
        PlayerData pData = DataManager.Instance.PlayerData;

        int oldBestScore = pData.BestScore;

        if (GameState.Score > pData.BestScore)
        {
            isBestScore = true;
            pData.BestScore = GameState.Score;
        }

        pData.TotalSavedAliens.Value += GameState.CurrentSavedAliens;
        pData.TotalBriefcases.Value += GameState.CurrentStolenBriefcases;
        pData.TotalGames++;
        pData.TotalTimePlayed += Time.time - GameState.TimeStarted;

        // TODO: Killed Soldiers, used heals, etc.

        DataManager.Instance.SaveData();

        if (OnGameEnd != null) OnGameEnd(isBestScore, oldBestScore, reason);
    }

    public void AddScore(ScoreGainType t)
    {
        int score = 0;
        float time = 0;

        switch(t)
        {
            case ScoreGainType.ALIEN_SAVED:
                score = RemoteSettings.Instance.SCORE_GAIN_PER_SAVED_ALIEN;
                time = RemoteSettings.Instance.TIME_GAIN_PER_SAVED_ALIEN;
                break;
            case ScoreGainType.SOLDIER_KILL:
                score = RemoteSettings.Instance.SCORE_GAIN_PER_KILLED_SOLDIER;
                time = RemoteSettings.Instance.TIME_GAIN_PER_KILLED_SOLDIER;
                break;
            case ScoreGainType.STOLEN_BRIEFCASE:
                score = RemoteSettings.Instance.SCORE_GAIN_PER_STOLEN_BRIEFCASE;
                time = RemoteSettings.Instance.TIME_GAIN_PER_STOLEN_BRIEFCASE;
                break;
        }
        
        GameState.Score += score;
        GameState.TimeStarted += time;
        if (OnScoreGain != null) OnScoreGain(t, score, time);
    }
}
