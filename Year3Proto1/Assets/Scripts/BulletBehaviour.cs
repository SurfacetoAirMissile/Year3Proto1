using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField]
    private float lifetime;
    private float timeElapsed;
    private GameObject owner;
    private float damage;

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= lifetime)
        {
            Destroy(gameObject);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        // if the object we hit belongs to the player
        Transform masterParent = GetParentRecursive(collision.transform);
        if (masterParent.name.Contains("Player"))
        {
            if (masterParent.name.Contains("Scimitar"))
            {
                if (owner.name.Contains("AI"))
                {
                    masterParent.GetComponent<ScimitarPlayer>().DeductHealth(damage);
                    timeElapsed = lifetime * .9f;
                }
            }
        }
        if (masterParent.name.Contains("AI"))
        {
            if (masterParent.name.Contains("Scimitar"))
            {
                if (owner.name.Contains("Player"))
                {
                    //masterParent.GetComponent<ScimitarAIController>().DeductHealth(damage);
                    timeElapsed = lifetime * .9f;
                }
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

    Transform GetParentRecursive(Transform _transform)
    {
        // If the transform has a parent
        if (_transform.parent)
        {
            // Call the function on the parent and return the result to the caller
            return GetParentRecursive(_transform.parent);
        }
        // If the transform does not have a parent, return it (that is the omega)
        else return _transform;
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
