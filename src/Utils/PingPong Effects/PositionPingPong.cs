using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPingPong : PingPongBase
{
    public override void Update()
    {
        base.Update();

        _Vector3.x = UseLocalSpace ? transform.localPosition.x : transform.position.x;
        _Vector3.y = UseLocalSpace ? transform.localPosition.y : transform.position.y;
        _Vector3.z = UseLocalSpace ? transform.localPosition.z : transform.position.z;

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

        if (UseLocalSpace) transform.localPosition = _Vector3; else transform.position = _Vector3;
    }
}
