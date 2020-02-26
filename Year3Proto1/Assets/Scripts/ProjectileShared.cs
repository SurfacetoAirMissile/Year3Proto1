using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShared : MonoBehaviour
{
    [Header("Projectile Shared")]
    [SerializeField]
    [Tooltip("How long the projectile will exist before being deleted.")]
    protected float lifetime;
    [SerializeField]
    [Tooltip("Projectile damage.")]
    protected float damage;
    protected float timeElapsed;
    protected GameObject owner;
    [SerializeField]
    protected GameObject hitSoundEffectPrefab;
    [SerializeField]
    protected GameObject bulletImpact;
    // Update is called once per frame
    protected void PSUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= lifetime)
        {
            Destroy(gameObject);
        }


    }

    protected void PSOnCollisionEnter(Collision collision)
    {
        
    }

    public GameObject GetOwner()
    {
        return owner;
    }

    public void SetOwner(GameObject _owner)
    {
        owner = _owner;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    public void SetOwner(float _damage)
    {
        damage = _damage;
    }
}
