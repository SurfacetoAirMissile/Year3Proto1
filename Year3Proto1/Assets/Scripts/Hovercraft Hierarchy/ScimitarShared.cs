using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarShared : HovercraftShared
{
    protected GameObject minigunTurret;
    protected GameObject minigunElevationRing;
    protected GameObject minigunBarrel;

    [SerializeField]
    protected float minigunROF;

    protected float minigunFireDelay;
    protected float minigunCooldown;


    public void ScimitarStartup()
    {
        Startup();
    }
}
