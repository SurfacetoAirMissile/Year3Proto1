using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftShared : MonoBehaviour
{
    [Header("Hovercraft Shared")]
    [SerializeField] [Tooltip("The amount of force pushing up on the chassis from each Hoverball, in thousands of units/second.")]
    protected float hoverForce;

    [SerializeField] [Tooltip("The amount of force pushing on the chassis to move around, in thousands of units/second.")]
    protected float pushForce;

    [SerializeField] [Tooltip("The amount of force pushing on the chassis to rotate it, in thousands of units/second.")]
    protected float rotationForce;

    public HealthComponent healthComponent;
    protected Rigidbody chassisRB;
    protected bool isTouchingGround;
    protected bool isFlipped;
    protected GameObject chassis;
    protected List<GameObject> hoverballs;
    protected float totalMass;
    protected bool thrustParticlePlay = false;
    protected bool blackSmokeParticlePlay = false;
    protected bool playerAiming = false;

    public List<UpgradeType> installedUpgrades;

    public enum ControllerType
    {
        PlayerController,
        AIController
    }


    public ControllerType controller;

    public enum PlayerFocus
    {
        Scimitar,
        ScimitarMinigun,
        ScimitarWindCannon,
        Tortoise,
        TortoiseMortar,
        TortoiseWindCannon
    }


    protected void Thrust(Vector3 _direction, float _power)
    {
        if (!isFlipped)
        {
            float pushAmount = Time.deltaTime * 1000f * pushForce * _power;
            Vector3 XZdirection = _direction; XZdirection.y = 0;
            chassisRB.AddForce(pushAmount * (isTouchingGround ? _direction.normalized : XZdirection.normalized));
        }
    }

    protected void HovercraftStartup()
    {
        healthComponent = new HealthComponent(1f);
        totalMass = GetTotalMass();
        hoverballs = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Hoverball")) { hoverballs.Add(child.gameObject); }
            if (child.name.Contains("Chassis")) { chassis = child.gameObject; }
            //if (child.name.Contains("Wind Cannon")) { weapons.Add(child.gameObject); }
            //if (child.name.Contains("Minigun")) { weapons.Add(child.gameObject); }
            //if (child.name.Contains("Mortar")) { weapons.Add(child.gameObject); }
        }
        chassisRB = chassis.GetComponent<Rigidbody>();
        installedUpgrades = new List<UpgradeType>();
    }

    protected void ApplyLevitationForces()
    {
        isTouchingGround = false;
        foreach (GameObject ball in hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 1f))
            {
                float forceStrength = Time.deltaTime * hoverForce * 1000f;
                float maxForceApplication = forceStrength * 10f;
                float distanceSquared = Mathf.Pow(groundCast.distance, 2f);
                float finalForce = Mathf.Clamp(forceStrength / groundCast.distance, 0f, maxForceApplication);
                if (healthComponent.GetHealth() > 0f)
                {
                    ball.GetComponent<Rigidbody>().AddForce(finalForce * Vector3.up);
                }
                //ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white * (1f - (finalForce / maxForceApplication)));
            }
            if (Physics.Raycast(ball.transform.position, Vector3.down, 2f))
            {
                isTouchingGround = true;
            }
        }
        isFlipped = false;
        Vector3 rotation = Quaternion.FromToRotation(chassis.transform.up, Vector3.up).eulerAngles;
        rotation.y = 0f;
        if (rotation.x >= 180) { rotation.x -= 360f; }
        if (rotation.z >= 180) { rotation.z -= 360f; }
        if (Mathf.Abs(rotation.x) > 30f || Mathf.Abs(rotation.z) > 30f)
        {
            isFlipped = true;
        }
        if (isTouchingGround && isFlipped)
        {
            chassisRB.AddTorque(rotation * Time.deltaTime * 1000f);
        }
    }

    protected void SmokeEmission()
    {
        if (!playerAiming)
        {
            if (healthComponent.GetHealthRelative() <= .5f)
            {
                if (!blackSmokeParticlePlay)
                {
                    blackSmokeParticlePlay = true;
                    chassis.transform.GetChild(1).GetComponent<ParticleSystem>().Play(true);
                }
            }
            else
            {
                if (blackSmokeParticlePlay)
                {
                    blackSmokeParticlePlay = false;
                    chassis.transform.GetChild(1).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }
        else
        {
            // player is aiming
            if (blackSmokeParticlePlay)
            {
                blackSmokeParticlePlay = false;
                chassis.transform.GetChild(1).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    protected void HovercraftSharedUpdate()
    {
        ApplyLevitationForces();
        SmokeEmission();
    }

    float GetTotalMass()
    {
        float massTotal = 0;
        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>())
        {
            massTotal += body.mass;
        }
        return massTotal;
    }

    public GameObject GetChassis()
    {
        return chassis;
    }

    public Rigidbody GetRB()
    {
        return chassisRB;
    }

    public bool Alive()
    {
        return healthComponent.GetHealth() > 0f;
    }

    protected void ThrustParticleEffect(bool _thrustParticleOn)
    {
        if (_thrustParticleOn)
        {
            if (!thrustParticlePlay)
            {
                chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
                thrustParticlePlay = true;
            }
        }
        else
        {
            if (thrustParticlePlay)
            {
                thrustParticlePlay = false;
                chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public void InstallUpgrade(UpgradeType _type)
    {
        if (!installedUpgrades.Contains(_type))
        {
            installedUpgrades.Add(_type);
            if (_type == UpgradeType.WEAPON_WIND_CANNON)
            {
                if (name.Contains("Tortoise"))
                {
                    GetComponent<TortoisePlayer>().windCannonForce *= 2f;
                }
                if (name.Contains("Scimitar"))
                {
                    GetComponent<ScimitarPlayer>().windCannonForce *= 2f;
                }
            }
            if (_type == UpgradeType.WEAPON_MORTAR)
            {
                if (name.Contains("Tortoise"))
                {
                    GetComponent<TortoisePlayer>().mortarDamage *= 2f;
                }
            }
            if (_type == UpgradeType.WEAPON_MINIGUN)
            {
                if (name.Contains("Scimitar"))
                {
                    GetComponent<ScimitarPlayer>().minigunDamage *= 2f;
                }
            }
            if (_type == UpgradeType.WEAPON_MORTAR_ROF)
            {
                if (name.Contains("Tortoise"))
                {
                    GetComponent<TortoisePlayer>().SetMortarFireRate(0.5f);
                }
            }
            if (_type == UpgradeType.WEAPON_MINIGUN_ROF)
            {
                if (name.Contains("Scimitar"))
                {
                    GetComponent<ScimitarPlayer>().SetMinigunFireRate(18.3f);
                }
            }
        }
    }
}
