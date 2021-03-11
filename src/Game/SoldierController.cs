using DarkTonic.MasterAudio;
using EmeraldAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : SpawnableObject
{
    public AudioSource ShootSound;
    public ParticleSystem MuzzleEffect;

    Animator anim;
    EmeraldAISystem emerald;
    NavMeshAgent agent;
    [HideInInspector] public bool IsDead = false;

    public override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        emerald = GetComponent<EmeraldAISystem>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Play Weapon Shoot effect
    public void CreateEmeraldProjectile() // Called by animation event
    {
        MasterAudio.PlaySound3DAtTransformAndForget("RifleShoot", transform);
        if (MuzzleEffect != null) MuzzleEffect.Play();
    }

    public async void DoDeath()
    {
        CameraController.Instance.ShakeCamera(4.5f);
        GameSceneManager.Instance.AddScore(ScoreGainType.SOLDIER_KILL);
        emerald.enabled = false;
        agent.enabled = false;
        IsDead = true;
        int rand = Random.Range(0, 2);
        anim.SetFloat("DeadIndex", rand);
        anim.SetBool("IsDead", true);

        GameSceneManager.Instance.GameState.CurrentKillStreak++;
        int currKillStreak = GameSceneManager.Instance.GameState.CurrentKillStreak;

        if (currKillStreak == 2)
        {
            MasterAudio.PlaySound("DoubleKill");
        }
        else if (currKillStreak == 3)
        {
            MasterAudio.PlaySound("TripleKill");
        }

        if (currKillStreak > 3)
        {
            MasterAudio.PlaySound("KillingSpreeRandom");
        }

        MasterAudio.PlaySound3DAtTransformAndForget("Punch", transform);
        MasterAudio.PlaySound3DAtTransformAndForget("HumanDeath", transform);

        await Task.Delay(5000);

        SpawnManager.Instance.DespawnObject(this);
    }

    #region Spawnable Impl
    public override void ResetComponents()
    {
        emerald.enabled = true;
        agent.enabled = true;
        IsDead = false;
        anim.SetBool("IsDead", false);
    }

    public override void TeleportTo(Vector3 position)
    {
        emerald.StartingDestination = position;
        agent.Warp(position);
        agent.SetDestination(position);
    }
    #endregion
}
