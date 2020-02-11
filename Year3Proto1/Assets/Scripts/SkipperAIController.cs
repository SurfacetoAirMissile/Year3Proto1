using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperAIController : SkipperShared
{
    // Start is called before the first frame update
    void Start()
    {
        Startup();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyLevitationForces();
    }
}
