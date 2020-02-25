using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarAIController : ScimitarShared
{
    public enum Faction
    {
        Neutral,
        Hostile
    }

    [SerializeField] [Tooltip("The Bot's faction.")]
    private Faction faction;

    public enum HovercraftAIState
    {
        Wander,
        Chase,
        OrbitEngage
    }

    protected GameObject explosionPrefab;

    #region Wander Variable Definitions

    private bool wanderTurning;

    private float wanderTurnForce;

    private float wanderForce;

    private readonly static float wanderUpdateTimer = .5f;

    private float wanderUpdateStopwatch;

    #endregion

    #region Chase Variable Definitions

    [SerializeField]
    private float spottingAngle;

    [SerializeField]
    private float spottingRange;

    private GameObject playerChassis;

    #endregion

    #region Orbit Engage Variable Definitions

    private float orbitDistance;

    #endregion

    public HovercraftAIState state;

    // Start is called before the first frame update
    void Start()
    {
        ScimitarStartup();
        state = HovercraftAIState.Wander;
        wanderTurning = false;
        wanderTurnForce = 0f;
        wanderForce = .5f;
        wanderUpdateStopwatch = 0f;
        playerChassis = GameObject.FindGameObjectWithTag("Player");
        // Determines orbit distance based on player craft
        if (playerChassis.transform.parent.name.Contains("Tortoise"))
        {
            orbitDistance = StaticFunc.FloatLookup("Tortoise Orbit Distance");
        }
        else if (playerChassis.transform.parent.name.Contains("Scimitar"))
        {
            orbitDistance = StaticFunc.FloatLookup("Scimitar Orbit Distance");
        }
        healthComponent.SetHealth(1f);
        explosionPrefab = Resources.Load("Explosion") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        minigunCooldown += Time.deltaTime;
        ApplyLevitationForces();
        // Get direction from AI to player
        if (Alive())
        {
            Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
            Vector3 AIMinigunToPlayer = playerChassis.transform.position - minigunTurret.transform.position;
            switch (state)
            {
                case HovercraftAIState.Wander:
                    if (faction == Faction.Hostile)
                    {
                        if (CanSeePlayer(spottingAngle, spottingRange))
                        {
                            ChangeState(HovercraftAIState.Chase);
                        }
                    }
                    wanderUpdateStopwatch += Time.deltaTime;
                    if (wanderUpdateStopwatch >= wanderUpdateTimer)
                    {
                        wanderUpdateStopwatch = 0f;
                        if (Chance(.3f))
                        {
                            if (Chance(.3f))
                            {
                                wanderTurning = !wanderTurning;
                            }
                            wanderTurnForce += Random.Range(-.3f, .3f);
                            wanderTurnForce = Mathf.Clamp(wanderTurnForce, -.6f, .6f);
                            wanderForce += Random.Range(-.3f, .3f);
                            wanderForce = Mathf.Clamp(wanderForce, .5f, .8f);
                        }
                    }
                    Thrust(chassis.transform.forward, wanderForce);
                    if (wanderTurning)
                    {
                        float rotationAmount = rotationForce * wanderTurnForce;
                        StaticFunc.RotateTo(chassisRB, 'y', wanderTurnForce);
                    }
                    break;
                case HovercraftAIState.Chase:
                    // Rotate them towards the player
                    Vector3 rotation = Quaternion.FromToRotation(chassis.transform.forward, AIToPlayer).eulerAngles;
                    if (rotation.y > 180f) { rotation.y -= 360f; }
                    StaticFunc.RotateTo(chassisRB, 'y', rotation.y);
                    if (Mathf.Abs(rotation.y) <= 35f)
                    {
                        Thrust(chassis.transform.forward, 1f);
                    }

                    // If we are far away from the player, chase, if we're within engagement distance, orbit them and fire.
                    if (AIToPlayer.magnitude < orbitDistance * 2f)
                    {
                        ChangeState(HovercraftAIState.OrbitEngage);
                    }

                    break;

                case HovercraftAIState.OrbitEngage:
                    // Aim the cannon at the player

                    Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, AIMinigunToPlayer.normalized).eulerAngles;
                    if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
                    StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y * 4f);
                    if (Mathf.Abs(minigunTurretRot.y) < 15f)
                    {
                        Vector3 toPlayer = AIMinigunToPlayer.normalized;
                        Vector3 barrelForward = -minigunElevationRing.transform.forward;
                        Debug.DrawLine(minigunBarrel.transform.position, minigunBarrel.transform.position + toPlayer, Color.red);
                        Debug.DrawLine(minigunBarrel.transform.position, minigunBarrel.transform.position + barrelForward, Color.green);
                        float angle = Vector3.Angle(toPlayer, barrelForward);
                        toPlayer.x = 0; toPlayer.z = 0;
                        barrelForward.x = 0; barrelForward.z = 0;
                        if (toPlayer.y < barrelForward.y)
                        {
                            angle *= -1;
                        }
                        StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
                    }

                    if (Mathf.Abs(minigunTurretRot.y) < 15f)
                    {
                        if (minigunCooldown >= minigunFireDelay)
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

                        }
                    }

                    // Find a spot orbit distance far away from the player that's between the player and the point infront of the hovercraft.
                    Vector3 ScimitarFront = chassis.transform.position + (chassis.transform.forward * 5f);
                    //Debug.DrawRay()
                    Vector3 orbitPoint = playerChassis.transform.position + (ScimitarFront - playerChassis.transform.position).normalized * orbitDistance;
                    Vector3 direction = orbitPoint - chassis.transform.position;
                    // Rotate them towards the player
                    Vector3 orbitRotation = Quaternion.FromToRotation(chassis.transform.forward, direction.normalized).eulerAngles;
                    if (orbitRotation.y > 180f) { orbitRotation.y -= 360f; }
                    StaticFunc.RotateTo(chassisRB, 'y', orbitRotation.y);
                    if (Mathf.Abs(orbitRotation.y) <= 35f)
                    {
                        Thrust(chassis.transform.forward, 1f);
                    }


                    // If we are far away from the player, chase, if we're within engagement distance, orbit them and fire.
                    if (AIToPlayer.magnitude > orbitDistance * 2f)
                    {
                        state = HovercraftAIState.Chase;
                    }

                    break;

            }
        }
        else
        {
            if (!deathFunctionCalled)
            {
                // call death function
                DeathFunction();
            }
            // destroy self
            Destroy(gameObject);
        }
    }

    bool Chance(float _outOfOne)
    {
        return Random.Range(0f, 1f) <= _outOfOne;
    }

    bool CanSeePlayer(float _spottingAngle, float _spottingRange)
    {
        Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
        if (AIToPlayer.magnitude > _spottingRange) { return false; }
        Debug.DrawLine(chassis.transform.position, chassis.transform.position + AIToPlayer.normalized, Color.red);
        Debug.DrawLine(chassis.transform.position, chassis.transform.position + chassis.transform.forward, Color.green);
        if (Vector3.Angle(chassis.transform.forward, AIToPlayer.normalized) > _spottingAngle)
        {
            return false;
        }
        return true;
    }

    bool deathFunctionCalled = false;
    void DeathFunction()
    {
        GameObject explosion = Instantiate(explosionPrefab, chassis.transform, false);
        explosion.transform.SetParent(null);
        Vector3 scale = explosion.transform.localScale;
        scale.x = 5f;
        scale.y = 5f;
        scale.z = 5f;
        explosion.transform.localScale = scale;
        Explosion explosionScript = explosion.GetComponent<Explosion>();
        explosionScript.explosionDamage = 0f;
        explosionScript.explosionRadius = 0f;
        deathFunctionCalled = true;
    }

    void MessageChasing()
    {
        GameManager.Instance.AddAIChasing(gameObject);
    }

    void MessageNotChasing()
    {
        GameManager.Instance.RemoveAIChasing(gameObject);
    }

    void ChangeState(HovercraftAIState _newState)
    {
        state = _newState;
        switch (state)
        {
            case HovercraftAIState.Chase:
                MessageChasing();
                break;
            case HovercraftAIState.OrbitEngage:
                MessageChasing();
                break;
            case HovercraftAIState.Wander:
                MessageNotChasing();
                break;
        }
    }
}
