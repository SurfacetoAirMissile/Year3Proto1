using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFunc
{
    public static void RotateTo(Rigidbody _bodyToRotate, char _axis, float _rotationForce)
    {
        // clamp between -90 and 90
        float clampedForce = Mathf.Clamp(_rotationForce, -90f, 90f);
        float forceValue = clampedForce / 90f;
        float finalForce = forceValue * _bodyToRotate.mass * 100000f * Time.deltaTime;
        switch (_axis)
        {
            case 'x':
                _bodyToRotate.AddTorque(finalForce, 0f, 0f);
                break;
            case 'y':
                _bodyToRotate.AddTorque(0f, finalForce, 0f);
                break;
            case 'z':
                _bodyToRotate.AddTorque(0f, 0f, finalForce);
                break;
        }
    }
}
