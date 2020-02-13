using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{

    public GameObject effect;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnEffect();
        }
    }

    public void SpawnEffect()
    {
        if (effect != null)
        {
            Instantiate(effect, transform);
        }
        else
        {
            Debug.LogError("Effect not set in inspector");
        }
    }
}
