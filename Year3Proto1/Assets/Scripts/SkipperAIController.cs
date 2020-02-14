using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperAIController : SkipperShared
{
    enum SkipperAIState
    { 
        Wander
    }

    #region Wander Variable Definitions

    private bool wanderTurning;

    private float wanderTurnForce;

    private float wanderForce;

    private readonly static float wanderUpdateTimer = .5f;

    private float wanderUpdateStopwatch;

    #endregion

    //private float stateTimeElapsed;

    SkipperAIState state;

    // Start is called before the first frame update
    void Start()
    {
        Startup();
        state = SkipperAIState.Wander;
        wanderTurning = false;
        wanderTurnForce = 0f;
        wanderForce = .5f;
        wanderUpdateStopwatch = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
        switch (state)
        {
            case SkipperAIState.Wander:
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
                        wanderForce = Mathf.Clamp(wanderForce, .4f, .7f);
                    }
                }
                float pushAmount = Time.deltaTime * PushForce * wanderForce;
                chassisRB.AddForce(pushAmount * -chassis.transform.forward);
                if (wanderTurning)
                {
                    float rotationAmount = Time.deltaTime * RotationForce * wanderTurnForce;
                    chassisRB.AddTorque(new Vector3(0f, rotationAmount, 0f));
                }
                break;
        }
    }

    bool Chance(float _outOfOne)
    {
        return Random.Range(0f, 1f) <= _outOfOne;
    }
}
