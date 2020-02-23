using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ProjectileShared
{
    private void Update()
    {
        PSUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PSOnCollisionEnter(collision);
    }
}
