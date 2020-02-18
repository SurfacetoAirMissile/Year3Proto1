using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperPlayerController : SkipperShared
{
    enum Weapons
    {
        WindCannon,
        Minigun,
        None
    }

    Weapons selectedWeapon;

    [SerializeField]
    protected GameObject minigunTurret;

    [SerializeField]
    protected GameObject minigunElevationRing;

    [SerializeField]
    protected GameObject minigunCannon;

    [SerializeField]
    protected float minigunROF;
    protected float minigunFireDelay;
    protected float minigunCooldown;

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
            CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
            cameraScript.cameraLookTarget = minigunTurret;
            cameraScript.orbitRadius = 1f;
            cameraScript.xRotationMin = -30f;
            cameraScript.xRotationMax = 60f;
            cameraScript.sitHeight = 0.5f;
            minigunFireDelay = 1f / minigunROF;
            minigunCooldown = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();

        float pushAmount = Time.deltaTime * 1000f * pushForce;
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        if (Input.GetKeyDown("w"))
        {
            chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
        }
        if (Input.GetKeyUp("w"))
        {
            chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
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
                selectedWeapon = Weapons.Minigun;
            }
            else if (selectedWeapon == Weapons.Minigun)
            {
                selectedWeapon = Weapons.None;
            }
            else
            {
                selectedWeapon = windCannon ? Weapons.WindCannon : Weapons.Minigun;
            }
            if (selectedWeapon == Weapons.Minigun)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = minigunTurret;
                cameraScript.orbitRadius = 1f;
                cameraScript.xRotationMin = -30f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 0.5f;
            }
            else if (selectedWeapon == Weapons.None)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = chassis;
                cameraScript.orbitRadius = 4f;
                cameraScript.xRotationMin = -20f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 0f;
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
            }
            else
            {
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
        else if (selectedWeapon == Weapons.Minigun)
        {
            minigunCooldown += Time.deltaTime;
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

                /*
                //float elevation = Vector3.Angle(-minigunCannon.transform.forward, Camera.main.transform.forward);
                float elevation = Vector3.Angle(-minigunElevationRing.transform.forward, Camera.main.transform.forward);
                if (Camera.main.transform.forward.y > -minigunCannon.transform.forward.y)
                {
                    elevation *= -1f;
                }
                Debug.Log(elevation);
                Debug.DrawRay(minigunCannon.transform.position, -minigunTurret.transform.forward, Color.red);
                Debug.DrawRay(minigunCannon.transform.position, -minigunCannon.transform.forward, Color.green);
                Debug.DrawRay(minigunCannon.transform.position, Camera.main.transform.forward, Color.blue);
                */


                //Vector3.RotateTowards(-minigunCannon.transform.forward, )
                if (Mathf.Abs(minigunTurretRot.y) < 90f)
                {
                    Vector3 rot = Quaternion.FromToRotation(-minigunCannon.transform.forward, Camera.main.transform.forward).eulerAngles;
                    if (rot.x > 180f) { rot.x -= 360f; }

                    if (Mathf.Abs(rot.x) > 2f)
                    {
                        minigunElevationRing.GetComponent<Rigidbody>().AddTorque(Mathf.Sign(rot.x) * 100f, 0f, 0f);
                    }
                    else
                    {
                        minigunElevationRing.GetComponent<Rigidbody>().AddTorque(Mathf.Sign(rot.x) * 10f, 0f, 0f);
                    }

                }

                if (Input.GetMouseButton(0))
                {
                    if (minigunCooldown >= minigunFireDelay)
                    {
                        minigunCooldown = 0f;
                        Vector3 spawnPos = minigunCannon.transform.GetChild(0).position;
                        GameObject bulletInstance = Instantiate(bullet, spawnPos, Quaternion.identity);
                        bulletInstance.GetComponent<Rigidbody>().AddForce(-minigunCannon.transform.forward * 5f);
                    }
                    //float miniSpin = 100f;
                    //minigunCannon.GetComponent<Rigidbody>().AddTorque(0f, 0f, miniSpin);
                }

            }

        }
        else if (selectedWeapon == Weapons.None)
        {
            if (minigunTurret)
            {
                Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, -chassis.transform.forward).eulerAngles;
                if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
                if (Mathf.Abs(minigunTurretRot.y) > 5f)
                {
                    minigunTurret.GetComponent<Rigidbody>().AddTorque(0f, Mathf.Sign(minigunTurretRot.y) * 1000f, 0f);
                }
                else
                {
                    minigunTurret.GetComponent<Rigidbody>().AddTorque(0f, minigunTurretRot.y, 0f);
                }

                if (Mathf.Abs(minigunTurretRot.y) < 90f)
                {
                    Vector3 rot = Quaternion.FromToRotation(-minigunCannon.transform.forward, -chassis.transform.forward).eulerAngles;
                    if (rot.x > 180f) { rot.x -= 360f; }
                    if (Mathf.Abs(rot.x) > 2f)
                    {
                        minigunElevationRing.GetComponent<Rigidbody>().AddTorque(Mathf.Sign(rot.x) * 100f, 0f, 0f);
                    }
                    else
                    {
                        minigunElevationRing.GetComponent<Rigidbody>().AddTorque(Mathf.Sign(rot.x) * 10f, 0f, 0f);
                    }
                }
            }
        }
    }
}
