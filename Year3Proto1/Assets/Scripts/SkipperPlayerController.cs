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

    //Weapons selectedWeapon;

    // 0 is left, 1 is right
    int autogunToFire;

    // Start is called before the first frame update
    void Start()
    {
        Startup();
        //selectedWeapon = Weapons.WindCannon;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();

        float pushAmount = Time.deltaTime * PushForce;
        float rotationAmount = Time.deltaTime * RotationForce;

        if (Input.GetKey("w"))
        {
            chassisRB.AddForce(pushAmount * -chassis.transform.forward);
        }
        if (Input.GetKey("s"))
        {
            chassisRB.AddForce(pushAmount * chassis.transform.forward);
        }
        if (Input.GetKey("e"))
        {
            chassisRB.AddForce(pushAmount * -chassis.transform.right);
        }
        if (Input.GetKey("q"))
        {
            chassisRB.AddForce(pushAmount * chassis.transform.right);
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
            /*
            if (selectedWeapon == Weapons.WindCannon)
            {
                selectedWeapon = Weapons.Autoguns;
            }
            else if (selectedWeapon == Weapons.Autoguns)
            {
                selectedWeapon = Weapons.WindCannon;
            }
            */
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

        //if (selectedWeapon == Weapons.WindCannon)
        //{
            Vector3 rotation;
            if (Input.GetKey("left ctrl"))
            {
                rotation = Quaternion.FromToRotation(-windCannon.transform.forward, Camera.main.transform.forward).eulerAngles;
            }
            else
            {
                rotation = Quaternion.FromToRotation(windCannon.transform.forward, Camera.main.transform.forward).eulerAngles;
            }

            if (rotation.y > 180f) { rotation.y -= 360f; }
            windCannon.GetComponent<Rigidbody>().AddTorque(0f, rotation.y, 0f);

            if (Input.GetMouseButtonDown(0))
            {
                windCannon.GetComponent<Rigidbody>().AddForce(-windCannon.transform.forward * CannonForce);
                List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
                foreach (GameObject target in targets)
                {
                    target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * CannonForce * 5f);
                }
            }
        //}
        /*
        else if (selectedWeapon == Weapons.Autoguns)
        {
            if (Input.GetMouseButtonDown(0))
            {
                autoguns[autogunToFire].transform.GetChild(1).GetComponent<Rigidbody>().AddForce(autoguns[autogunToFire].transform.up * 50f);
                GameObject bulletInstance = Instantiate(bullet, autoguns[autogunToFire].transform.GetChild(0).position, Quaternion.identity);
                bulletInstance.GetComponent<Rigidbody>().AddForce(-autoguns[autogunToFire].transform.up * 5f);
                autogunToFire = (autogunToFire == 0) ? 1 : 0;
            }
        }
        */
        


    }
}
