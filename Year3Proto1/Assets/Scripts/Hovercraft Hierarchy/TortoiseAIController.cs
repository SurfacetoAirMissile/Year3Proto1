using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TortoiseAIController : TortoiseShared
{
    public enum Faction
    {
        Neutral,
        AntiPlayer,
        Hostile
    }

    [SerializeField]
    [Tooltip("The Bot's faction.")]
    private Faction faction;

    //public AIShared AIcomp;

    [Serializable]
    public enum HovercraftAIState
    {
        Wander,
        Chase,
        ParkEngage
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
            if (_state == HovercraftAIState.ParkEngage && _target == null)
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

    #region Park Engage Variable Definitions

    private float ParkDistance
    {
        get
        {
            string targetName = stateO.target.transform.parent.name;
            if (targetName.Contains("Tortoise"))
            {
                return StaticFunc.FloatLookup("Tortoise Park Distance");
            }
            else // scimitar
            {
                return StaticFunc.FloatLookup("Scimitar Park Distance");
            }
        }
    }

    #endregion

    public StateObject stateO;

    public int scrapOnKill = 200;

    // Start is called before the first frame update
    void Start()
    {
        TortoiseStartup();
        stateO = new StateObject(HovercraftAIState.Wander);
        wanderTurning = false;
        wanderTurnForce = 0f;
        wanderForce = .5f;
        wanderUpdateStopwatch = 0f;
        playerChassis = GameObject.FindGameObjectWithTag("Player");
        healthComponent.SetHealth(3f, true);
        explosionPrefab = Resources.Load("ShipExplosion") as GameObject;
        controller = ControllerType.AIController;
        GameManager.Instance.AddAlive(this);
    }


    // Update is called once per frame
    void Update()
    {
        HovercraftSharedUpdate();
        mortarCooldown += Time.deltaTime;
        // Get direction from AI to target
        if (Alive())
        {
            Vector3 AIToTarget = Vector3.forward;
            Vector3 AIMortarToTarget = Vector3.forward;
            if (stateO.target)
            {
                AIToTarget = stateO.target.transform.position - chassis.transform.position;
                AIMortarToTarget = stateO.target.transform.position - mortarTurret.transform.position;
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

                    // If we are close enough to engage...
                    if (AIToTarget.magnitude < ParkDistance * 0.5f)
                    {
                        // engage...
                        ChangeState(HovercraftAIState.ParkEngage, stateO.target);
                    }


                    break;

                case HovercraftAIState.ParkEngage:
                    ThrustParticleEffect(false);
                    // Aim the cannon at the target
                    AimMortarAtAITarget();

                    Vector3 minigunTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, AIMortarToTarget.normalized).eulerAngles;
                    if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }

                    if (Mathf.Abs(minigunTurretRot.y) < 15f)
                    {
                        if (mortarCooldown >= mortarFireDelay)
                        {
                            FireMortar();
                        }
                    }

                    // If we are far away from the player, chase, if we're within engagement distance, orbit them and fire.
                    if (AIToTarget.magnitude > ParkDistance)
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
            GameManager.Instance.gameWave.scrap += scrapOnKill;

            GameManager.Instance.playerKills += 1;
            GameManager.Instance.gameWave.kills += 1;
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
            case HovercraftAIState.ParkEngage:
                stateO.target = _target;
                if (_target == playerChassis) { MessageChasing(); }
                break;
            case HovercraftAIState.Wander:
                MessageNotChasing();
                break;
        }
    }

    protected void AimMortarAtAITarget()
    {
        if (stateO.target)
        {
            Vector3 AIMortarToTarget = stateO.target.transform.position - mortarTurret.transform.position;
            float distanceToTarget = AIMortarToTarget.magnitude;
            float distAmount = distanceToTarget / ParkDistance;
            //float angleRad = Mathf.Deg2Rad * (distanceToTarget / ParkDistance) * 30;
            float angleRad = Mathf.Deg2Rad * 20f * (Mathf.Log(distAmount + 1f) + 0.2f);
            Vector3 mortarAim = Vector3.RotateTowards(AIMortarToTarget.normalized, mortarTurret.transform.up, angleRad, 30f);
            //Debug.Log(mortarAim);
            AimMortarAtTarget(mortarAim);
        }
        else
        {
            Debug.LogError("AI shouldn't try to aim the mortar at the target when there is no target...");
        }
    }
}
