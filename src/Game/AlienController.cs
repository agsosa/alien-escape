using DG.Tweening;
using EckTechGames.FloatingCombatText;
using EmeraldAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienController : SpawnableObject
{
    [HideInInspector] public PlayerCharacter CurrentController;
    [Header("AlienController Settings")]
    public Vector3 AttachPointPositionOffset = new Vector3(0, 0, 0);
    public Vector3 AttachPointRotationOffset = new Vector3(0, 0, 0);
    public Vector3 SprintingAttachPointPositionOffset = new Vector3(0, 0, 0);
    public Vector3 SprintingAttachPointRotationOffset = new Vector3(0, 0, 0);
    public int MinSittingIdleIndex = 0;
    public int MaxSittingIdleIndex = 2;

    public float LerpSpeed = 1.5f;

    Rigidbody rb;
    Animator anim;
    EmeraldAISystem emerald;
    NavMeshAgent agent;

    public override void Start()
    {
        base.Start();
        GameSceneManager.Instance.GameState.Aliens.Add(this);
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        emerald = GetComponent<EmeraldAISystem>();
        agent = GetComponent<NavMeshAgent>();
    }

    bool attachPosSkipLerp = true;
    private void Update()
    {
        if (CurrentController != null)
        {
            Vector3 posOffset = CurrentController.ActuallySprintingAndMoving() ? SprintingAttachPointPositionOffset : AttachPointPositionOffset;
            Vector3 rotOffset = CurrentController.ActuallySprintingAndMoving() ? SprintingAttachPointRotationOffset : AttachPointRotationOffset;

            transform.localPosition = attachPosSkipLerp ? posOffset : Vector3.Lerp(transform.localPosition, posOffset, Time.deltaTime * LerpSpeed);

            Quaternion newRotation = Quaternion.Euler(rotOffset);
            transform.localRotation = attachPosSkipLerp ? newRotation : Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * LerpSpeed);

            if (attachPosSkipLerp) attachPosSkipLerp = false;

            /* Vector3 rot = transform.localRotation.eulerAngles;
             rot.x = 0;
             transform.localRotation = Quaternion.Euler(rot);*/ //AttachPointRotationOffset

            /*Vector3 posOffset = CurrentController.Motor.ActuallySprintingAndMoving() ? SprintingAttachPointPositionOffset : AttachPointPositionOffset;
            rb.MovePosition(CurrentController.AlienAttachPoint.localPosition + posOffset);

            Vector3 rot = CurrentController.AlienAttachPoint.localRotation.eulerAngles;
            rot.x = 0;
            rb.MoveRotation(Quaternion.Euler(rot + AttachPointRotationOffset));*/
        }
    }

    public bool CanBeControlledByPlayer()
    {
        return CurrentController == null && !EnteringUfo;
    }

    public void FreeController()
    {
        ResetComponents();
    }

    public void SetController(PlayerCharacter p)
    {
        if (!CanBeControlledByPlayer()) return;

        // Setup components
        emerald.enabled = false;
        agent.enabled = false;

        // Finally set controller
        attachPosSkipLerp = true;

        CurrentController = p;
        transform.SetParent(CurrentController.AssignedSkin.Neck);

        anim.SetFloat("SitAnim", GameUtils.currAlienSittingIdleIndex);
        GameUtils.currAlienSittingIdleIndex = GameUtils.currAlienSittingIdleIndex + 1 > MaxSittingIdleIndex ? MinSittingIdleIndex : GameUtils.currAlienSittingIdleIndex + 1;

        anim.SetBool("BeingControlledByPlayer", true);

        Debug.LogError("Alien is now controlled by a char"); 
    }

    IEnumerator SaveAlienCoroutine;
    public bool EnteringUfo = false;
    Tween EnteringUfoTween;

    public void DOSaveAlien()
    {
        EnteringUfo = true;

        GameSceneManager.Instance.AddScore(ScoreGainType.ALIEN_SAVED);

        CurrentController = null;
        transform.SetParent(null);
        anim.SetBool("BeingControlledByPlayer", false);

        anim.SetBool("EnteringUfo", true);

        transform.position = EnvironmentController.Instance.AlienSaveSpotCenter.position + new Vector3(0, 1, 0);

        if (SaveAlienCoroutine != null) StopCoroutine(SaveAlienCoroutine);
        SaveAlienCoroutine = SaveAlienCoroutineImpl();
        StartCoroutine(SaveAlienCoroutine);
    }

    IEnumerator SaveAlienCoroutineImpl()
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.5f));
        EnteringUfoTween = transform.DOMove(EnvironmentController.Instance.UFOEntrance.position, 5);
        yield return new WaitUntil(() => IsEnteringUfoTweenCompleted());

        anim.SetBool("EnteringUfo", false);
        EnteringUfo = false;
        SpawnManager.Instance.DespawnObject(this);
    }

    bool IsEnteringUfoTweenCompleted()
    {
        return (EnteringUfoTween.IsComplete() || EnteringUfoTween == null || !EnteringUfoTween.IsActive());
    }

    #region Spawneable Impl
    public override void ResetComponents()
    {
        if (SaveAlienCoroutine != null) StopCoroutine(SaveAlienCoroutine);
        transform.SetParent(null);
        anim.SetBool("BeingControlledByPlayer", false);
        anim.SetBool("EnteringUfo", false);
        CurrentController = null;
        EnteringUfoTween.Kill();
        EnteringUfo = false;
        transform.rotation = Quaternion.identity;
        agent.enabled = true;
        emerald.enabled = true;
    }

    public override void TeleportTo(Vector3 position)
    {
        emerald.StartingDestination = position;
        agent.Warp(position);
        agent.SetDestination(position);
    }
    #endregion
}
