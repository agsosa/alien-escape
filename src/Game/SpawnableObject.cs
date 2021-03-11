using System;
using UnityEngine;

public abstract class SpawnableObject: MonoBehaviour
{
    public SpawnType SpawnType = SpawnType.MANUAL_PLACEMENT;
    public RespawnType RespawnType = RespawnType.DESTROY_ON_DESPAWN; // El objeto debe ser destruido o respawneado al llamar SpawnManager.Despawn?
    public float MinRespawnTime = 5; // Tiempo minimo que esperara el SpawnerManager para respawnear este objeto
    public float MaxRespawnTime = 10; // Tiempo maximo que esperara el SpawnerManager para respawnear este objeto
    public int MaxInstances = 10; // Not used in SpawnType.MANUAL_PLACEMENT
    public bool ShouldStartInactive = false;
    public bool AddRespawnTimeOnStart = false;
    public float PlayerForwardDistance = 5f; // Only for SpawnType.PLAYER FORWARD
    public bool ReassignRandomPositionOnRespawn = true;

    [HideInInspector] public float NextRespawnTime = 0;
    [HideInInspector] public Vector3 AssignedPosition;

    public abstract void ResetComponents(); // Called before the SpawnerManager spawn the object
    public abstract void TeleportTo(Vector3 position);

    public virtual void Start()
    {
        AssignedPosition = transform.position;
        if (SpawnType == SpawnType.MANUAL_PLACEMENT)
        {
            SpawnManager.Instance.ObjectPool.Add(this);
        }
    }

    public void SetRespawnTime()
    {
        NextRespawnTime = Time.time + UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime);
    }
}