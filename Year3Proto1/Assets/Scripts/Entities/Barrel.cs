using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : MonoBehaviour
{
    private readonly float totalFuel;
    private float currentFuel;

    private bool interacting = false;

    private Text text;

    private void Start()
    {
        currentFuel = totalFuel;
    }

    public void Update()
    {
        if(interacting)
        {
            currentFuel -= Time.deltaTime;
        }
    }

}
