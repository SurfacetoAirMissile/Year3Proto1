using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarShared : HovercraftShared
{
    [Header("Scimitar Shared")]

    [SerializeField] [Tooltip("The Minigun's rate of fire, in rounds per second (not rounds per minute/rpm)")]
    protected float minigunROF;

    [SerializeField] [Tooltip("The Minigun's maximum angle of depression")]
    protected float minigunMaximumDepression;

    [SerializeField] [Tooltip("The Minigun's damage")]
    protected float minigunDamage;

    protected GameObject minigunTurret;
    protected GameObject minigunElevationRing;
    protected GameObject minigunBarrel;
    protected GameObject bulletPrefab;
    protected float minigunFireDelay;
    protected float minigunCooldown;



    public void ScimitarStartup()
    {
        minigunFireDelay = 1f / minigunROF;
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
