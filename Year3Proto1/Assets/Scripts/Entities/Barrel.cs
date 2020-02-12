using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : MonoBehaviour
{
    //Object Data
    private bool interacting = false;
    private readonly float totalFuel = 1000.0f;
    private float currentFuel;

    //User Interface
    [SerializeField] private BarrelUI userInterface;


    private void Start()
    {
        currentFuel = totalFuel;
    }

    public void Update()
    {
        if (interacting)
        {
            if(!userInterface.gameObject.activeSelf) userInterface.gameObject.SetActive(true);
            userInterface.tmpText.text = "Fuel\n" + currentFuel + "/" + totalFuel;
            
        }
        else
        {
            if (userInterface.gameObject.activeSelf) userInterface.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Enter works");
            interacting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Exit works");
            interacting = false;
        }
    }

}
