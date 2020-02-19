using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarPlayer : ScimitarShared
{
    enum Weapons
    {
        Minigun,
        None
    }

    Weapons selectedWeapon;

    // Start is called before the first frame update
    void Start()
    {
        ScimitarStartup();
    }

    // Update is called once per frame
    void Update()
    {

        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        minigunCooldown += Time.deltaTime;

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
            if (selectedWeapon == Weapons.Minigun)
            {
                selectedWeapon = Weapons.None;
            }
            else
            {
                selectedWeapon = Weapons.Minigun;
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
        /*
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
        */
        if (selectedWeapon == Weapons.Minigun)
        {
            // point the airCannon forward
            /*
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
            */
            Debug.DrawLine(minigunTurret.transform.position, -minigunTurret.transform.forward, Color.red);
            Debug.DrawLine(minigunTurret.transform.position, Camera.main.transform.forward, Color.blue);
            Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, Camera.main.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);

            if (Mathf.Abs(minigunTurretRot.y) < 90f)
            {
                // if the z rotation is higher, rotate calculation vectors by 90 degrees on the y axis
                Vector3 rot = Quaternion.FromToRotation(-minigunBarrel.transform.forward, Camera.main.transform.forward).eulerAngles;
                if (rot.z > rot.x)
                {
                    float temp = rot.z; rot.z = rot.x; rot.x = temp;
                }
                if (rot.x > 180f) { rot.x -= 360f; }
                StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', rot.x);
                float minigunEleRingRotX = minigunElevationRing.transform.rotation.eulerAngles.x;
                if (minigunEleRingRotX > 180f) { minigunEleRingRotX -= 360f; };
                if (minigunEleRingRotX < minigunMaximumDepression)
                {
                    StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', -2 * rot.x);
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (minigunCooldown >= minigunFireDelay)
                {
                    minigunCooldown = 0f;
                    Vector3 spawnPos = minigunBarrel.transform.GetChild(0).position;
                    GameObject bulletInstance = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                    bulletInstance.GetComponent<Rigidbody>().AddForce(-minigunBarrel.transform.forward * 5f);
                }

                // TODO MINIGUN CANNON SPINNING
                //float miniSpin = 100f;
                //minigunCannon.GetComponent<Rigidbody>().AddTorque(0f, 0f, miniSpin);
            }

        }
        else if (selectedWeapon == Weapons.None)
        {
            Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, -chassis.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);

            if (Mathf.Abs(minigunTurretRot.y) < 90f)
            {
                Vector3 rot = Quaternion.FromToRotation(-minigunBarrel.transform.forward, -chassis.transform.forward).eulerAngles;
                if (rot.x > 180f) { rot.x -= 360f; }
                StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', rot.x);
            }
        }
    }
}
