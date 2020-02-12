using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrel : MonoBehaviour
{
    //Object Data
    private bool interacting = true;
    private readonly float totalFuel;
    private float currentFuel;

    //User Interface
    [SerializeField] private GameObject userInterface;
    [SerializeField] private Transform userInterfaceParent;
    private GameObject userInterfaceFinal;


    private void Start()
    {
        currentFuel = totalFuel;
    }

    public void Update()
    {
        if(interacting) 
        {
            if (userInterfaceFinal == null)
            {
                userInterfaceFinal = Instantiate(userInterface, userInterfaceParent, true);
                userInterfaceFinal.GetComponent<BarrelUI>().SetBarrel(transform);
            }
            else
            {
                userInterfaceFinal.GetComponent<BarrelUI>().text.text = currentFuel + "/" + totalFuel;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetType() == typeof(Player))
        {

        }
    }

}
