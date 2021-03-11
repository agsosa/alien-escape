using DarkTonic.MasterAudio;
using EmeraldAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BASE CLASS FOR Player and Invader

// TODO: Add mesh effect while IsCurrentlyInvulnerable

public abstract class PlayerCharacter : MonoBehaviour
{
    public bool IsDead = false;
    public float Health = 100;
    public float MaxHealth = 100;

    public Skin AssignedSkin;

    public delegate void PlayerCharacterSimpleDelegate();
    public event PlayerCharacterSimpleDelegate OnDeath;

    internal Animator anim;
    int hitAnimHash;

    internal GameSceneManager GM;

    [HideInInspector] public AlienController ControlledAlien;

    #region Blood Effect
    public GameObject BloodEffect;
    IEnumerator BloodEffectCoroutine;
    bool PlayingBloodEffect = false;
    IEnumerator PlayBloodEffect()
    {
        PlayingBloodEffect = true;
        BloodEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        BloodEffect.SetActive(false);
        PlayingBloodEffect = false;
    }

    #endregion

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        hitAnimHash = Animator.StringToHash("Damaged");
        MaxHealth = RemoteSettings.Instance.PLAYER_CHARACTER_MAX_HEALTH;
        Health = RemoteSettings.Instance.PLAYER_CHARACTER_MAX_HEALTH;
        //GM.GameState.InvokeOnHealthChange();
        GM = GameSceneManager.Instance;
        GM.GameState.TotalMaxHealth += Health;
    }

    public virtual void AssignSkin(Skin skin)
    {
        AssignedSkin = Instantiate(skin, transform, false);

        AssignedSkin.SkinRig.transform.SetParent(transform);
        AssignedSkin.SkinMeshRenderer.transform.SetParent(transform);
        anim.avatar = AssignedSkin.AnimatorAvatar; // TODO: Pasar a una funcion virtual a PlayerCharacter
        anim.Rebind();
    }

    public void Heal(float amount)
    {
        if(!IsDead)
        {
            float restante_to_max = MaxHealth - Health;
            float sum_amount = amount + Health > MaxHealth ? restante_to_max : amount;
           // Debug.LogError("health = "+Health+" healing amount = " + amount + " restante = " + restante_to_max + " sum amount " + sum_amount);
            float new_health = Health + sum_amount;
            Health = new_health;
            GM.GameState.InvokeOnHealthChange();
        }
    }
    
    public virtual void OnEnable()
    {
        GameSceneManager.Instance.GameState.OnPlayerDrink += DoDrinkAnim;
    }

    public virtual void OnDisable()
    {
        GameSceneManager.Instance.GameState.OnPlayerDrink -= DoDrinkAnim;
    }

    public virtual void ProcessProjectileHit(EmeraldAISystem attacker) // Called when EmeraldAIProjectile collides with the invader
    {

        // TODO: Ver si agregar raycast para prevenir daño a traves de las paredes.
        /*RaycastHit hit;
        if (Physics.Linecast(attacker.transform.position, transform.position, out hit))
        {
            Debug.LogError("hit.transform.gameObject.layer = " + hit.transform.gameObject.layer);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                Debug.LogError("Soldier can't currently see this character");
                return;
            }
        }*/

        // TODO: Añadir chance de esquivar si esta sprinteando

        float dmg = Random.Range(RemoteSettings.Instance.PLAYER_CHARACTER_BULLET_DAMAGE_MIN, RemoteSettings.Instance.PLAYER_CHARACTER_BULLET_DAMAGE_MAX) * RemoteSettings.Instance.GetLinearDamageScalingMultiplier();
        ApplyDamage(dmg, true);

        if (Health <= 0)
        {
            attacker.EmeraldDetectionComponent.SearchForTarget();
        }
    }

    public virtual void ApplyDamage(float dmg, bool isProjectile)
    {
        if (dmg < 0) return;
        if (IsDead) return;
        if (GM.GameState.IsPlayerInvulnerable()) return;
        if (GM.GameState.CurrentPhase != GamePhase.IN_PROGRESS) return;

        if (isProjectile)
        {
            MasterAudio.PlaySound3DAtTransformAndForget("BulletImpactBody", transform);
        }

        float newhealth = Mathf.Max(0, Health - dmg);
        Health = newhealth;
        GM.GameState.InvokeOnHealthChange();

        anim.SetTrigger(hitAnimHash);

        if (!PlayingBloodEffect)
        {
            BloodEffectCoroutine = PlayBloodEffect();
            StartCoroutine(BloodEffectCoroutine);
        }

        float min = RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MIN;
        float max = RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MAX;
        float eneGain = Mathf.Max(min, Mathf.Min(dmg * RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_PER_DAMAGE_MULTIPLIER, max));
        if (Health <= 0) eneGain = RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_PER_DEATH;
       // Debug.LogError("Damage = " + dmg + " Energy gain = " + eneGain);
        GameSceneManager.Instance.GameState.Energy += eneGain;

        if (Health <= 0)
        {
            MasterAudio.PlaySound3DAtTransformAndForget("HumanDeath", transform);
            DoDeath();
        }
        else
        {
            MasterAudio.PlaySound3DAtTransformAndForget("HumanPain", transform);
        }
    }

    public virtual void DoDeath()
    {
        IsDead = true;
        FreeControllingAlien();
        gameObject.tag = "DeadPlayerCharacter";
        gameObject.layer = LayerMask.NameToLayer("DeadPlayerCharacter");
        anim.SetBool("IsDead", true);
        GM.GameState.CurrentAliveInvaders = Mathf.Max(0, GM.GameState.CurrentAliveInvaders - 1);

        //anim.enabled = false;

        if (OnDeath != null) OnDeath();
    }

    public virtual void DoDrinkAnim()
    {
        if (IsDead) return;

     //   Debug.LogError("DoDrink()");
        anim.SetTrigger("Drink");
    }

    public abstract bool ActuallySprintingAndMoving();

    #region Alien Controlling
    public bool IsControllingAlien()
    {
        return ControlledAlien != null;
    }

    public void SetControllingAlien(AlienController alien)
    {
        if (!IsControllingAlien())
        {
            if (alien.CanBeControlledByPlayer())
            {
                alien.SetController(this);
                ControlledAlien = alien;
            }
        }
    }

    public void FreeControllingAlien()
    {
        if (IsControllingAlien())
        {
            ControlledAlien.FreeController();
            ControlledAlien = null;
        }
    }

    public virtual void SaveControllingAlien()
    {
        if (!IsDead && IsControllingAlien())
        {
            GM.GameState.CurrentSavedAliens++;
            ControlledAlien.DOSaveAlien();
            ControlledAlien = null;
            // TODO: Destroy alien or something. Ver aliencontroller. 
        }
    }
    #endregion
}
