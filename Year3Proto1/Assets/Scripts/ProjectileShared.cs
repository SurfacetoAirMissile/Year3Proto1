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
    private GameObject hitSoundEffectPrefab;
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
        // if the object we hit belongs to the player
        Transform masterParent = StaticFunc.GetParentRecursive(collision.transform);
        if (masterParent.name.Contains("Player"))
        {
            if (owner.name.Contains("AI"))
            {
                masterParent.GetComponent<HovercraftShared>().healthComponent.DeductHealth(damage);
                GameObject hitSound = Instantiate(hitSoundEffectPrefab, transform, false);
                hitSound.transform.SetParent(null);
                timeElapsed = lifetime * .99f;
            }
        }
        else if (masterParent.name.Contains("AI"))
        {
            if (owner.name.Contains("Player"))
            {
                masterParent.GetComponent<HovercraftShared>().healthComponent.DeductHealth(damage);
                GameObject hitSound = Instantiate(hitSoundEffectPrefab, transform, false);
                hitSound.transform.SetParent(null);
                timeElapsed = lifetime * .99f;
            }
        }
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
