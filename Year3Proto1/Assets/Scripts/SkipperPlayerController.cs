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
            chassisRB.AddForce(pushAmount * chassis.transform.forward);
        }
        if (Input.GetKey("s"))
        {
            chassisRB.AddForce(pushAmount * -chassis.transform.forward);
        }
        if (Input.GetKey("e"))
        {
            chassisRB.AddForce(pushAmount * chassis.transform.right);
        }
        if (Input.GetKey("q"))
        {
            chassisRB.AddForce(pushAmount * -chassis.transform.right);
        }
        if (Input.GetKey("d"))
        {
            chassisRB.AddTorque(0f, rotationAmount, 0f);
        }
        if (Input.GetKey("a"))
        {
            chassisRB.AddTorque(0f, -rotationAmount, 0f);
        }
        Vector3 rotation;
        if (Input.GetKey("left ctrl"))
        {
            rotation = Quaternion.FromToRotation(cannon.transform.up, Camera.main.transform.forward).eulerAngles;
        }
        else
        {
            rotation = Quaternion.FromToRotation(-cannon.transform.up, Camera.main.transform.forward).eulerAngles;
        }

        if (rotation.y > 180f) { rotation.y -= 360f; }
        cannon.GetComponent<Rigidbody>().AddTorque(0f, rotation.y, 0f);

        if (Input.GetMouseButtonDown(0))
        {
            cannon.GetComponent<Rigidbody>().AddForce(cannon.transform.up * CannonForce);
            List<GameObject> targets = cannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
            foreach (GameObject target in targets)
            {
                target.GetComponent<Rigidbody>().AddForce(-cannon.transform.up * CannonForce * 5f);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {   
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }
}
