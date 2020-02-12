using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperShared : MonoBehaviour
{
    [SerializeField]
    protected GameObject chassis;

    [SerializeField]
    protected GameObject[] hoverballs;

    [SerializeField]
    protected GameObject windCannon;

    [SerializeField]
    protected GameObject bullet;

    [SerializeField]
    protected GameObject[] autoguns;

    [SerializeField]
    protected float CannonForce;

    [SerializeField]
    protected float HoverForce;

    [SerializeField]
    protected float PushForce;

    [SerializeField]
    protected float RotationForce;

    protected Rigidbody chassisRB;

    protected void Startup()
    {
        chassisRB = chassis.GetComponent<Rigidbody>();
    }

    protected void ApplyLevitationForces()
    {
        foreach (GameObject ball in hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 2f))
            {
                ball.GetComponent<Rigidbody>().AddForce(Mathf.Clamp(Time.deltaTime * HoverForce / Mathf.Pow(groundCast.distance, 2f), 0f, 10f) * Vector3.up);
            }
        }
    }
}
