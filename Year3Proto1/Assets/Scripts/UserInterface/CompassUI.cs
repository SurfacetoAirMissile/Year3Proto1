using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassUI: MonoBehaviour
{
    public float pixelsToNorth;
    public GameObject target;

    private Vector3 startPosition;
    private float rotationAngle;

    private void Start()
    {
        startPosition = transform.position;
        rotationAngle = pixelsToNorth / 360.0f;
    }

    private void Update()
    {
        Vector3 cross = Vector3.Cross(Vector3.forward, target.transform.forward);
        float angle = Vector3.Angle(target.transform.forward, Vector3.forward);
        float direction = Vector3.Dot(cross, Vector3.up);


        transform.position = startPosition + (new Vector3(angle * Mathf.Sign(direction) * rotationAngle, 0, 0));
    }
}
