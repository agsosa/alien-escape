using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePingPong : PingPongBase
{
    private new bool UseLocalSpace;

    public override void Update()
    {
        base.Update();

        _Vector3 = transform.localScale;

        switch (AxisToPingPong)
        {
            case AxisEnum.X:
                _Vector3.x = CurrValue;
                break;
            case AxisEnum.Y:
                _Vector3.y = CurrValue;
                break;
            case AxisEnum.Z:
                _Vector3.z = CurrValue;
                break;
            case AxisEnum.UNIFORM:
                _Vector3.x = CurrValue;
                _Vector3.y = CurrValue;
                _Vector3.z = CurrValue;
                break;
        }

        transform.localScale = _Vector3;
    }
}
