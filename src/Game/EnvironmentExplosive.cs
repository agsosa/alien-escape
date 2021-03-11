using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Important: Use Layer collision matrix (Physics Settings) to change the possible targets!!

public class EnvironmentExplosive : SpawnableObject
{
    [Space]
    [Header("Use Layer collision matrix (Physics Settings) to change the trigger targets!!")]
    [Header("Setup the LayerMask below for the explosive targets (not the same as trigger targets)")]
    [Header("Don't forget to setup a SphereCollider to trigger the explosion")]
    [Space]
    public EnvironmentExplosiveType EnvironmentExplosiveType;
    public float DamageRadius = 0.5f;
    public bool IsDelayed = false;
    public float DelayedExplosionSeconds = 3;
    public LayerMask ExplosionOverlapTargets;
    public string DamageTagTarget = "Invader";

    [Header("Can be null")]
    public ParticleSystem ExplosionParticles;
    public SpriteRenderer AreaRenderer, WarningRenderer;
    public MeshRenderer MeshRenderer;

    bool Exploding = false;
    SphereCollider Trigger;
    SphereCollider DamageArea;
    IEnumerator DelayedExplosionCoroutine;

    private void OnEnable()
    {
        if (IsDelayed) // TODO: Change to ActivateOnEnable
        {
            if (DelayedExplosionCoroutine != null) StopCoroutine(DelayedExplosionCoroutine);
            DelayedExplosionCoroutine = DODelayedExplosion();
            StartCoroutine(DelayedExplosionCoroutine);
        }
    }

    public override void Start()
    {
        base.Start();

        Trigger = GetComponent<SphereCollider>();

        if (Trigger == null) Debug.LogError("SphereCollider Trigger is missing!");
        overlapResults = new Collider[1+RemoteSettings.Instance.MAX_INVADERS];
    }

    private void OnTriggerEnter(Collider other)
    {
        // Important: Use Layer collision matrix (Physics Settings) to change the possible targets!!
        if (!Exploding && !IsDelayed)
        {
            Exploding = true;
            DOInstantExplosion();
        }
    }

    public void DOInstantExplosion()
    {
        DODamage();
        PlayExplosionEffect();
    }

    IEnumerator DODelayedExplosion()
    {
        // TODO: Play sounds etc
        yield return new WaitForSeconds(DelayedExplosionSeconds);
        DODamage();
        PlayExplosionEffect();
    }

    async void PlayExplosionEffect()
    {
        // TODO: Play sound

        EnvironmentController.Instance.InvokeEnvironmentExplosionEvent(transform, EnvironmentExplosiveType);

        if (EnvironmentExplosiveType == EnvironmentExplosiveType.MISIL)
        {
            MasterAudio.PlaySound3DAtTransformAndForget("MisilExplosion", transform);
        }

        if (EnvironmentExplosiveType == EnvironmentExplosiveType.MINE)
        {
            MasterAudio.PlaySound3DAtTransformAndForget("MineExplosion", transform);
        }

        if (AreaRenderer != null) AreaRenderer.enabled = false;
        if (WarningRenderer != null) WarningRenderer.enabled = false;
        if (MeshRenderer != null) MeshRenderer.enabled = false;
        if (ExplosionParticles != null) ExplosionParticles.Play();

        await Task.Delay(3000);
        SpawnManager.Instance.DespawnObject(this);
    }

    private Collider[] overlapResults;
    void DODamage()
    {
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, DamageRadius, overlapResults, ExplosionOverlapTargets);

        for (int i = 0; i < numFound; i++)
        {
            if (overlapResults[i].CompareTag(DamageTagTarget))
            {
                PlayerCharacter target = overlapResults[i].GetComponent<PlayerCharacter>();
                if (target != null && !target.IsDead)
                {
                    float proximity = (transform.position - target.transform.position).magnitude;
                    float distance_modifier = 1 - (proximity / DamageRadius);

                    float dmg = 0;

                    switch (EnvironmentExplosiveType)
                    {
                        case EnvironmentExplosiveType.MINE: dmg = RemoteSettings.Instance.PLAYER_CHARACTER_MINE_EXPLOSION_DAMAGE; break;
                        case EnvironmentExplosiveType.MISIL: dmg = RemoteSettings.Instance.PLAYER_CHARACTER_MISIL_EXPLOSION_DAMAGE; break;
                    }

                    target.ApplyDamage(dmg * distance_modifier * RemoteSettings.Instance.GetLinearDamageScalingMultiplier(), false);

              //      Debug.LogError(EnvironmentExplosiveType.ToString() + " Hitting character with dmg = " + dmg + " damage modifier " + distance_modifier + " distance = " + proximity + " final dmg = " + dmg * distance_modifier);
                }
            }
        }

        /*Collider[] objectsInRange = Physics.OverlapSphere(transform.position, DamageRadius, );
        foreach (Collider col in objectsInRange)
        {
            PlayerCharacter enemy = col.GetComponent<PlayerCharacter>();
            if (enemy != null)
            {
                // linear falloff of effect
                float proximity = (location - enemy.transform.position).magnitude;
                float effect = 1 - (proximity / radius);


                enemy.ApplyDamage(damage * effect);
            }
        }*/
    }

    #region Spawnable Object Impl
    public override void ResetComponents()
    {
        Exploding = false;
        if (AreaRenderer != null) AreaRenderer.enabled = true;
        if (WarningRenderer != null) WarningRenderer.enabled = true;
        if (MeshRenderer != null) MeshRenderer.enabled = true;
        if (ExplosionParticles != null) ExplosionParticles.Stop();
    }

    public override void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
    #endregion
}
