using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftShared : MonoBehaviour
{
    protected GameObject chassis;
    protected List<GameObject> hoverballs;
    protected float totalMass;

    [SerializeField] [Tooltip("The amount of force pushing up on the chassis from each Hoverball, in thousands of units/second.")]
    protected float hoverForce;

    [SerializeField] [Tooltip("The amount of force pushing on the chassis to move around, in thousands of units/second.")]
    protected float pushForce;

    [SerializeField] [Tooltip("The amount of force pushing on the chassis to rotate it, in thousands of units/second.")]
    protected float rotationForce;

    protected Rigidbody chassisRB;
    protected bool isTouchingGround;
    protected bool isFlipped;

    protected void Thrust(Vector3 _direction, float _power)
    {
        if (!isFlipped)
        {
            float pushAmount = Time.deltaTime * 1000f * pushForce * _power;
            Vector3 XZdirection = _direction; XZdirection.y = 0;
            chassisRB.AddForce(pushAmount * (isTouchingGround ? _direction.normalized : XZdirection.normalized));
        }
    }

    protected void HovercraftStartup()
    {
        totalMass = GetTotalMass();
        hoverballs = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Hoverball")) { hoverballs.Add(child.gameObject); }
            if (child.name.Contains("Chassis")) { chassis = child.gameObject; }
            //if (child.name.Contains("Wind Cannon")) { weapons.Add(child.gameObject); }
            //if (child.name.Contains("Minigun")) { weapons.Add(child.gameObject); }
            //if (child.name.Contains("Mortar")) { weapons.Add(child.gameObject); }
        }
        chassisRB = chassis.GetComponent<Rigidbody>();
    }

    protected void ApplyLevitationForces()
    {
        isTouchingGround = false;
        foreach (GameObject ball in hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 1f))
            {
                float forceStrength = Time.deltaTime * hoverForce * 1000f;
                float maxForceApplication = forceStrength * 10f;
                float distanceSquared = Mathf.Pow(groundCast.distance, 2f);
                ball.GetComponent<Rigidbody>().AddForce(Mathf.Clamp(forceStrength / distanceSquared, 0f, maxForceApplication) * chassis.transform.up);
            }
            if (Physics.Raycast(ball.transform.position, Vector3.down, 2f))
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

    float GetTotalMass()
    {
        float massTotal = 0;
        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>())
        {
            massTotal += body.mass;
        }
        return massTotal;
    }
}
