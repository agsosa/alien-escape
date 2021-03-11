using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Eliminar este script, para rotacion usar TransformAutoRotation.cs, para position usar PingPongPosition, para emission color hacer un script nuevo

public class UFOAnimator : MonoBehaviour
{
    public float RotationAmount = 20f;
    public float MinY = 7.2f;
    public float MaxY = 8.3f;
    public float PingPongPosTime = 1;
    public float EmissionPingPongTimeModifier = 1.5f;

    [ColorUsageAttribute(true, true)] public Color EmissionColor1;
    [ColorUsageAttribute(true, true)] public Color EmissionColor2;

    public Material lod1_mat;

    Vector3 _pos = new Vector3();

    void Update()
    {
        transform.Rotate(0, RotationAmount * Time.deltaTime, 0);
        lod1_mat.SetColor("_EmissionColor", Color.Lerp(EmissionColor1, EmissionColor2, Mathf.PingPong(Time.time * EmissionPingPongTimeModifier, 1)));
        _pos.x = transform.localPosition.x;
        _pos.y = GameUtils.PingPongMinMax(Time.time * PingPongPosTime, MinY, MaxY);
        _pos.z = transform.localPosition.z;
        transform.localPosition = _pos;
    }
}
