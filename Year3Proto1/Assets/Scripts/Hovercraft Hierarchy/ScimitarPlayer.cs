using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarPlayer : ScimitarShared
{
    [Header("Scimitar Player")]
    [SerializeField]
    [Tooltip("The amount of force the wind cannon applies, in thousands of units.")]
    public float windCannonForce;
    [SerializeField]
    [Tooltip("The Wind Cannon's rate of fire, in winds per second (not winds per minute/rpm)")]
    protected float windCannonROF;
    protected float windCannonFireDelay;
    protected float windCannonCooldown;

    public enum Weapons
    {
        Minigun,
        WindCannon
    }

    public Weapons selectedWeapon;

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
        selectedWeapon = Weapons.Minigun;
        healthComponent.SetHealth(3f, true);
        windCannonFireDelay = 1f / windCannonROF;
        windCannonCooldown = 0f;
        controller = ControllerType.PlayerController;
        GameManager.Instance.AddAlive(this);
    }

    // Update is called once per frame
    void Update()
    {
        HovercraftSharedUpdate();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce * (installedUpgrades.Contains(UpgradeType.ENGINE_SPEED) ? 1.5f : 1f);
        minigunCooldown += Time.deltaTime;
        windCannonCooldown += Time.deltaTime;
        float thrustValue = installedUpgrades.Contains(UpgradeType.ENGINE_SPEED) ? 1.5f : 1f;
        if (GameManager.Instance.GetPlayerControl())
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (selectedWeapon == Weapons.WindCannon)
                {
                    playerAiming = true;
                    ScimitarChangeFocus(PlayerFocus.ScimitarWindCannon);
                }
                else
                {
                    playerAiming = true;
                    ScimitarChangeFocus(PlayerFocus.ScimitarMinigun);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                playerAiming = false;
                ScimitarChangeFocus(PlayerFocus.ScimitarNone);
            }
            if (Input.GetKeyDown("left ctrl"))
            {
                windCannonAimMode = windCannonAimMode == 0 ? 1 : 0;
            }
            if (Input.GetKeyDown("h"))
            {
                healthComponent.RestoreToFull();
            }
            if (Input.GetKeyDown("g"))
            {
                GameManager.Instance.playerScrap += 1000;
            }
            if (Input.GetKey("w"))
            {
                Thrust(chassis.transform.forward, thrustValue);
                ThrustParticleEffect(true);
            }
            else
            {
                ThrustParticleEffect(false);
            }
            if (Input.GetKey("s"))
            {
                Thrust(-chassis.transform.forward, thrustValue);
            }
            if (Input.GetKey("e"))
            {
                Thrust(chassis.transform.right, thrustValue);
            }
            if (Input.GetKey("q"))
            {
                Thrust(-chassis.transform.right, thrustValue);
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
                        selectedWeapon = Weapons.Minigun;
                        if (Input.GetMouseButton(1))
                        {
                            playerAiming = true;
                            ScimitarChangeFocus(PlayerFocus.ScimitarMinigun);
                        }
                        break;
                    case Weapons.Minigun:
                        selectedWeapon = Weapons.WindCannon;
                        if (Input.GetMouseButton(1))
                        {
                            playerAiming = true;
                            ScimitarChangeFocus(PlayerFocus.ScimitarWindCannon);
                        }
                        break;
                }
            }
            if (Input.GetKeyDown("1"))
            {
                selectedWeapon = Weapons.Minigun;
                if (Input.GetMouseButton(1))
                {
                    playerAiming = true;
                    ScimitarChangeFocus(PlayerFocus.ScimitarMinigun);
                }
            }
            if (Input.GetKeyDown("2"))
            {
                selectedWeapon = Weapons.WindCannon;
                if (Input.GetMouseButton(1))
                {
                    playerAiming = true;
                    ScimitarChangeFocus(PlayerFocus.ScimitarWindCannon);
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
                if (Input.GetMouseButton(0) && GameManager.Instance.playerControl)
                {
                    // If the Wind Cannon has cooled down...
                    if (windCannonCooldown >= windCannonFireDelay)
                    {
                        FireWindCannon();
                    }
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
                if (Input.GetMouseButton(0) && GameManager.Instance.playerControl)
                {
                    // If the Wind Cannon has cooled down...
                    if (windCannonCooldown >= windCannonFireDelay)
                    {
                        FireWindCannon();
                    }
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
        windCannon.transform.GetChild(1).GetComponent<EffectSpawner>().SpawnEffect();
        windCannon.GetComponent<AudioSource>().Play();
        List<GameObject> targets = windCannon.GetComponentInChildren<CannonArea>().overlappingGameObjects;
        foreach (GameObject target in targets)
        {
            target.GetComponent<Rigidbody>().AddForce(windCannon.transform.forward * trueForce * 5f);
        }
    }
}
