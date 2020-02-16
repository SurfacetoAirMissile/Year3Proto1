using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : InteractableEntity
{
    private bool collecting = false;
    public GameObject effect;

    private Rigidbody body;
    private float finalSpeed = 24.0f;
    private float currentSpeed = 0.0f;

    public override void OnInteract()
    {
        if (!collecting)
        {
            interactableUI.gameObject.SetActive(false);
            gameObject.GetComponentInChildren<BoxCollider>().isTrigger = true;

            Rigidbody body = gameObject.GetComponentInChildren<Rigidbody>();
            body.AddForce(220 * body.gameObject.transform.up);

            collecting = true;
        }
    }

    public override void OnRefresh()
    {
        interactableUI.SetProgress(1.0f);
        interactableUI.SetText("Collect Scrap");

        if (collecting)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (currentSpeed < finalSpeed) currentSpeed += Time.deltaTime / 4.0f;

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, currentSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target.transform.localRotation, 1.2f * Time.deltaTime);

            if (distance < 1.0f)
            {
                Instantiate(effect, transform.parent);
                Destroy(interactableUI.gameObject);
                Destroy(gameObject);
            }

            if (distance > 100.0f)
            {
                collecting = false;
                gameObject.GetComponentInChildren<Collider>().isTrigger = false;
                interactableUI.gameObject.SetActive(true);
            }
        }
    }

    public override void OnExitRange() { }
}
