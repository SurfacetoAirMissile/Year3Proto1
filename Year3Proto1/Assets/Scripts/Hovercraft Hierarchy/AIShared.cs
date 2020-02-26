using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShared
{


    #region Wander Variable Definitions

    public bool wanderTurning;

    public float wanderTurnForce;

    public float wanderForce;

    public readonly static float wanderUpdateTimer = .5f;

    public float wanderUpdateStopwatch;

    #endregion

    #region Chase Variable Definitions

    [SerializeField]
    public float spottingAngle;

    [SerializeField]
    public float spottingRange;

    public GameObject playerChassis;

    #endregion

    #region Orbit Engage Variable Definitions

    public float orbitDistance;

    #endregion

    public enum HovercraftAIState
    {
        Wander,
        Chase,
        OrbitEngage
    }

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

    // Start is called before the first frame update
    public void AISStart()
    {
        
    }

    // Update is called once per frame
    public void AISUpdate()
    {
        
    }
}
