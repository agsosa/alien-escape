using DarkTonic.MasterAudio;
using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// TODO: Mover a otro script las acciones (no overrides) para onplayerdrink y esas cosas

public class PlayerController : PlayerCharacter
{
    public ParticleSystem EnergyParticles;
    public ParticleSystem EnergyActivationParticles;
    public ParticleSystem HitSoldierParticles;

    public ParticleSystem HeartParticles;
    [HideInInspector] public vThirdPersonController Motor;

    public override bool ActuallySprintingAndMoving()
    {
        return Motor.ActuallySprintingAndMoving();
    }

    private void Awake()
    {
        Motor = GetComponent<vThirdPersonController>();
        GM = GameSceneManager.Instance;
    }

    public override void Start()
    {
        base.Start();
        AssignSkin(SkinManager.Instance.GetSkinPrefab(DataManager.Instance.PlayerData.UsingSkinID));
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GM.GameState.OnInvaderAliveCountUpdate += OnInvaderAliveCountUpdateEventHandler;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        GM.GameState.OnInvaderAliveCountUpdate -= OnInvaderAliveCountUpdateEventHandler;
    }

    void OnInvaderAliveCountUpdateEventHandler(int old, int n) // Change tag of player so soldiers can target him
    {
        // Check if all invaders are dead
        if (n <= 0 && GM.GameState.CurrentPhase == GamePhase.IN_PROGRESS)
        {
            gameObject.tag = "Invader";
        }
    }

    public override void DoDeath()
    {
        base.DoDeath();
        Motor.StopAllMotions(true);
        GM.EndGame(EndGameReason.PLAYER_DIED);
    }

    public void ReviveInvader()
    {
        if (GameSceneManager.Instance.GameState.CurrentMedKits <= 0) return;
        if (GameSceneManager.Instance.GameState.CurrentAliveInvaders == RemoteSettings.Instance.MAX_INVADERS) return;

        foreach(InvaderController i in GameSceneManager.Instance.GameState.Invaders)
        {
            if (i.IsDead)
            {
                gameObject.tag = "Player";
                MasterAudio.PlaySound("MedKit");
                HeartParticles.Play();
                GameSceneManager.Instance.GameState.CurrentMedKits--;
                i.Revive();
                break;
            }
        }
    }

    #region Drink system
    public override void DoDrinkAnim()
    {
        base.DoDrinkAnim();
        HeartParticles.Play();
    }

    // TODO: Mover a coroutine por problemas de performance
    public async void TryUseDrinkAsync()
    {
        if (!GM.GameState.CanDrink()) return;

        //Debug.LogError("Drinking...");

        // Update state
        GM.GameState.Drinking = true;
        GM.GameState.CurrentAvailableDrinks--;

        // Freeze
        Motor.SetMovementFreeze(true);

        MasterAudio.PlaySound3DAtTransformAndForget("Drinking", transform);

        // Call event
        GM.GameState.InvokeOnPlayerDrink(); // Call event

        GM.GameState.InvulnerabilityEndTime = Time.time + RemoteSettings.Instance.PLAYER_CHARACTER_HEAL_INVULNERABILITY_TIME;

        // Heal player
        float amount = Random.Range(RemoteSettings.Instance.PLAYER_CHARACTER_MIN_HEAL_AMOUNT, RemoteSettings.Instance.PLAYER_CHARACTER_MAX_HEAL_AMOUNT);
        GM.GameState.Player.Heal(amount);
        // Heal invaders
        foreach (InvaderController i in GM.GameState.Invaders)
        {
            amount = Random.Range(RemoteSettings.Instance.PLAYER_CHARACTER_MIN_HEAL_AMOUNT, RemoteSettings.Instance.PLAYER_CHARACTER_MAX_HEAL_AMOUNT);
            i.Heal(amount);
        }

        // Unfreeze
        await Task.Delay(RemoteSettings.Instance.PLAYER_DRINKING_FREEZE_MILLISECONDS);
        GM.GameState.Player.Motor.SetMovementFreeze(false);
        GM.GameState.Drinking = false;
    }
    #endregion

