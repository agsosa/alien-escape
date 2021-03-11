using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleShakeEverySeconds : MonoBehaviour
{
    public float SleepSeconds = 2f;
    public float ShakeDuration = 1;
    public float strength = 1;
    public int vibrato = 10;
    public float randomness = 90;
    public bool fadeout = true;

    float NextUpdate = 0;
    void Update()
    {
        if (Time.time > NextUpdate)
        {
            transform.DOShakeScale(ShakeDuration, strength, vibrato, randomness, fadeout);
            NextUpdate = Time.time + SleepSeconds;
        }
    }
}
