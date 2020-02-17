using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperShared : MonoBehaviour
{

    [Header("To Be Removed (Component Assignment)")]



    [SerializeField]
    protected GameObject chassis;

    [SerializeField]
    protected GameObject[] hoverballs;

    [SerializeField]
    protected GameObject windCannon;

    [SerializeField]
    protected GameObject bullet;
    /*
    [SerializeField]
    protected GameObject[] autoguns;
    */

    [SerializeField]
    [Tooltip("The amount of force applied when the Wind Cannon is fired, in thousands of units.")]
    protected float windCannonForce;

    [SerializeField]
    [Tooltip("The amount of force pushing up on the chassis from each Hoverball, in thousands of units/second.")]
    protected float hoverForce;

    [SerializeField]
    [Tooltip("The amount of force pushing on the chassis to move around, in thousands of units/second.")]
    protected float pushForce;

    [SerializeField]
    [Tooltip("The amount of force pushing on the chassis to rotate it, in thousands of units/second.")]
    protected float rotationForce;

    protected Rigidbody chassisRB;

    protected bool isTouchingGround;

    protected bool isFlipped;

    protected void Thrust(Vector3 _direction, float _amount)
    {
        if (!isFlipped)
        {
            float pushAmount = Time.deltaTime * 1000f * pushForce * _amount;
            Vector3 XZdirection = _direction; XZdirection.y = 0;
            chassisRB.AddForce(pushAmount * (isTouchingGround ? _direction.normalized : XZdirection.normalized));
        }
    }

    protected void Startup()
    {
        chassisRB = chassis.GetComponent<Rigidbody>();
    }

    protected void ApplyLevitationForces()
    {
        isTouchingGround = false;
        foreach (GameObject ball in hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 2f))
            {
                ball.GetComponent<Rigidbody>().AddForce(Mathf.Clamp(Time.deltaTime * hoverForce * 1000f / Mathf.Pow(groundCast.distance, 2f), 0f, Time.deltaTime * hoverForce * 10000f) * Vector3.up);
            }
            if (Physics.Raycast(ball.transform.position, Vector3.down, 3f))
            {
                isTouchingGround = true;
            }
        }
        isFlipped = false;
        Vector3 rotation = Quaternion.FromToRotation(chassis.transform.up, Vector3.up).eulerAngles;
        rotation.y = 0f;
        if (rotation.x >= 180) { rotation.x -= 360f; }
        if (rotation.z >= 180) { rotation.z -= 360f; }
        if (Mathf.Abs(rotation.x) > 30f || Mathf.Abs(rotation.z) > 30f)
        {
            isFlipped = true;
        }
        if (isTouchingGround && isFlipped)
        {
            chassisRB.AddTorque(rotation * Time.deltaTime * 1000f);
        }
    }
}
