using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOutPost : InteractableEntity
{
    //Different Inventory
    public ResourceEntity[] resourceLoad;

    private List<ResourceEntity> resourceCache;

    public bool Buy(ResourceEntity resourceEntity, int amount)
    {
        long buyPrice = resourceEntity.price * amount;

        //Check how much money player has...

        return true;
    }

    public bool Sell(ResourceEntity resourceEntity, int amount)
    {
        long sellPrice = resourceEntity.price * amount;

        //Check how much player has of the resource...

        return true;
    }
}
