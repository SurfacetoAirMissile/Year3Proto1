﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : InteractableEntity
{
    private float currentfuel;
    private readonly float totalFuel = 1000.0f;

    public override void Refresh()
    {
        interactableUI.SetProgress(currentfuel / totalFuel);
        interactableUI.SetText("Fuel Ship");
            
    }
}