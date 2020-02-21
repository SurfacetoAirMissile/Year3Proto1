using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarPlayer : ScimitarShared
{
    [Header("Scimitar Player")]
    [SerializeField] [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    protected float windCannonForce;

    enum Weapons
    {
        Minigun,
        WindCannon,
        None
    }

    Weapons selectedWeapon;

    protected GameObject windCannon;
    protected int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        ScimitarStartup();
        selectedWeapon = Weapons.WindCannon;
        windCannonAimMode = 0;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Wind Cannon")) { windCannon = child.gameObject; }
        }
        CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
        cameraScript.cameraLookTarget = windCannon;
        cameraScript.orbitRadius = 1f;
        cameraScript.xRotationMin = -30f;
        cameraScript.xRotationMax = 60f;
        cameraScript.sitHeight = 0.5f;
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
            Thrust(chassis.transform.forward, 1f);
        }
        if (Input.GetKey("s"))
        {
            Thrust(-chassis.transform.forward, 1f);
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
            else if (selectedWeapon == Weapons.None)
            {
                selectedWeapon = Weapons.WindCannon;
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
            else if (selectedWeapon == Weapons.WindCannon)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = windCannon;
                cameraScript.orbitRadius = 1f;
                cameraScript.xRotationMin = -30f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 0.5f;
            }
        }
        if (selectedWeapon == Weapons.WindCannon)
        {
            Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, chassis.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);
            if (Mathf.Abs(minigunTurretRot.y) < 15f)
            {
                Vector3 chassisForward = chassis.transform.forward;
                Vector3 barrelForward = -minigunElevationRing.transform.forward;
                float angle = Vector3.Angle(chassisForward, barrelForward);
                chassisForward.x = 0; chassisForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (chassisForward.y < barrelForward.y)
                {
                    angle *= -1;
                }
                StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
            }

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
                rotation = Quaternion.FromToRotation(windCannon.transform.forward, -Camera.main.transform.forward).eulerAngles;
            }

            if (rotation.y > 180f) { rotation.y -= 360f; }
            StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', rotation.y);

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
        if (selectedWeapon == Weapons.Minigun)
        {
            // point the wind cannon forward
            Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
            if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
            StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', WCrotation.y);

            Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, Camera.main.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);

            if (Mathf.Abs(minigunTurretRot.y) < 15f)
            {
                // if the z rotation is higher, rotate calculation vectors by 90 degrees on the y axis
                /*
                Vector3 rot = Quaternion.FromToRotation(-minigunBarrel.transform.forward, Camera.main.transform.forward).eulerAngles;
                if (rot.x > 180f) { rot.x -= 360f; }
                if (rot.y > 180f) { rot.y -= 360f; }
                if (rot.z > 180f) { rot.z -= 360f; }
                */

                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 barrelForward = -minigunElevationRing.transform.forward;
                float angle = Vector3.Angle(cameraForward, barrelForward);
                cameraForward.x = 0; cameraForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (cameraForward.y < barrelForward.y)
                {
                    angle *= -1;
                }

                StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
                /*
                Debug.DrawLine(minigunBarrel.transform.position, minigunBarrel.transform.position + new Vector3(rot.x / 45f, 0f, 0f), Color.red);
                Debug.DrawLine(minigunBarrel.transform.position, minigunBarrel.transform.position + new Vector3(0f, rot.y / 45f, 0f), Color.green);
                Debug.DrawLine(minigunBarrel.transform.position, minigunBarrel.transform.position + new Vector3(0f, 0f, rot.z / 45f), Color.blue);
                */
                //StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', rot.x);
                /*
                float minigunEleRingRotX = minigunElevationRing.transform.rotation.eulerAngles.x;
                if (minigunEleRingRotX > 180f) { minigunEleRingRotX -= 360f; };
                if (minigunEleRingRotX < minigunMaximumDepression)
                {
                    StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', -2 * rot.x);
                }
                */
            }

            if (Input.GetMouseButton(0))
            {
                if (minigunCooldown >= minigunFireDelay)
                {
                    minigunCooldown = 0f;
                    Vector3 spawnPos = minigunBarrel.transform.GetChild(0).position;
                    GameObject bulletInstance = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                    Rigidbody bulletRB = bulletInstance.GetComponent<Rigidbody>();
                    bulletRB.velocity = chassisRB.velocity;
                    bulletRB.AddForce(-minigunBarrel.transform.forward * 5f);
                    BulletBehaviour bulletB = bulletInstance.GetComponent<BulletBehaviour>();
                    bulletB.SetDamage(minigunDamage);
                    bulletB.SetOwner(this.gameObject);
                }

                // TODO MINIGUN CANNON SPINNING
                //float miniSpin = 100f;
                //minigunCannon.GetComponent<Rigidbody>().AddTorque(0f, 0f, miniSpin);
            }

        }
        else if (selectedWeapon == Weapons.None)
        {
            Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, chassis.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);
            if (Mathf.Abs(minigunTurretRot.y) < 15f)
            {
                Vector3 chassisForward = chassis.transform.forward;
                Vector3 barrelForward = -minigunElevationRing.transform.forward;
                float angle = Vector3.Angle(chassisForward, barrelForward);
                chassisForward.x = 0; chassisForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (chassisForward.y < barrelForward.y)
                {
                    angle *= -1;
                }
                StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
            }

            // point the wind cannon forward
            Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
            if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
            StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', WCrotation.y);

        }
    }
}
