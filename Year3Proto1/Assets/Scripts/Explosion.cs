using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 8f;
    public float explosionDamage = 1f;
    float timeElapsed = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        HovercraftShared[] victimScripts = FindObjectsOfType<HovercraftShared>();
        foreach (HovercraftShared script in victimScripts)
        {
            Vector3 toSkimmer = script.GetChassis().transform.position - transform.position;
            float distance = toSkimmer.magnitude;
            if (distance <= explosionRadius)
            {
                float amount;
                if (distance <= explosionRadius * .5f)
                {
                    amount = 1f;
                }
                else
                {
                    amount = 1f - ((distance - explosionRadius * .5f) / explosionRadius * .5f);
                }
                
                script.healthComponent.DeductHealth(explosionDamage * amount);
                script.GetRB().AddForce(toSkimmer.normalized * explosionDamage * 1000f * amount);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 5f)
        {
            Destroy(gameObject);
        }
    }
}
