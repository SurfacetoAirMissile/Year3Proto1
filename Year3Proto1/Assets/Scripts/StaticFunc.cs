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
                _bodyToRotate.AddRelativeTorque(finalForce, 0f, 0f);
                break;
            case 'y':
                _bodyToRotate.AddRelativeTorque(0f, finalForce, 0f);
                break;
            case 'z':
                _bodyToRotate.AddRelativeTorque(0f, 0f, finalForce);
                break;
        }
    }

    private static Dictionary<string, float> floatData = new Dictionary<string, float>
    { { "Tortoise Orbit Distance", 20f },
    { "Skipper Orbit Distance", 15f },
    { "Scimitar Orbit Distance", 10f } };

    public static float FloatLookup(string _key)
    {
        return floatData[_key];
    }

    static public Transform GetParentRecursive(Transform _transform)
    {
        // If the transform has a parent
        if (_transform.parent)
        {
            // Call the function on the parent and return the result to the caller
            return GetParentRecursive(_transform.parent);
        }
        // If the transform does not have a parent, return it (that is the omega)
        else return _transform;
    }
}
