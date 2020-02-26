using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScimitarAIController : ScimitarShared
{
    public enum Faction
    {
        Neutral,
        AntiPlayer,
        Hostile
    }

    [SerializeField] [Tooltip("The Bot's faction.")]
    private Faction faction;

    //public AIShared AIcomp;

    [Serializable]
    public enum HovercraftAIState
    {
        Wander,
        Chase,
        OrbitEngage
    }

    [Serializable]
    public struct StateObject
    {
        public StateObject(HovercraftAIState _state, GameObject _target = null)
        {
            if (_state == HovercraftAIState.Chase && _target == null)
            {
                Debug.LogError("StateObject needs target if in Chase state");
            }
            if (_state == HovercraftAIState.OrbitEngage && _target == null)
            {
                Debug.LogError("StateObject needs target if in OrbitEngage state");
            }
            stateName = _state;
            target = _target;
        }

        public HovercraftAIState stateName;
        public GameObject target;
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

    private float OrbitDistance
    {
        get
        {
            string targetName = stateO.target.transform.parent.name;
            if (targetName.Contains("Tortoise"))
            {
                return StaticFunc.FloatLookup("Tortoise Orbit Distance");
            }
            else // scimitar
            {
                return StaticFunc.FloatLookup("Scimitar Orbit Distance");
            }
        } 
    }

    #endregion

    public StateObject stateO;

    public int scrapOnKill = 100;

    // Start is called before the first frame update
    void Start()
    {
        ScimitarStartup();
        stateO = new StateObject(HovercraftAIState.Wander);
        wanderTurning = false;
        wanderTurnForce = 0f;
        wanderForce = .5f;
        wanderUpdateStopwatch = 0f;
        playerChassis = GameObject.FindGameObjectWithTag("Player");
        // Determines orbit distance based on player craft
        /*
        if (playerChassis.transform.parent.name.Contains("Tortoise"))
        {
            orbitDistance = StaticFunc.FloatLookup("Tortoise Orbit Distance");
        }
        else if (playerChassis.transform.parent.name.Contains("Scimitar"))
        {
            orbitDistance = StaticFunc.FloatLookup("Scimitar Orbit Distance");
        }
        */
        healthComponent.SetHealth(1f);
        explosionPrefab = Resources.Load("ShipExplosion") as GameObject;
        controller = ControllerType.AIController;
        GameManager.Instance.AddAlive(this);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        minigunCooldown += Time.deltaTime;
        // Get direction from AI to target
        if (Alive())
        {
            Vector3 AIToTarget = Vector3.forward;
            Vector3 AIMinigunToTarget = Vector3.forward;
            if (stateO.target)
            {
                AIToTarget = stateO.target.transform.position - chassis.transform.position;
                AIMinigunToTarget = stateO.target.transform.position - minigunTurret.transform.position;
            }

            switch (stateO.stateName)
            {
                case HovercraftAIState.Wander:
                    if (faction == Faction.AntiPlayer)
                    {
                        if (CanSeePlayer(spottingAngle, spottingRange))
                        {
                            ChangeState(HovercraftAIState.Chase, playerChassis);
                        }
                    }
                    else if (faction == Faction.Hostile)
                    {
                        GameObject target = CheckForTargets(spottingAngle, spottingRange);
                        if (target)
                        {
                            ChangeState(HovercraftAIState.Chase, target);
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
                            wanderTurnForce += UnityEngine.Random.Range(-.3f, .3f);
                            wanderTurnForce = Mathf.Clamp(wanderTurnForce, -.6f, .6f);
                            wanderForce += UnityEngine.Random.Range(-.3f, .3f);
                            wanderForce = Mathf.Clamp(wanderForce, .5f, .8f);
                        }
                    }
                    Thrust(chassis.transform.forward, wanderForce);
                    ThrustParticleEffect(true);
                    if (!thrustParticlePlay)
                    {
                        chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
                        thrustParticlePlay = true;
                    }

                    if (wanderTurning)
                    {
                        StaticFunc.RotateTo(chassisRB, 'y', wanderTurnForce);
                    }
                    break;
                case HovercraftAIState.Chase:
                    // Rotate them towards the player
                    Vector3 rotation = Quaternion.FromToRotation(chassis.transform.forward, AIToTarget).eulerAngles;
                    if (rotation.y > 180f) { rotation.y -= 360f; }
                    StaticFunc.RotateTo(chassisRB, 'y', rotation.y);
                    if (Mathf.Abs(rotation.y) <= 35f)
                    {
                        Thrust(chassis.transform.forward, 1f);
                        ThrustParticleEffect(true);
                    }
                    else
                    {
                        ThrustParticleEffect(false);
                    }

                    // If we are far away from the target, chase, if we're within engagement distance, orbit them and fire.
                    if (AIToTarget.magnitude < OrbitDistance * 2f)
                    {
                        ChangeState(HovercraftAIState.OrbitEngage, stateO.target);
                    }

                    break;

                case HovercraftAIState.OrbitEngage:
                    // Aim the cannon at the target
                    AimMinigunAtTarget(AIMinigunToTarget.normalized);
                    /*
                    Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, AIMinigunToTarget.normalized).eulerAngles;
                    if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
                    StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y * 4f);
                    if (Mathf.Abs(minigunTurretRot.y) < 15f)
                    {
                        Vector3 toPlayer = AIMinigunToTarget.normalized;
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
                    */

                    Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, AIMinigunToTarget.normalized).eulerAngles;
                    if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }

                    if (Mathf.Abs(minigunTurretRot.y) < 15f)
                    {
                        if (minigunCooldown >= minigunFireDelay)
                        {
                            FireMinigun();
                            /*
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
                            */
                        }
                    }

                    // Find a spot orbit distance far away from the player that's between the player and the point infront of the hovercraft.
                    Vector3 ScimitarFront = chassis.transform.position + (chassis.transform.forward * 5f);
                    //Debug.DrawRay()
                    Vector3 orbitPoint = stateO.target.transform.position + (ScimitarFront - stateO.target.transform.position).normalized * OrbitDistance;
                    Vector3 direction = orbitPoint - chassis.transform.position;
                    // Rotate them towards the player
                    Vector3 orbitRotation = Quaternion.FromToRotation(chassis.transform.forward, direction.normalized).eulerAngles;
                    if (orbitRotation.y > 180f) { orbitRotation.y -= 360f; }
                    StaticFunc.RotateTo(chassisRB, 'y', orbitRotation.y);
                    if (Mathf.Abs(orbitRotation.y) <= 35f)
                    {
                        Thrust(chassis.transform.forward, 1f);
                        ThrustParticleEffect(true);
                    }
                    else
                    {
                        ThrustParticleEffect(false);
                    }


                    // If we are far away from the player, chase, if we're within engagement distance, orbit them and fire.
                    if (AIToTarget.magnitude > OrbitDistance * 2f)
                    {
                        ChangeState(HovercraftAIState.Chase, stateO.target);
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
        return UnityEngine.Random.Range(0f, 1f) <= _outOfOne;
    }

    bool CanSeePlayer(float _spottingAngle, float _spottingRange)
    {
        Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
        if (AIToPlayer.magnitude > _spottingRange) { return false; }
        if (Vector3.Angle(chassis.transform.forward, AIToPlayer.normalized) > _spottingAngle)
        {
            return false;
        }
        return true;
    }

    GameObject CheckForTargets(float _spottingAngle, float _spottingRange)
    {
        foreach (HovercraftShared craft in GameManager.Instance.aliveCraft)
        {
            if (craft.gameObject == gameObject)
            {
                continue;
            }
            GameObject craftChassis = craft.GetChassis();
            Vector3 AIToCraft = craftChassis.transform.position - chassis.transform.position;
            if (AIToCraft.magnitude > _spottingRange) { continue; }
            if (Vector3.Angle(chassis.transform.forward, AIToCraft.normalized) > _spottingAngle)
            {
                continue;
            }
            return craftChassis;
        }
        return null;
    }

    bool deathFunctionCalled = false;
    void DeathFunction()
    {
        GameObject explosion = Instantiate(explosionPrefab, chassis.transform, false);
        explosion.transform.SetParent(null);
        MessageNotChasing();
        //Vector3 scale = explosion.transform.localScale;
        //scale.x = 1f;
        //scale.y = 1f;
        //scale.z = 1f;
        //explosion.transform.localScale = scale;
        //Explosion explosionScript = explosion.GetComponent<Explosion>();
        //explosionScript.explosionDamage = 0f;
        //explosionScript.explosionRadius = 0f;
        deathFunctionCalled = true;
        GameManager.Instance.RemoveAlive(this);
        if (healthComponent.GetKiller().Contains("Player"))
        {
            GameManager.Instance.playerScrap += scrapOnKill;
        }
    }

    void MessageChasing()
    {
        GameManager.Instance.AddAIChasing(gameObject);
    }

    void MessageNotChasing()
    {
        GameManager.Instance.RemoveAIChasing(gameObject);
    }

    public void ChangeState(HovercraftAIState _newState, GameObject _target = null)
    {
        stateO.stateName = _newState;
        switch (_newState)
        {
            case HovercraftAIState.Chase:
                stateO.target = _target;
                if (_target == playerChassis) { MessageChasing(); }
                break;
            case HovercraftAIState.OrbitEngage:
                stateO.target = _target;
                if (_target == playerChassis) { MessageChasing(); }
                break;
            case HovercraftAIState.Wander:
                MessageNotChasing();
                break;
        }
    }
}
