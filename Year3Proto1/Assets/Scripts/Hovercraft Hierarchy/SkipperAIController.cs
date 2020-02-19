using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperAIController : SkipperShared
{
    enum SkipperAIState
    { 
        Wander,
        Chase
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

    private GameObject playerChassis;

    #endregion


    [SerializeField]
    private float spottingAngle;

    [SerializeField]
    private float spottingRange;

    //private float stateTimeElapsed;

    [SerializeField]
    private Faction faction;

    SkipperAIState state;

    // Start is called before the first frame update
    void Start()
    {
        HovercraftStartup();
        if (faction == Faction.Neutral)
        {
            state = SkipperAIState.Wander;
        }
        if (faction == Faction.Hostile)
        {
            state = SkipperAIState.Wander;
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
            case SkipperAIState.Wander:
                if (faction == Faction.Hostile)
                {
                    if (CanSeePlayer(spottingAngle, spottingRange))
                    {
                        state = SkipperAIState.Chase;
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
                Thrust(-chassis.transform.forward, wanderForce);
                if (wanderTurning)
                {
                    float rotationAmount = Time.deltaTime * rotationForce * 1000f * wanderTurnForce;
                    chassisRB.AddTorque(new Vector3(0f, rotationAmount, 0f));
                }
                break;
            case SkipperAIState.Chase:
                // Get direction from AI to player
                Vector3 AIToPlayer = playerChassis.transform.position - chassis.transform.position;
                // Rotate them towards the player
                Vector3 rotation = Quaternion.FromToRotation(-chassis.transform.forward, AIToPlayer).eulerAngles;
                if (rotation.y > 180f) { rotation.y -= 360f; }
                if (Mathf.Abs(rotation.y) > 5f)
                {
                    float yRotation = Time.deltaTime * rotationForce * Mathf.Sign(rotation.y);
                    chassisRB.AddTorque(0f, yRotation * 1000f, 0f);
                }
                if (Mathf.Abs(rotation.y) <= 35f)
                {
                    Thrust(-chassis.transform.forward, 1f);
                }
                break;
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
        Vector3 rotation = Quaternion.FromToRotation(-chassis.transform.forward, AIToPlayer.normalized).eulerAngles;
        if (rotation.x > 180f) { rotation.x -= 360f; }
        if (rotation.y > 180f) { rotation.y -= 360f; }
        if (rotation.z > 180f) { rotation.z -= 360f; }
        if (Mathf.Abs(rotation.x) > _spottingAngle) { return false; }
        if (Mathf.Abs(rotation.y) > _spottingAngle) { return false; }
        if (Mathf.Abs(rotation.z) > _spottingAngle) { return false; }
        return true;
    }
}
