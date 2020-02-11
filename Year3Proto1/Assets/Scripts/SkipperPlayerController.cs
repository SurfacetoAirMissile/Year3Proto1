using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperPlayerController : SkipperShared
{
    // Start is called before the first frame update
    void Start()
    {
        Startup();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();

        float pushAmount = Time.deltaTime * PushForce;
        float rotationAmount = Time.deltaTime * RotationForce;

        if (Input.GetKey("w"))
        {
            thisRB.AddForce(pushAmount * transform.forward);
        }
        if (Input.GetKey("s"))
        {
            thisRB.AddForce(pushAmount * -transform.forward);
        }
        if (Input.GetKey("e"))
        {
            thisRB.AddForce(pushAmount * transform.right);
        }
        if (Input.GetKey("q"))
        {
            thisRB.AddForce(pushAmount * -transform.right);
        }
        if (Input.GetKey("d"))
        {
            thisRB.AddTorque(new Vector3(0f, rotationAmount, 0f));
        }
        if (Input.GetKey("a"))
        {
            thisRB.AddTorque(new Vector3(0f, -rotationAmount, 0f));
        }
    }
}
