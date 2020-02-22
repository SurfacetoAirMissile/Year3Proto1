using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortoisePlayer : TortoiseShared
{
    [Header("Tortoise Player")]
    [SerializeField]
    [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    protected float windCannonForce;

    enum Weapons
    {
        Mortar,
        WindCannon,
        None
    }

    Weapons selectedWeapon;

    protected GameObject windCannon;
    protected int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        TortoiseStartup();
        selectedWeapon = Weapons.WindCannon;
        windCannonAimMode = 0;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Wind Cannon")) { windCannon = child.gameObject; }
        }
        CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
        cameraScript.cameraLookTarget = windCannon;
        cameraScript.orbitRadius = 2.5f;
        cameraScript.xRotationMin = -30f;
        cameraScript.xRotationMax = 60f;
        cameraScript.sitHeight = 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        mortarCooldown += Time.deltaTime;

        if (Input.GetKeyDown("w"))
        {
            //chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
        }
        if (Input.GetKeyUp("w"))
        {
            //chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
            if (selectedWeapon == Weapons.Mortar)
            {
                selectedWeapon = Weapons.None;
            }
            else if (selectedWeapon == Weapons.None)
            {
                selectedWeapon = Weapons.WindCannon;
            }
            else
            {
                selectedWeapon = Weapons.Mortar;
            }

            if (selectedWeapon == Weapons.Mortar)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = mortarTurret;
                cameraScript.orbitRadius = 2.5f;
                cameraScript.xRotationMin = -30f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 1.8f;
            }
            else if (selectedWeapon == Weapons.None)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = chassis;
                cameraScript.orbitRadius = 4f;
                cameraScript.xRotationMin = -30f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 0.7f;
            }
            else if (selectedWeapon == Weapons.WindCannon)
            {
                CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
                cameraScript.cameraLookTarget = windCannon;
                cameraScript.orbitRadius = 1.5f;
                cameraScript.xRotationMin = -30f;
                cameraScript.xRotationMax = 60f;
                cameraScript.sitHeight = 0.7f;
            }
        }
        if (selectedWeapon == Weapons.WindCannon)
        {
            // Mortar aim forward
            Vector3 mortarTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, chassis.transform.forward).eulerAngles;
            if (mortarTurretRot.y > 180f) { mortarTurretRot.y -= 360f; }
            StaticFunc.RotateTo(mortarTurret.GetComponent<Rigidbody>(), 'y', mortarTurretRot.y);
            if (Mathf.Abs(mortarTurretRot.y) < 15f)
            {
                Vector3 chassisForward = chassis.transform.forward;
                Vector3 barrelForward = mortarBarrel.transform.forward;
                float angle = Vector3.Angle(chassisForward, barrelForward);
                chassisForward.x = 0; chassisForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (chassisForward.y > barrelForward.y)
                {
                    angle *= -1;
                }
                StaticFunc.RotateTo(mortarBarrel.GetComponent<Rigidbody>(), 'x', angle * 0.1f);
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
            }
        }
        if (selectedWeapon == Weapons.Mortar)
        {
            // point the wind cannon backwards
            Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
            if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
            StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', WCrotation.y);

            Vector3 minigunTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, Camera.main.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(mortarTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);

            if (Mathf.Abs(minigunTurretRot.y) < 15f)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 barrelForward = mortarBarrel.transform.forward;
                float angle = Vector3.Angle(cameraForward, barrelForward);
                cameraForward.x = 0; cameraForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (cameraForward.y > barrelForward.y)
                {
                    angle *= -1;
                }

                StaticFunc.RotateTo(mortarBarrel.GetComponent<Rigidbody>(), 'x', angle * 0.1f);
            }

            if (Input.GetMouseButton(0))
            {
                if (mortarCooldown >= mortarFireDelay)
                {
                    mortarCooldown = 0f;
                    Vector3 spawnPos = mortarBarrel.transform.GetChild(0).position;
                    GameObject shellInstance = Instantiate(shellPrefab, spawnPos, Quaternion.identity);
                    Rigidbody shellRB = shellInstance.GetComponent<Rigidbody>();
                    shellRB.velocity = chassisRB.velocity;
                    shellRB.AddForce(mortarBarrel.transform.forward * 500f);
                    ShellBehaviour shellB = shellInstance.GetComponent<ShellBehaviour>();
                    shellB.SetDamage(mortarDamage);
                    shellB.SetOwner(this.gameObject);
                    shellB.explosionPrefab = explosionPrefab;
                    shellInstance.layer = 12;
                }
            }

        }
        else if (selectedWeapon == Weapons.None)
        {
            Vector3 minigunTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, chassis.transform.forward).eulerAngles;
            if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
            StaticFunc.RotateTo(mortarTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);
            if (Mathf.Abs(minigunTurretRot.y) < 15f)
            {
                Vector3 chassisForward = chassis.transform.forward;
                Vector3 barrelForward = mortarBarrel.transform.forward;
                float angle = Vector3.Angle(chassisForward, barrelForward);
                chassisForward.x = 0; chassisForward.z = 0;
                barrelForward.x = 0; barrelForward.z = 0;
                if (chassisForward.y > barrelForward.y)
                {
                    angle *= -1;
                }
                StaticFunc.RotateTo(mortarBarrel.GetComponent<Rigidbody>(), 'x', angle * 0.1f);
            }

            // point the wind cannon forward
            Vector3 WCrotation = Quaternion.FromToRotation(windCannon.transform.forward, -chassis.transform.forward).eulerAngles;
            if (WCrotation.y > 180f) { WCrotation.y -= 360f; }
            StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', WCrotation.y);

        }
    }
}
