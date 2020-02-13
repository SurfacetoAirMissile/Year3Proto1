using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntity : MonoBehaviour
{
    public GameObject prefab;
    public GameObject canvas;

    private InteractableUI interactableUI;

    private void OnTriggerEnter(Collider other)
    {
        GameObject instantiatedObject = Instantiate(prefab, canvas.transform, false);
        interactableUI = instantiatedObject.GetComponent<InteractableUI>();
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(interactableUI);
    }

    public InteractableUI UserInterface()
    {
        return interactableUI;
    }
}
