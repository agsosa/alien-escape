
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderController : PlayerCharacter
{
    [Header("AI Follow Player Motor")]
    public Transform target;

    // Internal vars
    Rigidbody rb;
    int blendVelocityHash; // animation
    Vector3 lastPos; // animation
    float position_magnitude_difference; // animation

    public override void Start()
    {
        base.Start();

        AssignSkin(SkinManager.Instance.GetRandomSkinPrefab());

        rb = GetComponent<Rigidbody>();
        blendVelocityHash = Animator.StringToHash("Velocity");
        lastPos = transform.position;
    }

    public void Revive()
    {
        IsDead = false;
        Heal(MaxHealth);
        gameObject.tag = "Invader";
        gameObject.layer = LayerMask.NameToLayer("Invader");
        anim.SetBool("IsDead", false);
        GM.GameState.CurrentAliveInvaders++;
        rb.MovePosition(GameSceneManager.Instance.GameState.Player.transform.position);
    }

    void FixedUpdate() // TODO: Optimize (poner un rate, pasar a update?)
    {
        if (!IsDead && target != null)
        {
            Animate();
            FollowPlayer();
        }
    }

    public override bool ActuallySprintingAndMoving()
    {
        return anim.GetFloat(blendVelocityHash) > 0.65f;
    }

    void FollowPlayer()
    {
        Quaternion targetQ = Quaternion.LookRotation(target.position - transform.position);
        targetQ.x = 0;
        targetQ.z = 0;
        Quaternion new_rotation = Quaternion.Slerp(transform.rotation, targetQ, RemoteSettings.Instance.INVADER_ROTATION_SPEED * Time.fixedDeltaTime);
        rb.MoveRotation(new_rotation.normalized);

        if (Vector3.SqrMagnitude(target.position - transform.position) >= RemoteSettings.Instance.INVADER_FOLLOW_PLAYER_STOP_DISTANCE)
        {
            // TODO: Cambiar movespeed dependiendo de la distancia
            float motorPowerDif = GameSceneManager.Instance.GameState.Player.Motor.freeSprintSpeed - 5;
            float currSpeed = GameSceneManager.Instance.GameState.Player.Motor.isSprinting ? (RemoteSettings.Instance.INVADER_PLAYER_SPRINTING_SPEED+motorPowerDif) : RemoteSettings.Instance.INVADER_PLAYER_NORMAL_RUN_SPEED;
            currSpeed = GameSceneManager.Instance.GameState.IsEnergyStateActive ? currSpeed * RemoteSettings.Instance.PLAYER_CHARACTER_ENERGY_SPEED_MODIFIER * 0.8f : currSpeed;
            rb.velocity = transform.forward * currSpeed;
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime);
        }
    }

    void Animate() // TODO: Optimize No need to call SetFloat every fixed update
    {
        bool moving = false;

        Vector3 speed = transform.position - lastPos;
        speed /= Time.fixedDeltaTime;

        position_magnitude_difference = speed.magnitude;

        if (position_magnitude_difference > RemoteSettings.Instance.INVADER_MIN_MAGNITUDE_MOVING)
        {
            moving = true;
        }

        anim.SetFloat(blendVelocityHash, Mathf.Lerp(anim.GetFloat(blendVelocityHash), moving ? 1 : 0, Time.fixedDeltaTime * RemoteSettings.Instance.INVADER_ANIM_LERP_SPEED));

        lastPos = transform.position;
    }

    public override void DoDeath()
    {
        base.DoDeath();
        rb.velocity = Vector3.zero;
    }
}
