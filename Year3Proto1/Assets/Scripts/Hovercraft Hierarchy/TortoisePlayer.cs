using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortoisePlayer : TortoiseShared
{
    [Header("Tortoise Player")]
    [SerializeField]
    [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    protected float windCannonForce;

    PlayerFocus playerFocus;

    protected GameObject windCannon;
    protected int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        TortoiseStartup();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Wind Cannon")) { windCannon = child.gameObject; }
        }
        windCannonAimMode = 0;
        playerFocus = PlayerFocus.TortoiseWindCannon;
        TortoiseChangeFocus(playerFocus);
        healthComponent.SetHealth(5f);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        mortarCooldown += Time.deltaTime;


        if (Input.GetKeyDown("left ctrl"))
        {
            windCannonAimMode = windCannonAimMode == 0 ? 1 : 0;
        }
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
            switch (playerFocus)
            {
                case PlayerFocus.TortoiseMortar:
                    TortoiseChangeFocus(PlayerFocus.TortoiseNone);
                    break;
                case PlayerFocus.TortoiseNone:
                    TortoiseChangeFocus(PlayerFocus.TortoiseWindCannon);
                    break;
                case PlayerFocus.TortoiseWindCannon:
                    TortoiseChangeFocus(PlayerFocus.TortoiseMortar);
                    break;
                default:
                    Debug.Log("TortoisePlayer playerFocus shouldn't be non-Tortoise");
                    break;
            }
        }
        switch (playerFocus)
        {
            case PlayerFocus.TortoiseNone:
                // First, rotate the Mortar back to the "default position".
                AimMortarAtTarget(chassis.transform.forward);

                // point the wind cannon forward
                YawWindCannonToTarget(chassis.transform.forward);

                // If the Player presses the LMB...
                if (Input.GetMouseButtonDown(0))
                {
                    FireWindCannon();
                }
                break;
            case PlayerFocus.TortoiseMortar:
                // First, rotate the Wind Cannon back to the "default position".
                YawWindCannonToTarget(chassis.transform.forward);

                // Second, aim the Mortar at the Camera.
                AimMortarAtTarget(Camera.main.GetComponent<CameraMotion>().mortarAimTarget);

                // If the Player presses the LMB...
                if (Input.GetMouseButton(0))
                {
                    // If the Mortar has cooled down...
                    if (mortarCooldown >= mortarFireDelay)
                    {
                        FireMortar();
                    }
                }
                break;
            case PlayerFocus.TortoiseWindCannon:
                // First, rotate the Mortar back to the "default position".
                AimMortarAtTarget(chassis.transform.forward);

                // Second, aim the Wind Cannon at the Camera.
                YawWindCannonToTarget(Camera.main.transform.forward);

                // If the Player presses the LMB...
                if (Input.GetMouseButtonDown(0))
                {
                    FireWindCannon();
                }
                break;
        }
    }

    protected void TortoiseChangeFocus(PlayerFocus _playerFocus)
    {
        CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
        playerFocus = _playerFocus;
        switch (_playerFocus)
        {
            case PlayerFocus.TortoiseNone:
                cameraScript.cameraLookTarget = chassis;
                cameraScript.LoadPreset(PlayerFocus.TortoiseNone);
                mortarBarrel.GetComponent<TrajectoryArc>().enabled = false;
                mortarBarrel.GetComponent<LineRenderer>().enabled = false;
                break;
            case PlayerFocus.TortoiseMortar:
                cameraScript.cameraLookTarget = mortarTurret;
                cameraScript.LoadPreset(PlayerFocus.TortoiseMortar);
                mortarBarrel.GetComponent<TrajectoryArc>().enabled = true;
                mortarBarrel.GetComponent<LineRenderer>().enabled = true;
                break;
            case PlayerFocus.TortoiseWindCannon:
                cameraScript.cameraLookTarget = windCannon;
                cameraScript.LoadPreset(PlayerFocus.TortoiseWindCannon);
                mortarBarrel.GetComponent<TrajectoryArc>().enabled = false;
                mortarBarrel.GetComponent<LineRenderer>().enabled = false;
                break;
            default:
                Debug.Log("Tortoise Player can't focus on a non-Tortoise part.");
                break;
        }
    }

    void PitchMortarToTarget(Vector3 _targetDirection)
    {
        Vector3 barrelForward = mortarBarrel.transform.forward;
        _targetDirection.x = 1; _targetDirection.z = 1;
        barrelForward.x = 1; barrelForward.z = 1;
        float angle = Vector3.Angle(_targetDirection, barrelForward);
        if (_targetDirection.y > barrelForward.y) { angle *= -1; }
        StaticFunc.RotateTo(mortarBarrel.GetComponent<Rigidbody>(), 'x', angle * 0.025f);
    }

    void YawMortarToTarget(Vector3 _targetDirection)
    {
        Vector3 mortarTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, _targetDirection).eulerAngles;
        if (mortarTurretRot.y > 180f) { mortarTurretRot.y -= 360f; }
        StaticFunc.RotateTo(mortarTurret.GetComponent<Rigidbody>(), 'y', mortarTurretRot.y * 0.5f);
    }

    void YawWindCannonToTarget(Vector3 _targetDirection)
    {
        Vector3 rotation;
        if (windCannonAimMode == 0)
        {
            rotation = Quaternion.FromToRotation(windCannon.transform.forward, _targetDirection).eulerAngles;
        }
        else
        {
            rotation = Quaternion.FromToRotation(windCannon.transform.forward, -_targetDirection).eulerAngles;
        }
        if (rotation.y > 180f) { rotation.y -= 360f; }
        StaticFunc.RotateTo(windCannon.GetComponent<Rigidbody>(), 'y', rotation.y);
    }

    void FireWindCannon()
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

    void FireMortar()
    {
        mortarCooldown = 0f;
        Vector3 spawnPos = mortarBarrel.transform.GetChild(0).position;
        GameObject shellInstance = Instantiate(shellPrefab, spawnPos, Quaternion.identity);
        Rigidbody shellRB = shellInstance.GetComponent<Rigidbody>();
        shellRB.velocity = chassisRB.velocity;
        shellRB.AddForce(mortarBarrel.transform.forward * 1500f);
        ShellBehaviour shellB = shellInstance.GetComponent<ShellBehaviour>();
        shellB.SetDamage(mortarDamage);
        shellB.SetOwner(this.gameObject);
        shellB.explosionPrefab = explosionPrefab;
        shellInstance.layer = 12;
    }

    void AimMortarAtTarget(Vector3 _targetDirection)
    {
        // Mortar Turret Rotation
        YawMortarToTarget(_targetDirection);
        PitchMortarToTarget(_targetDirection);
    }

}
