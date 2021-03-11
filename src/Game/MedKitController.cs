using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKitController : SpawnableObject
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            MasterAudio.PlaySound("Collect2");
            SpawnManager.Instance.DespawnObject(this);
            GameSceneManager.Instance.GameState.CurrentMedKits++;
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
