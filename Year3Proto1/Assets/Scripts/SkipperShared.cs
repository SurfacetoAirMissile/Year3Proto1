using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperShared : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] Hoverballs;

    [SerializeField]
    protected float HoverForce;

    [SerializeField]
    protected float PushForce;

    [SerializeField]
    protected float RotationForce;

    protected Rigidbody thisRB;

    protected void Startup()
    {
        thisRB = GetComponent<Rigidbody>();
    }

    protected void ApplyLevitationForces()
    {
        foreach (GameObject ball in Hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 2f))
            {
                ball.GetComponent<Rigidbody>().AddForce(Time.deltaTime * HoverForce / Mathf.Clamp(Mathf.Pow(groundCast.distance, 2f), 0f, 1f) * Vector3.up);
            }
        }
    }
}
