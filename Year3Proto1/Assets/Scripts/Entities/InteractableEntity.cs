using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableEntity : MonoBehaviour
{
    public GameObject prefab, canvas;
    public InteractableUI interactableUI { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject instantiatedObject = Instantiate(prefab, canvas.transform, false);
            interactableUI = instantiatedObject.GetComponent<InteractableUI>();
            interactableUI.target = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(interactableUI);
        }
    }

    private void LateUpdate()
    {
        if (interactableUI != null) Refresh();
    }

    public InteractableUI UserInterface()
    {
        return interactableUI;
    }

    public abstract void Refresh();
}
