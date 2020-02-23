using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : ProjectileShared
{
    public GameObject explosionPrefab;

    private void Update()
    {
        PSUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform);
        explosionInstance.transform.SetParent(null);
        Destroy(this.gameObject);
    }
}
