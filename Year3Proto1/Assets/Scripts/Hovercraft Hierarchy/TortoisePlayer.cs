using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortoisePlayer : TortoiseShared
{
    [Header("Tortoise Player")]
    [SerializeField]
    [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    protected float windCannonForce;
    [SerializeField]
    [Tooltip("The Wind Cannon's rate of fire, in winds per second (not winds per minute/rpm)")]
    protected float windCannonROF;
    protected float windCannonFireDelay;
    protected float windCannonCooldown;

    public enum Weapons
    {
        Mortar,
        WindCannon
    }

    public Weapons selectedWeapon;

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
        selectedWeapon = Weapons.Mortar;
        healthComponent.SetHealth(5f);
        windCannonFireDelay = 1f / windCannonROF;
        windCannonCooldown = 0f;
        controller = ControllerType.PlayerController;
        GameManager.Instance.AddAlive(this);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;
        mortarCooldown += Time.deltaTime;
        windCannonCooldown += Time.deltaTime;

        if (GameManager.Instance.playerControl)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (selectedWeapon == Weapons.WindCannon)
                {
                    TortoiseChangeFocus(PlayerFocus.TortoiseWindCannon);
                }
                else
                {
                    TortoiseChangeFocus(PlayerFocus.TortoiseMortar);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                TortoiseChangeFocus(PlayerFocus.TortoiseNone);
            }
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
                switch (selectedWeapon)
                {
                    case Weapons.WindCannon:
                        selectedWeapon = Weapons.Mortar;
                        break;
                    case Weapons.Mortar:
                        selectedWeapon = Weapons.WindCannon;
                        break;
                }
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
                if (Input.GetMouseButtonDown(0) && GameManager.Instance.playerControl)
                {
                    // If the Wind Cannon has cooled down...
                    if (windCannonCooldown >= windCannonFireDelay)
                    {
                        FireWindCannon();
                    }
                }
                break;
            case PlayerFocus.TortoiseMortar:
                // First, rotate the Wind Cannon back to the "default position".
                YawWindCannonToTarget(chassis.transform.forward);

                // Second, aim the Mortar at the Camera.
                AimMortarAtTarget(Camera.main.GetComponent<CameraMotion>().mortarAimTarget);

                // Update the position of the impact zone
                UpdateMortarImpactZone();

                // If the Player presses the LMB...
                if (Input.GetMouseButton(0) && GameManager.Instance.playerControl)
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
                if (Input.GetMouseButtonDown(0) && GameManager.Instance.playerControl)
                {
                    // If the Wind Cannon has cooled down...
                    if (windCannonCooldown >= windCannonFireDelay)
                    {
                        FireWindCannon();
                    }
                }
                break;
        }
        if (chassisRB.velocity.magnitude > 6.5f)
        {
            GameManager.Instance.SetPlayerGoingFast(true);
        }
        else
        {
            GameManager.Instance.SetPlayerGoingFast(false);
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
                mortarImpactZone.GetComponent<MeshRenderer>().enabled = false;
                break;
            case PlayerFocus.TortoiseMortar:
                cameraScript.cameraLookTarget = mortarTurret;
                cameraScript.LoadPreset(PlayerFocus.TortoiseMortar);
                mortarBarrel.GetComponent<TrajectoryArc>().enabled = true;
                mortarBarrel.GetComponent<LineRenderer>().enabled = true;
                mortarImpactZone.GetComponent<MeshRenderer>().enabled = true;
                break;
            case PlayerFocus.TortoiseWindCannon:
                cameraScript.cameraLookTarget = windCannon;
                cameraScript.LoadPreset(PlayerFocus.TortoiseWindCannon);
                mortarBarrel.GetComponent<TrajectoryArc>().enabled = false;
                mortarBarrel.GetComponent<LineRenderer>().enabled = false;
                mortarImpactZone.GetComponent<MeshRenderer>().enabled = false;
                break;
            default:
                Debug.Log("Tortoise Player can't focus on a non-Tortoise part.");
                break;
        }
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
        windCannonCooldown = 0f;
        float trueForce = windCannonForce * 1000f;
        windCannon.GetComponent<Rigidbody>().AddForce(-windCannon.transform.forward * trueForce);
        windCannon.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        windCannon.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        windCannon.GetComponent<AudioSource>().Play();
        List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
        foreach (GameObject target in targets)
        {
            target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * trueForce * 5f);
        }
    }

    void UpdateMortarImpactZone()
    {
        LineRenderer line = mortarBarrel.GetComponent<LineRenderer>();
        float shortestDistance = Mathf.Infinity;
        int segmentClosest = 0;
        // Find the closest segment
        for (int i = 0; i < line.positionCount; i++)
        {
            Vector3 lineIposition = line.GetPosition(i);
            if (Physics.Raycast(lineIposition, Vector3.down, out RaycastHit cast))
            {
                if (cast.distance < shortestDistance && i > 5)
                {
                    segmentClosest = i;
                    shortestDistance = cast.distance;
                }
            }
        }

        Vector3 groundPosition = line.GetPosition(segmentClosest);
        groundPosition.y -= shortestDistance;
        mortarImpactZone.transform.position = groundPosition;
    }


    float StepBack(LineRenderer _line, int _segment)
    {
        if (_segment - 1 >= 0)
        {
            Vector3 segmentPosition = _line.GetPosition(_segment - 1);
            if (Physics.Raycast(segmentPosition, Vector3.down, out RaycastHit cast))
            {
                return cast.distance;
            }
        }

        return Mathf.Infinity;
    }

    float StepForward(LineRenderer _line, int _segment)
    {
        if (_line.positionCount >= _segment + 1)
        {
            Vector3 segmentPosition = _line.GetPosition(_segment + 1);
            if (Physics.Raycast(segmentPosition, Vector3.down, out RaycastHit cast))
            {
                return cast.distance;
            }
        }

        return Mathf.Infinity;
    }
}
