using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperPlayerController : SkipperShared
{
    enum Weapons
    {
        WindCannon,
        Autoguns
    }

    Weapons selectedWeapon;

    // 0 is left, 1 is right
    int autogunToFire;

    // 0 is front, 1 is back
    int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        Startup();
        windCannonAimMode = 0;
        selectedWeapon = Weapons.WindCannon;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();

        float pushAmount = Time.deltaTime * 1000f * pushForce;
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;

        if (Input.GetKey("w"))
        {
            Thrust(-chassis.transform.forward, 1f);
        }
        if (Input.GetKey("s"))
        {
            Thrust(chassis.transform.forward, 1f);
        }
        if (Input.GetKey("e"))
        {
            Thrust(-chassis.transform.right, 1f);
        }
        if (Input.GetKey("q"))
        {
            Thrust(chassis.transform.right, 1f);
        }
        if (Input.GetKey("d"))
        {
            chassisRB.AddTorque(0f, rotationAmount, 0f);
        }
        if (Input.GetKey("a"))
        {
            chassisRB.AddTorque(0f, -rotationAmount, 0f);
        }
        if (Input.GetKeyDown("tab"))
        {
            if (selectedWeapon == Weapons.WindCannon)
            {
                selectedWeapon = Weapons.Autoguns;
            }
            else if (selectedWeapon == Weapons.Autoguns)
            {
                selectedWeapon = Weapons.WindCannon;
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
        if (selectedWeapon == Weapons.WindCannon)
        {
            Vector3 rotation;
            if (Input.GetKeyDown("left ctrl"))
            {
                windCannonAimMode = windCannonAimMode == 0 ? 1 : 0;
            }

            if (windCannonAimMode == 0)
            {
                rotation = Quaternion.FromToRotation(windCannon.transform.forward, Camera.main.transform.forward).eulerAngles;
            }
            else
            {
                rotation = Quaternion.FromToRotation(-windCannon.transform.forward, Camera.main.transform.forward).eulerAngles;
            }

            if (rotation.y > 180f) { rotation.y -= 360f; }
            if (Mathf.Abs(rotation.y) > 5f)
            {
                windCannon.GetComponent<Rigidbody>().AddTorque(0f, Mathf.Sign(rotation.y) * 10f, 0f);
            }            else            {
                windCannon.GetComponent<Rigidbody>().AddTorque(0f, rotation.y, 0f);
            }
            if (Input.GetMouseButtonDown(0))
            {
                float trueForce = windCannonForce * 1000f;
                windCannon.GetComponent<Rigidbody>().AddForce(-windCannon.transform.forward * trueForce);
                windCannon.transform.GetChild(1).GetComponent<EffectSpawner>().SpawnEffect();
                List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
                foreach (GameObject target in targets)
                {
                    target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * trueForce * 5f);
                }
            }        }
        else if (selectedWeapon == Weapons.Autoguns)
        {
            // point the airCannon forward
            Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
            if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
            if (Mathf.Abs(WCrotation.y) > 5f)
            {
                windCannon.GetComponent<Rigidbody>().AddTorque(0f, Mathf.Sign(WCrotation.y) * 10f, 0f);
            }            else            {
                windCannon.GetComponent<Rigidbody>().AddTorque(0f, WCrotation.y, 0f);
            }
            /*
            if (Input.GetMouseButtonDown(0))
            {
                autoguns[autogunToFire].transform.GetChild(1).GetComponent<Rigidbody>().AddForce(autoguns[autogunToFire].transform.up * 50f);
                GameObject bulletInstance = Instantiate(bullet, autoguns[autogunToFire].transform.GetChild(0).position, Quaternion.identity);
                bulletInstance.GetComponent<Rigidbody>().AddForce(-autoguns[autogunToFire].transform.up * 5f);
                autogunToFire = (autogunToFire == 0) ? 1 : 0;
            }
            */
        }
    }
}
