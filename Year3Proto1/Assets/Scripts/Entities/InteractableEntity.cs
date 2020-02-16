using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableEntity : MonoBehaviour
{
    public GameObject prefab, canvas, target;
    public InteractableUI interactableUI { get; set; }

    private bool interacting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interacting = true;
           
            GameObject instantiatedObject = Instantiate(prefab, canvas.transform, false);
            interactableUI = instantiatedObject.GetComponent<InteractableUI>();
            interactableUI.target = transform;
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interacting = false;
            Destroy(interactableUI.gameObject);
            Destroy(interactableUI.gameObject);
            OnExitRange();
        }
    }

    private void Update()
    {
        if (interactableUI.InRange() && Input.GetKey("f") && interacting) OnInteract();
        if (interactableUI != null && interacting) OnRefresh();
    }

    public InteractableUI UserInterface()
    {
        return interactableUI;
    }

    public abstract void OnRefresh();

    public abstract void OnExitRange();

    public abstract void OnInteract();
}