    #region Alien System
    public override void SaveControllingAlien()
    {
        if (!IsDead && IsControllingAlien())
        {
            MasterAudio.PlaySound("Collect4");
        }
        base.SaveControllingAlien();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AlienSaveSpot"))
        {
            SaveControllingAlien();

            foreach(PlayerCharacter p in GameSceneManager.Instance.GameState.Invaders)
            {
                p.SaveControllingAlien();
            }
        }

        if (other.CompareTag("AlienControlTrigger")) // Collision with alien
        {
            AlienController a = other.transform.parent.GetComponent<AlienController>(); // Get alien component

            if (a != null) // Check if alien component is ok
            {
                if (!IsControllingAlien()) // Player is not controlling alien
                {
                    SetControllingAlien(a);
                }
                else // Player is controlling alien, find an invader available
                {
                    List<InvaderController> availableInvaders = GameSceneManager.Instance.GameState.Invaders.Where(i => !i.IsDead && !i.IsControllingAlien()).ToList();
                    if (availableInvaders.Count > 0)
                    {
                        InvaderController first = availableInvaders.First();
                        first.SetControllingAlien(a); // Give alien controll to the first invader found
                    }
                }
            }
        }

        if (GM.GameState.IsEnergyStateActive)
        {
            if (other.CompareTag("Soldier"))
            {
                SoldierController soldier = other.GetComponent<SoldierController>();
                if (soldier != null && !soldier.IsDead)
                {
                    HitSoldierParticles.Play();
                    soldier.DoDeath();
                }
            }
        }
    }


    #endregion

    #region Energy system
    public void TryUseEnergy()
    {
        if (GM.GameState.CanUseEnergy())
        {
            GameSceneManager.Instance.GameState.CurrentKillStreak = 0;
            GM.GameState.IsEnergyStateActive = true;
            StartCoroutine(EnergyCoroutine());
        }
        else
        {
            Debug.LogError("Can't use energy!");
        }
    }

    float normalSprintSpeed = 0;
    IEnumerator EnergyCoroutine()
    {
        Motor.SetMovementFreeze(true);

        normalSprintSpeed = Motor.freeSprintSpeed;
        Motor.freeSprintSpeed = Motor.freeSprintSpeed * RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_SPEED_MODIFIER;

        MasterAudio.PlaySound3DAtTransformAndForget("Growl", transform);
        MasterAudio.PlaySound3DAtTransformAndForget("NightvisionOn", transform);
        anim.SetTrigger("EnergyActivation");
        MasterAudio.PlaySound3DAtTransformAndForget("ElectricSpark", transform);
        EnergyActivationParticles.Play();
        EnergyParticles.Play();

        yield return new WaitForSeconds(RemoteSettings.Instance.PLAYER_CHARACTER_ACTIVATE_ENERGY_FREEZE_TIME);

        Motor.SetMovementFreeze(false);

        while (GM.GameState.Energy > 0)
        {
            //    Debug.LogError("decreasing energy");
            float lerped = Mathf.Lerp(
                RemoteSettings.Instance.PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_AMOUNT,
                RemoteSettings.Instance.PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_AMOUNT_MIN,
                SkinManager.Instance.GetSkinLinearInterpolationPower(DataManager.Instance.PlayerData.UsingSkinID));
         //   Debug.LogError("Decreasing energy = " + lerped);
            GM.GameState.Energy -= lerped;
            yield return new WaitForSeconds(RemoteSettings.Instance.PLAYER_CHARACTER_ACTIVE_ENERGY_DECREASE_RATE);
        }

        GM.GameState.IsEnergyStateActive = false;
        Motor.freeSprintSpeed = normalSprintSpeed;

        EnergyParticles.Stop();
    }
    #endregion
}
