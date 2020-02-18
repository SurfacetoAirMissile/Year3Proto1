using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOutPost : InteractableEntity
{
    //Different Inventory
    public GameObject panel;

    private bool open = false;

    public override void OnRefresh()
    {
        interactableUI.SetProgress(1.0f);
        interactableUI.SetText("Trade");
    }

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

    public override void OnInteract()
    {
        if (!open)
        {
            open = true;
            panel.SetActive(open);
        }
    }

    public override void OnExitRange()
    {
        open = false;
        panel.SetActive(false);
    }
}
