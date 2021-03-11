using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColaCanController : SpawnableObject
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            MasterAudio.PlaySound("Collect1");
            GameSceneManager.Instance.GameState.CurrentAvailableDrinks++;
            SpawnManager.Instance.DespawnObject(this);
        }
    }

    public override void ResetComponents()
    {

    }

    public override void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
