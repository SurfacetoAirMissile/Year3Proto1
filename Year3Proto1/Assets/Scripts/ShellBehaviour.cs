using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : ProjectileShared
{
    public GameObject explosionPrefab;
    public float explosionRadius;

    private string ownerName;

    private void Start()
    {
        ownerName = owner.name;
    }

    private void Update()
    {
        PSUpdate();
        if (timeElapsed >= .1f)
        {
            GetComponent<SphereCollider>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform);
        explosionInstance.transform.SetParent(null);
        Explosion explosionScript = explosionInstance.GetComponent<Explosion>();
        explosionScript.ownerName = ownerName;
        explosionScript.owner = owner;
        explosionScript.explosionDamage = damage;
        explosionScript.explosionRadius = explosionRadius;
        Destroy(this.gameObject);
    }
}
