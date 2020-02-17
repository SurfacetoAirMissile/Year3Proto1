using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperPlayerController : SkipperShared
{
    enum Weapons
    {
        WindCannon,
        Minigun
    }

    Weapons selectedWeapon;

    [SerializeField]
    protected GameObject minigunTurret;

    // 0 is front, 1 is back
    int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        Startup();
        windCannonAimMode = 0;
        if (windCannon)
        {
            selectedWeapon = Weapons.WindCannon;
        }
        else
        {
            selectedWeapon = Weapons.Minigun;
        }
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
            if (windCannon)
            {
                if (selectedWeapon == Weapons.WindCannon)
                {
                    selectedWeapon = Weapons.Minigun;
                }
                else if (selectedWeapon == Weapons.Minigun)
                {
                    selectedWeapon = Weapons.WindCannon;
                }
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
                chassisRB.AddForce(-windCannon.transform.forward * trueForce);
                windCannon.transform.GetChild(1).GetComponent<EffectSpawner>().SpawnEffect();
                List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
                foreach (GameObject target in targets)
                {
                    target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * trueForce * 3f);
                }
            }        }
        else if (selectedWeapon == Weapons.Minigun)
        {
            // point the airCannon forward
            if (windCannon)
            {
                Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
                if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
                if (Mathf.Abs(WCrotation.y) > 5f)
                {
                    windCannon.GetComponent<Rigidbody>().AddTorque(0f, Mathf.Sign(WCrotation.y) * 10f, 0f);
                }
                else
                {
                    windCannon.GetComponent<Rigidbody>().AddTorque(0f, WCrotation.y, 0f);
                }
            }
            if (minigunTurret)
            {
                Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, Camera.main.transform.forward).eulerAngles;
                if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
                if (Mathf.Abs(minigunTurretRot.y) > 5f)
                {
                    minigunTurret.GetComponent<Rigidbody>().AddTorque(0f, Mathf.Sign(minigunTurretRot.y) * 1000f, 0f);
                }
                else
                {
                    minigunTurret.GetComponent<Rigidbody>().AddTorque(0f, minigunTurretRot.y, 0f);
                }

                Transform minigunEleRing = minigunTurret.transform.GetChild(0);
                Transform minigunCannon = minigunEleRing.GetChild(0);
                
                float elevation = Vector3.Angle(-minigunEleRing.forward, Camera.main.transform.forward);

                /*
                if (miniRot.x > 180f) { miniRot.x -= 360f; }

                Vector3 offset = Vector3.forward;

                offset = Quaternion.AngleAxis(, Vector3.right) * offset;

                if (Mathf.Abs(miniRot.x) > 5f)
                {
                    minigunCannon.GetComponent<Rigidbody>().AddTorque(Mathf.Sign(miniRot.x) * 1000f, 0f, 0f);
                }
                else
                {
                    minigunCannon.GetComponent<Rigidbody>().AddTorque(miniRot.x, 0f, 0f);
                }
                */
                
                if (Input.GetMouseButton(0))
                {
                    float miniSpin = 100f;
                    minigunCannon.GetComponent<Rigidbody>().AddTorque(0f, 0f, miniSpin);
                }
            }
            // rotate the minigun towards the camera

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
