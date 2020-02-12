using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOutPost : MonoBehaviour
{
    //Different Inventory
    public Resource[] resources;

    public bool Buy(Resource resource, int amount)
    {
        long buyPrice = resource.price * amount;

        //Check how much money player has...

        return true;
    }

    public bool Sell(Resource resource, int amount)
    {
        long sellPrice = resource.price * amount;

        //Check how much player has of the resource...

        return true;
    }
}
