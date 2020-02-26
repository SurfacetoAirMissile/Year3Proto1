using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarPlayer : ScimitarShared
{
    [Header("Scimitar Player")]
    [SerializeField]
    [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    protected float windCannonForce;

    enum Weapons
    {
        Minigun,
        WindCannon,
        None
    }

    PlayerFocus playerFocus;

    protected GameObject windCannon;
    protected int windCannonAimMode;

    // Start is called before the first frame update
    void Start()
    {
        ScimitarStartup();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Wind Cannon")) { windCannon = child.gameObject; }
        }
        windCannonAimMode = 0;
        ScimitarChangeFocus(PlayerFocus.ScimitarNone);
        healthComponent.SetHealth(3f);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        minigunCooldown += Time.deltaTime;

        if (GameManager.Instance.playerControl)
        {
            if (Input.GetKeyDown("left ctrl"))
            {
                windCannonAimMode = windCannonAimMode == 0 ? 1 : 0;
            }
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
                switch (playerFocus)
                {
                    case PlayerFocus.ScimitarNone:
                        ScimitarChangeFocus(PlayerFocus.ScimitarMinigun);
                        break;
                    case PlayerFocus.ScimitarMinigun:
                        ScimitarChangeFocus(PlayerFocus.ScimitarWindCannon);
                        break;
                    case PlayerFocus.ScimitarWindCannon:
                        ScimitarChangeFocus(PlayerFocus.ScimitarNone);
                        break;
                    default:
                        Debug.Log("ScimitarPlayer playerFocus shouldn't be non-Scimitar");
                        break;
                }
            }
        }
        switch (playerFocus)
        {
            case PlayerFocus.ScimitarNone:
                // First, rotate the Minigun back to the "default position".
                AimMinigunAtTarget(chassis.transform.forward);

                // point the wind cannon forward
                YawWindCannonToTarget(chassis.transform.forward);

                // If the Player presses the LMB...
                if (Input.GetMouseButtonDown(0) && GameManager.Instance.playerControl)
                {
                    FireWindCannon();
                }
                break;
            case PlayerFocus.ScimitarMinigun:
                // First, rotate the Wind Cannon back to the "default position".
                YawWindCannonToTarget(chassis.transform.forward);

                // Second, aim the Mortar at the Camera.
                AimMinigunAtTarget(Camera.main.transform.forward);

                // If the Player presses the LMB...
                if (Input.GetMouseButton(0) && GameManager.Instance.playerControl)
                {
                    // If the Mortar has cooled down...
                    if (minigunCooldown >= minigunFireDelay)
                    {
                        FireMinigun();
                    }
                }
                break;
            case PlayerFocus.ScimitarWindCannon:
                // First, rotate the Mortar back to the "default position".
                AimMinigunAtTarget(chassis.transform.forward);

                // Second, aim the Wind Cannon at the Camera.
                YawWindCannonToTarget(Camera.main.transform.forward);

                // If the Player presses the LMB...
                if (Input.GetMouseButtonDown(0) && GameManager.Instance.playerControl)
                {
                    FireWindCannon();
                }
                break;
        }
        if (chassisRB.velocity.magnitude > 7f)
        {
            GameManager.Instance.SetPlayerGoingFast(true);
        }
        else
        {
            GameManager.Instance.SetPlayerGoingFast(false);
        }
    }

    protected void ScimitarChangeFocus(PlayerFocus _playerFocus)
    {
        CameraMotion cameraScript = Camera.main.GetComponent<CameraMotion>();
        playerFocus = _playerFocus;
        switch (_playerFocus)
        {
            case PlayerFocus.ScimitarNone:
                cameraScript.cameraLookTarget = chassis;
                cameraScript.LoadPreset(PlayerFocus.ScimitarNone);
                break;
            case PlayerFocus.ScimitarMinigun:
                cameraScript.cameraLookTarget = minigunTurret;
                cameraScript.LoadPreset(PlayerFocus.ScimitarMinigun);
                break;
            case PlayerFocus.ScimitarWindCannon:
                cameraScript.cameraLookTarget = windCannon;
                cameraScript.LoadPreset(PlayerFocus.ScimitarWindCannon);
                break;
            default:
                Debug.Log("Scimitar Player can't focus on a non-Scimitar part.");
                break;
        }
    }

    void PitchMinigunToTarget(Vector3 _targetDirection)
    {
        Vector3 barrelForward = -minigunElevationRing.transform.forward;
        Debug.DrawRay(minigunTurret.transform.position, barrelForward);
        Debug.DrawRay(minigunTurret.transform.position, _targetDirection);
        _targetDirection.x = 1; _targetDirection.z = 1;
        barrelForward.x = 1; barrelForward.z = 1;
        float angle = Vector3.Angle(_targetDirection, barrelForward);
        if (_targetDirection.y < barrelForward.y) { angle *= -1; }
        StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
    }

    void YawMinigunToTarget(Vector3 _targetDirection)
    {
        Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, _targetDirection).eulerAngles;
        if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
        StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);
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
        windCannon.GetComponent<AudioSource>().Play();
        List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
        foreach (GameObject target in targets)
        {
            target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * trueForce * 5f);
        }
    }

    void FireMinigun()
    {
        minigunCooldown = 0f;
        minigunBarrel.GetComponent<AudioSource>().Play();
        Vector3 spawnPos = minigunBarrel.transform.GetChild(0).position;
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody bulletRB = bulletInstance.GetComponent<Rigidbody>();
        bulletRB.velocity = chassisRB.velocity;
        bulletRB.AddForce(-minigunBarrel.transform.forward * 5f);
        BulletBehaviour bulletB = bulletInstance.GetComponent<BulletBehaviour>();
        bulletB.SetDamage(minigunDamage);
        bulletB.SetOwner(this.gameObject);
        bulletInstance.layer = 12;
    }

    void AimMinigunAtTarget(Vector3 _targetDirection)
    {
        // Mortar Turret Rotation
        YawMinigunToTarget(_targetDirection);
        PitchMinigunToTarget(_targetDirection);
    }
}
