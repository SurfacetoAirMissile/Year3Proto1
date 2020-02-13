using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : InteractableEntity
{
    private float currentfuel;
    private readonly float totalFuel = 1000.0f;

    private void Start()
    {
        UserInterface().displayname = "Fuel Ship";
    }

    private void Update()
    {
        UserInterface().level = currentfuel / totalFuel;
    }
}
