using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{

    public float delay;


    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if (delay <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
