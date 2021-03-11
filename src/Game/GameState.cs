using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Only fire events after gamephase starting?

    /*
     * SOLO PROPIEDADES Y METODOS BOOL
     * 
     */

[System.Serializable]
public class GameState
{
    public delegate void OnStateUpdateDelegate<T>(T old_value, T new_value);
    public delegate void StateGenericDelegate();

    public GamePhase CurrentPhase = GamePhase.STARTING;

    public float TimeStarted = 0;

    public float GetTimeRemaining()
    {
        float mTime = TimeStarted + GameUtils.GetMaxSecondsPerGame();
        return Time.time >= mTime ? 0 : (mTime - Time.time);
    }

    public PlayerController Player;
    public List<InvaderController> Invaders = new List<InvaderController>();
    public List<AlienController> Aliens = new List<AlienController>();

    // Drink variable
    public bool Drinking = false;
    public event OnStateUpdateDelegate<int> OnDrinkCountUpdate;
    [SerializeField] private int _currentAvailableDrinks = 0;
    public int CurrentAvailableDrinks
    {
        get { return _currentAvailableDrinks; }
        set
        {
            if (_currentAvailableDrinks == value) return;
            if (OnDrinkCountUpdate != null) OnDrinkCountUpdate(_currentAvailableDrinks, value); // Event
            _currentAvailableDrinks = value;
        }
    }

    public event StateGenericDelegate OnPlayerDrink;
    public void InvokeOnPlayerDrink()
    {
        if (OnPlayerDrink != null) OnPlayerDrink();
    }

    public bool CanDrink()
    {
        if (CurrentAvailableDrinks <= 0)
        {
            return false;
        }

        if (Drinking) return false;

        bool Herido = Player.Health < 100;
        if (!Herido) // check for invaders
        {
            foreach(InvaderController i in Invaders)
            {
                if (!i.IsDead)
                {
                    if (i.Health < 100)
                    {
                        Herido = true;
                        break;
                    }
                }
            }
        }

        return Herido;
    }

    // Med kits variable
    public event OnStateUpdateDelegate<int> OnMedKitsCountChange;
    [SerializeField] private int _CurrentMedKits = 0;
    public int CurrentMedKits
    {
        get { return _CurrentMedKits; }
        set
        {
            if (_CurrentMedKits == value) return;
            if (OnMedKitsCountChange != null) OnMedKitsCountChange(_CurrentMedKits, value); // Event
            _CurrentMedKits = value;
        }
    }

    // Alive invaders variable
    public event OnStateUpdateDelegate<int> OnInvaderAliveCountUpdate;
    [SerializeField] private int _CurrentAliveInvaders = 0;
    public int CurrentAliveInvaders
    {
        get { return _CurrentAliveInvaders; }
        set
        {
            if (_CurrentAliveInvaders == value) return;
            if (OnInvaderAliveCountUpdate != null) OnInvaderAliveCountUpdate(_CurrentAliveInvaders, value); // Event
            _CurrentAliveInvaders = value;
        }
    }

    // Score variable
    public event OnStateUpdateDelegate<int> OnScoreChange;
    [SerializeField] private int _Score = 0;
    public int Score // Use gamescenemanager AddScore to assign
    {
        get { return _Score; }
        set
        {
            if (_Score == value) return;
            if (OnScoreChange != null) OnScoreChange(_Score, value); // Event
            _Score = value;
        }
    }

    // Variable to save the sum of all invaders and player health
    public float TotalMaxHealth = 0;

    // Current total health variable
    public event OnStateUpdateDelegate<float> OnTotalHealthChange;
    float LastTotalHealthUpdate = 0;
    public void InvokeOnHealthChange()
    {
        if (OnTotalHealthChange != null)
        {
            OnTotalHealthChange(LastTotalHealthUpdate, CurrentTotalHealth);
        }
        LastTotalHealthUpdate = CurrentTotalHealth;
    }
    public float CurrentTotalHealth
    {
        get { return Player.Health + Invaders.Sum(i => i.Health); }
    }

