using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Implementar para rectTransform!!!!

public class PingPongBase: MonoBehaviour
{
    public AxisEnum AxisToPingPong;
    public float MinValue = 0;
    public float MaxValue = 0;
    public float TimeModifier = 1;
    public bool UseLocalSpace = false;

    internal float CurrValue;
    internal Vector3 _Vector3;

    public virtual void Update()
    {
        CurrValue = GameUtils.PingPongMinMax(Time.time * TimeModifier, MinValue, MaxValue);
    }
}
