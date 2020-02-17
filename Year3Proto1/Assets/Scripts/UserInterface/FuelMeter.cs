using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : MonoBehaviour
{
    public Transform needle;

    private const float MIN_ANGLE = -20.0f;
    private const float MAX_ANGLE = 120.0f;

    private float fuel;
    private float fuelMax;

    private void Update()
    {
        needle.eulerAngles = new Vector3(0.0f, 0.0f, MIN_ANGLE - (MIN_ANGLE - MAX_ANGLE) * (fuel / fuelMax));
    }
}
