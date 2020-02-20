using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperPlayerController : SkipperShared
{


    // Start is called before the first frame update
    void Start()
    {
        SkipperStartup();
    }

    // Update is called once per frame
    void Update()
    {

        ApplyLevitationForces();
        float rotationAmount = Time.deltaTime * 1000f * rotationForce;

        if (Input.GetKeyDown("w"))
        {
            chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
        }
        if (Input.GetKeyUp("w"))
        {
            chassis.transform.GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        if (Input.GetKey("w"))
        {
            Thrust(chassis.transform.forward, 1f);
        }
        if (Input.GetKey("s"))
        {
            Thrust(-chassis.transform.forward, 1f);
        }                                                                              
        if (Input.GetKey("e"))                                                         
        {                                                                              
            Thrust(-chassis.transform.right, 1f);                                      
        }                                                                              
        if (Input.GetKey("q"))                                                         
        {                                                                              
            Thrust(chassis.transform.right, 1f);                                       
        }                                                                              
        if (Input.GetKey("d"))                                                         
        {                                                                              
            chassisRB.AddTorque(0f, rotationAmount, 0f);                               
        }
        if (Input.GetKey("a"))
        {
            chassisRB.AddTorque(0f, -rotationAmount, 0f);
        }
    }
}
