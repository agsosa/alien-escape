using EmeraldAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Script execution order required to be less than GameSceneManager

public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
{
    float UpdateRate = 0.5f;

    [Header("Predefined Positions")]
    public Transform PlayerSpawnPoint;
    public List<Transform> InvaderSpawnPoints = new List<Transform>();
    public List<Transform> PredefinedPositions;

    [Header("Prefabs (ISpawnable)")]
    public PlayerController PlayerPrefab;
    public InvaderController InvaderPrefab;
    public List<SpawnableObject> SpawnablePrefabs = new List<SpawnableObject>();

    public List<SpawnableObject> ObjectPool = new List<SpawnableObject>();

    private void OnDisable()
    {
        Debug.Log("Destroying object pool");
        foreach(SpawnableObject o in ObjectPool)
        {
            if (o != null && o.gameObject != null) Destroy(o.gameObject);
        }

        Destroy(GameSceneManager.Instance.GameState.Player.gameObject);

        foreach (InvaderController o in GameSceneManager.Instance.GameState.Invaders)
        {
            if (o != null && o.gameObject != null) Destroy(o.gameObject);
        }
    }

    public void Initialize() // Called by GameSceneManager
    {
        Debug.LogError("Initializing SpawnManager...");

        if (PredefinedPositions.Count <= 1)
        {
            Debug.LogError("IMPORTANT: SpawnManager needs more Predefined Positions!");
            return;
        }

        Debug.LogError("Spawning dynamic prefabs");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        foreach (SpawnableObject prefab in SpawnablePrefabs)
        {
            if (prefab.SpawnType == SpawnType.MANUAL_PLACEMENT) continue;

            for (int i = 0; i < prefab.MaxInstances; i++)
            {
                SpawnableObject n = null;

                switch (prefab.SpawnType)
                {
                    case SpawnType.RANDOM_PREDEFINED_POSITION:
                        n = Instantiate(prefab, GetRandomPredefinedPosition(), Quaternion.identity);
                        break;
                    case SpawnType.RANDOM_MAP_POSITION:
                        n = Instantiate(prefab, GetRandomMapPosition(), Quaternion.identity);
                        break;
                    case SpawnType.PLAYER_FORWARD:
                        n = Instantiate(prefab);
                        break;
                }

                if (n.AddRespawnTimeOnStart) n.SetRespawnTime();
                if (n.ShouldStartInactive) n.gameObject.SetActive(false);

                if (n != null) ObjectPool.Add(n);
            }
        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.LogError("Spawning dynamic prefabs took " + elapsedMs+" ms");

        // Spawn Invaders
        Queue<Transform> q = new Queue<Transform>(InvaderSpawnPoints);
        for (int i = 0; i < RemoteSettings.Instance.MAX_INVADERS; i++)
        {
            var s = q.Dequeue();
            InvaderController b = Instantiate(InvaderPrefab, s.position, Quaternion.identity);
            GameSceneManager.Instance.GameState.Invaders.Add(b);
            GameSceneManager.Instance.GameState.CurrentAliveInvaders++;
            GameUIController.Instance.RegisterInvader(b);
            q.Enqueue(s);
        }

        // Spawn player
        GameSceneManager.Instance.GameState.Player = Instantiate(PlayerPrefab, PlayerSpawnPoint.position, Quaternion.identity);

        // Initialize invaders target
        foreach (InvaderController i in GameSceneManager.Instance.GameState.Invaders)
        {
            i.target = GameSceneManager.Instance.GameState.Player.transform;
        }

        // Start update interval

        InvokeRepeating("UpdateInterval", UpdateRate, UpdateRate);

        Debug.LogError("SpawnManager is ready");
    }

    void UpdateInterval()
    {
        if (GameSceneManager.Instance == null || GameSceneManager.Instance.GameState.CurrentPhase != GamePhase.IN_PROGRESS) return;

        // Try respawn objects
        foreach(SpawnableObject obj in ObjectPool)
        {
            if (obj != null && obj.RespawnType == RespawnType.RESPAWN_AFTER_TIME)
            {
                if (!obj.gameObject.activeSelf && Time.time > obj.NextRespawnTime)
                {
                    RespawnObject(obj);
                }
            }
        }
     }

    void RespawnObject(SpawnableObject obj)
    {
      //  Debug.LogError("Respawning " + obj.gameObject.name);

        if (obj.ReassignRandomPositionOnRespawn || obj.SpawnType == SpawnType.PLAYER_FORWARD)
        {
            switch (obj.SpawnType)
            {
                case SpawnType.PLAYER_FORWARD:
              //      Debug.LogError("Trying to respawn player_forward");
                    Transform pTransform = GameSceneManager.Instance.GameState.Player.transform;
                    Vector3 forwardPos = (pTransform.forward * obj.PlayerForwardDistance) + pTransform.position;
                    if (!GetWalkablePosition(forwardPos, 3.0f, out obj.AssignedPosition))
                    {
                   //     Debug.LogError("Can't respawn object " + obj.gameObject.name + " due to no walkable position found");
                        return;
                    }
                    break;
                case SpawnType.RANDOM_MAP_POSITION:
                    obj.AssignedPosition = GetRandomMapPosition();
                    break;
                case SpawnType.RANDOM_PREDEFINED_POSITION:
                    obj.AssignedPosition = GetRandomPredefinedPosition();
                    break;
            }
        }

        if (obj.AssignedPosition == null || obj.AssignedPosition == Vector3.zero) {
          //  Debug.LogError("Can't respawn object " + obj.gameObject.name + " due to invalid AssignedPosition");
            return;
        }

        obj.ResetComponents();
        obj.TeleportTo(obj.AssignedPosition);
        obj.gameObject.SetActive(true);
    }

    public void DespawnObject(SpawnableObject obj)
    {
        switch (obj.RespawnType)
        {
            case RespawnType.DESTROY_ON_DESPAWN:
               // Debug.LogError("Destroying obj " + obj.gameObject.name + " due to RespawnType.DESTROY_ON_DESPAWN");
                ObjectPool.Remove(obj);
                Destroy(obj.gameObject);
                break;
            case RespawnType.RESPAWN_AFTER_TIME:
           //     Debug.LogError("Deactivating obj " + obj.gameObject.name + " due to RespawnType.RESPAWN");
                obj.SetRespawnTime();
                obj.gameObject.SetActive(false);
                break;
            case RespawnType.INSTANT_RESPAWN:
             //   Debug.LogError("Respawning obj " + obj.gameObject.name + " due to RespawnType.INSTANT_RESPAWN");
                RespawnObject(obj);
                break;
        }
    }

    Vector3 GetRandomPredefinedPosition()
    {
        // Shuffle list
        PredefinedPositions.Shuffle();
        Vector3 result = PredefinedPositions[GameUtils.RNG.Next(PredefinedPositions.Count)].position;
        return result;
    }

    #region Random map position (navmesh)
    Mesh navMesh;
    public Mesh GetNavMesh()
    {
        if (navMesh == null)
        {
            NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();
            navMesh = new Mesh();
            navMesh.vertices = triangulatedNavMesh.vertices;
        }

        return navMesh;
    }

    public bool GetWalkablePosition(Vector3 position, float radius, out Vector3 walkable_position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, radius, NavMesh.AllAreas))
        {
            walkable_position = hit.position;
            return true;
        }

        walkable_position = Vector3.zero;

        return false;
    }

    public Vector3 GetRandomMapPosition()
    {
        Mesh navMesh = GetNavMesh();

        bool found = false;

        Vector3 walkablePos = Vector3.zero;
        Vector3 point1;
        Vector3 point2;
        Vector3 randomPoint;

        while (!found)
        {
            point1 = navMesh.vertices[UnityEngine.Random.Range(0, navMesh.vertexCount)];
            point2 = navMesh.vertices[UnityEngine.Random.Range(0, navMesh.vertexCount)];
            randomPoint = Vector3.Lerp(point1, point2, UnityEngine.Random.value);

            if (GetWalkablePosition(randomPoint, 3.0f, out walkablePos))
            {
                break;
            }
        }

        return walkablePos;
    }

    /*
     *   TESTING
     *  TESTING
     * TESTING
     *  TESTING
     *   TESTING
     */
    int TestMaxPositions = 100;
    float GizmoRadius = 0.5f;
    List<Vector3> resultPos = new List<Vector3>();
    [ContextMenu("Run test rand positions")]
    public void TestRandomMapPositions()
    {
        resultPos.Clear();
        for (int i = 0; i < TestMaxPositions; i++)
        {
            resultPos.Add(GetRandomMapPosition());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 p in resultPos)
        {
            Gizmos.DrawSphere(p, GizmoRadius);
        }

        if (GameSceneManager.Instance.GameState.Player.transform.position != null)
        {
            Gizmos.DrawSphere(GameSceneManager.Instance.GameState.Player.transform.position, GizmoRadius);
        }
    }

    #endregion
}
