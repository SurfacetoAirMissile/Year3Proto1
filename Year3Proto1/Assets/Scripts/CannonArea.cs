using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonArea : MonoBehaviour
{
    public List<GameObject> overlappingGameObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Chassis"))
        {
            // ignore own chassis
            if (other.gameObject != transform.parent.parent.GetChild(0).gameObject)
            {
                if (!overlappingGameObjects.Contains(other.gameObject))
                {
                    overlappingGameObjects.Add(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Chassis"))
        {
            // ignore own chassis
            if (other.gameObject != transform.parent.parent.GetChild(0).gameObject)
            {
                if (overlappingGameObjects.Contains(other.gameObject))
                {
                    overlappingGameObjects.Remove(other.gameObject);
                }
            }
        }
    }
}
