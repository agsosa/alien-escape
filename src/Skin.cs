using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [Header("Skin Properties")]
    public int ID = -1;
    public int Heals = 3;
    public int ExtraSeconds = 0;
    public int Revives = 3;
    public int Power = 1;
    public int RequiredSavedAliens = 0;
    public int RequiredStolenBriefcases = 0;

    [Header("Components")]
    public Transform Neck; // Alien attach point
    public SkinnedMeshRenderer SkinMeshRenderer;
    public GameObject SkinRig;
    public Avatar AnimatorAvatar;
}
