using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private List<int> layers;

    private void Awake()
    {
        layers = new List<int>();
    }

    public abstract void OnInteraction(GameObject gameObject);

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gameObject = collision.gameObject;
        layers.ForEach(layer => {
            if(gameObject.layer == layer) OnInteraction(gameObject);
        });
    }
}
