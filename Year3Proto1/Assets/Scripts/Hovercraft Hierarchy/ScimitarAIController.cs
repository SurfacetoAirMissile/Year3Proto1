using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarAIController : ScimitarShared
{
    enum HovercraftAIState
    {
        Wander,
        Chase,
        OrbitEngage
    }

    public enum Faction
    {
        Neutral,
        Hostile
    }

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

    [SerializeField]
    private float orbitDistance;

    #endregion

    [SerializeField]
    private Faction faction;

    HovercraftAIState state;

    // Start is called before the first frame update
    void Start()
    {
        HovercraftStartup();
        if (faction == Faction.Neutral)
        {
            state = HovercraftAIState.Wander;
        }
        if (faction == Faction.Hostile)
        {
            state = HovercraftAIState.Wander;
        }
        wanderTurning = false;
        wanderTurnForce = 0f;
        wanderForce = .5f;
        wanderUpdateStopwatch = 0f;
        playerChassis = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        switch (state)
        {
            case HovercraftAIState.Wander:
                if (faction == Faction.Hostile)
                {
                    if (CanSeePlayer(spottingAngle, spottingRange))
                    {
                        state = HovercraftAIState.Chase;
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
                //Thrust(chassis.transform.forward, wanderForce);
                if (wanderTurning)
                {
                    float rotationAmount = rotationForce * wanderTurnForce;
                    StaticFunc.RotateTo(chassisRB, 'y', wanderTurnForce);
                }
                break;
            case HovercraftAIState.Chase:
                // Get direction from AI to player
                Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
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
                    state = HovercraftAIState.OrbitEngage;
                }

                break;
                /*
            case HovercraftAIState.OrbitEngage:
                // Get direction from AI to player
                Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
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
                    state = HovercraftAIState.OrbitEngage;
                }

                break;
                */
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
}
