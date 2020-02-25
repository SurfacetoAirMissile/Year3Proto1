using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortoiseShared : HovercraftShared
{
    [Header("Tortoise Shared")]

    [SerializeField]
    [Tooltip("The Mortar's rate of fire, in rounds per second (not rounds per minute/rpm)")]
    protected float mortarROF;

    [SerializeField]
    [Tooltip("The Mortar's maximum angle of depression")]
    protected float mortarMaximumDepression;

    [SerializeField]
    [Tooltip("The Mortar's maximum angle of elevation")]
    protected float mortarMaximumElevation;

    [SerializeField]
    [Tooltip("The Mortar's damage")]
    protected float mortarDamage;

    [SerializeField]
    [Tooltip("The Mortar's explosion radius")]
    protected float mortarExpRadius;

    protected GameObject mortarTurret;
    protected GameObject mortarBarrel;
    protected GameObject shellPrefab;
    protected GameObject explosionPrefab;
    protected float mortarFireDelay;
    protected float mortarCooldown;

    public void TortoiseStartup()
    {
        HovercraftStartup();
        mortarFireDelay = 1f / mortarROF;
        mortarCooldown = 0f;
        shellPrefab = Resources.Load("Shell") as GameObject;
        explosionPrefab = Resources.Load("Explosion") as GameObject;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Mortar Turret")) { mortarTurret = child.gameObject; }
            if (child.name.Contains("Mortar Barrel")) { mortarBarrel = child.gameObject; }
        }
    }
}
