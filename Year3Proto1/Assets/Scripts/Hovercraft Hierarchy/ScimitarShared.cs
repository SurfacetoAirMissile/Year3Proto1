using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarShared : HovercraftShared
{
    protected GameObject minigunTurret;
    protected GameObject minigunElevationRing;
    protected GameObject minigunBarrel;
    protected GameObject bulletPrefab;

    [SerializeField]
    protected float minigunROF;

    [SerializeField]
    protected float minigunMaximumDepression;

    protected float minigunFireDelay;
    protected float minigunCooldown;

    public void ScimitarStartup()
    {
        HovercraftStartup();
        bulletPrefab = Resources.Load("Bullet") as GameObject;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Minigun Turret")) { minigunTurret = child.gameObject; }
            if (child.name.Contains("Minigun Elevation Ring")) { minigunElevationRing = child.gameObject; }
            if (child.name.Contains("Minigun Barrel")) { minigunBarrel = child.gameObject; }
        }
    }
}
