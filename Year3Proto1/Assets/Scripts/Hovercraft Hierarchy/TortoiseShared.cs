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
    protected GameObject mortarImpactZone;

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
            if (child.name.Contains("MortarImpactZone")) { mortarImpactZone = child.gameObject; }
        }
    }

    protected void PitchMortarToTarget(Vector3 _targetDirection)
    {
        Vector3 barrelForward = mortarBarrel.transform.forward;
        _targetDirection.x = 1; _targetDirection.z = 1;
        barrelForward.x = 1; barrelForward.z = 1;
        float angle = Vector3.Angle(_targetDirection, barrelForward);
        if (_targetDirection.y > barrelForward.y) { angle *= -1; }
        StaticFunc.RotateTo(mortarBarrel.GetComponent<Rigidbody>(), 'x', angle);
    }

    protected void YawMortarToTarget(Vector3 _targetDirection)
    {
        Vector3 mortarTurretRot = Quaternion.FromToRotation(mortarTurret.transform.forward, _targetDirection).eulerAngles;
        if (mortarTurretRot.y > 180f) { mortarTurretRot.y -= 360f; }
        StaticFunc.RotateTo(mortarTurret.GetComponent<Rigidbody>(), 'y', mortarTurretRot.y);
    }

    protected void FireMortar()
    {
        mortarCooldown = 0f;
        mortarTurret.GetComponent<AudioSource>().Play();
        mortarBarrel.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        Vector3 spawnPos = mortarBarrel.transform.GetChild(0).position;
        GameObject shellInstance = Instantiate(shellPrefab, spawnPos, Quaternion.identity);
        Rigidbody shellRB = shellInstance.GetComponent<Rigidbody>();
        shellRB.velocity = chassisRB.velocity;
        shellRB.AddForce(mortarBarrel.transform.forward * 1500f);
        ShellBehaviour shellB = shellInstance.GetComponent<ShellBehaviour>();
        shellB.SetDamage(mortarDamage);
        shellB.SetOwner(this.gameObject);
        shellB.explosionPrefab = explosionPrefab;
        if (controller == ControllerType.PlayerController)
        {
            shellInstance.layer = 12;
        }
        else if (controller == ControllerType.AIController)
        {
            shellInstance.layer = 13;
        }

    }

    protected void AimMortarAtTarget(Vector3 _targetDirection)
    {
        // Mortar Turret Rotation
        YawMortarToTarget(_targetDirection);
        PitchMortarToTarget(_targetDirection);
    }
}
