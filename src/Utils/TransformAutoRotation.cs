using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAutoRotation : MonoBehaviour
{
    public bool UseLocalRotation = false;
    public float RotationAmount = 0;
    public AxisEnum Axis = AxisEnum.Y;

    void Update()
    {
        float rot = RotationAmount * Time.deltaTime;
        transform.Rotate(Axis == AxisEnum.X || Axis == AxisEnum.UNIFORM ? rot : 0, Axis == AxisEnum.Y || Axis == AxisEnum.UNIFORM ? rot : 0, Axis == AxisEnum.Z || Axis == AxisEnum.UNIFORM ? rot : 0);
    }
}
