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
    protected GameObject bulletEffect;
    protected float minigunFireDelay;
    protected float minigunCooldown;

    public void ScimitarStartup()
    {
        HovercraftStartup();
        minigunFireDelay = 1f / minigunROF;
        bulletPrefab = Resources.Load("Bullet") as GameObject;
        //bulletEffect = Resources.Load("BulletImpact") as GameObject;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Minigun Turret")) { minigunTurret = child.gameObject; }
            if (child.name.Contains("Minigun Elevation Ring")) { minigunElevationRing = child.gameObject; }
            if (child.name.Contains("Minigun Barrel")) { minigunBarrel = child.gameObject; }
        }
    }

    protected void PitchMinigunToTarget(Vector3 _targetDirection)
    {
        Vector3 barrelForward = -minigunElevationRing.transform.forward;
        Debug.DrawRay(minigunTurret.transform.position, barrelForward);
        Debug.DrawRay(minigunTurret.transform.position, _targetDirection);
        _targetDirection.x = 1; _targetDirection.z = 1;
        barrelForward.x = 1; barrelForward.z = 1;
        float angle = Vector3.Angle(_targetDirection, barrelForward);
        if (_targetDirection.y < barrelForward.y) { angle *= -1; }
        StaticFunc.RotateTo(minigunElevationRing.GetComponent<Rigidbody>(), 'x', angle);
    }

    protected void YawMinigunToTarget(Vector3 _targetDirection)
    {
        Vector3 minigunTurretRot = Quaternion.FromToRotation(-minigunTurret.transform.forward, _targetDirection).eulerAngles;
        if (minigunTurretRot.y > 180f) { minigunTurretRot.y -= 360f; }
        StaticFunc.RotateTo(minigunTurret.GetComponent<Rigidbody>(), 'y', minigunTurretRot.y);
    }


    protected void FireMinigun()
    {
        minigunCooldown = 0f;
        minigunBarrel.GetComponent<AudioSource>().Play();
        Vector3 spawnPos = minigunBarrel.transform.GetChild(0).position;
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody bulletRB = bulletInstance.GetComponent<Rigidbody>();
        //bulletRB.velocity = chassisRB.velocity;
        bulletRB.AddForce(-minigunBarrel.transform.forward * 5f);
        BulletBehaviour bulletB = bulletInstance.GetComponent<BulletBehaviour>();
        bulletB.SetDamage(minigunDamage);
        bulletB.SetOwner(this.gameObject);
        bulletInstance.layer = 12;
    }

    protected void AimMinigunAtTarget(Vector3 _targetDirection)
    {
        // Mortar Turret Rotation
        YawMinigunToTarget(_targetDirection);
        PitchMinigunToTarget(_targetDirection);
    }
}
