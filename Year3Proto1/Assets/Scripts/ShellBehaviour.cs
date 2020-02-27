using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : ProjectileShared
{
    public GameObject explosionPrefab;
    public float explosionRadius;

    private void Update()
    {
        PSUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform);
        explosionInstance.transform.SetParent(null);
        Explosion explosionScript = explosionInstance.GetComponent<Explosion>();
        explosionScript.owner = owner;
        explosionScript.explosionDamage = damage;
        explosionScript.explosionRadius = explosionRadius;
        Destroy(this.gameObject);
    }
}