    // Current saved aliens
    public event OnStateUpdateDelegate<int> OnSavedAliensCountUpdate;
    [SerializeField] private int _CurrentTotalSavedAliens = 0;
    public int CurrentSavedAliens
    {
        get { return _CurrentTotalSavedAliens; }
        set
        {
            if (_CurrentTotalSavedAliens == value) return;
            if (OnSavedAliensCountUpdate != null) OnSavedAliensCountUpdate(_CurrentTotalSavedAliens, value); // Event
            _CurrentTotalSavedAliens = value;
        }
    }

    //CurrentKillStreak
    public event OnStateUpdateDelegate<int> OnCurrentKillStreakUpdate;
    [SerializeField] private int _CurrentSkillStreak = 0;
    public int CurrentKillStreak
    {
        get { return _CurrentSkillStreak; }
        set
        {
            if (_CurrentSkillStreak == value) return;
            if (OnCurrentKillStreakUpdate != null) OnCurrentKillStreakUpdate(_CurrentSkillStreak, value); // Event
            _CurrentSkillStreak = value;
        }
    }


    //CurrentStolenBriefcases
    public event OnStateUpdateDelegate<int> OnStolenBriefcasesCountUpdate;
    [SerializeField] private int _CurrentStolenBriefcases = 0;
    public int CurrentStolenBriefcases
    {
        get { return _CurrentStolenBriefcases; }
        set
        {
            if (_CurrentStolenBriefcases == value) return;
            if (OnStolenBriefcasesCountUpdate != null) OnStolenBriefcasesCountUpdate(_CurrentStolenBriefcases, value); // Event
            _CurrentStolenBriefcases = value;
        }
    }

    // Invulnerability time expiration
    public event OnStateUpdateDelegate<float> OnInvulnerabilityEndTimeUpdate;
    [SerializeField] private float _InvulnerabilityEndTime = 0;
    public float InvulnerabilityEndTime
    {
        get { return _InvulnerabilityEndTime; }
        set
        {
            if (_InvulnerabilityEndTime == value) return;
            if (OnInvulnerabilityEndTimeUpdate != null) OnInvulnerabilityEndTimeUpdate(_InvulnerabilityEndTime, value); // Event
            _InvulnerabilityEndTime = value;
        }
    }

    public bool IsPlayerInvulnerable()
    {
        return Time.time < InvulnerabilityEndTime || (RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_INVULNERABILITY ? IsEnergyStateActive : true);
    }

    // Fury energy
    public event OnStateUpdateDelegate<float> OnEnergyValueUpdate;
    [SerializeField] private float _energy = 0;
    public float Energy
    {
        get { return _energy; }
        set
        {
            if (_energy == value) return;
            if (OnEnergyValueUpdate != null)
            {
          //      Debug.LogError("Calling on energy value update");
                OnEnergyValueUpdate(_energy, value); // Event
            }

            _energy = Mathf.Min(value, 100);
        }
    }

    public bool CanRegenerateEnergy()
    {
        return Energy < 100 && !IsEnergyStateActive;
    }

    public bool CanUseEnergy()
    {
        return Energy >= 100 && !IsEnergyStateActive && !Drinking;
    }

    [SerializeField] private bool _EnergyStateActive = false;
    public bool IsEnergyStateActive
    {
        get { return _EnergyStateActive; }
        set
        {
            if (_EnergyStateActive == value) return;
            if (value && OnEnergyStateActivate != null) OnEnergyStateActivate();
            if (!value && OnEnergyStateDeactivate != null) OnEnergyStateDeactivate();
            _EnergyStateActive = value;
        }
    }

    public event StateGenericDelegate OnEnergyStateDeactivate;
    public event StateGenericDelegate OnEnergyStateActivate;
}
