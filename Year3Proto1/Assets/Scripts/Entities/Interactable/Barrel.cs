using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : InteractableEntity
{
    private float currentFuel = 1000.0f;
    private readonly float totalFuel = 1000.0f;

    private float timeStamp;

    public override void OnRefresh()
    {
        interactableUI.SetProgress(currentFuel / totalFuel);

        if (currentFuel <= 0.0f) {
            interactableUI.SetText("Empty");
        } else {
            interactableUI.SetText("Fuel Ship");
        }
    }

    public override void OnInteract() {
        if (currentFuel > 0.0f) currentFuel -= Time.deltaTime * 100.0f;
    }

    public override void OnExitRange()
    {

    }
}
