using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseController : SpawnableObject
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            MasterAudio.PlaySound("Collect3");
            GameSceneManager.Instance.AddScore(ScoreGainType.STOLEN_BRIEFCASE);
            GameSceneManager.Instance.GameState.CurrentStolenBriefcases++;
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
