using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeResource : MonoBehaviour
{
    //User Interface
    public Image imageIcon;
    public TMP_Text itemName;
    public TMP_Text cost;
    public UpgradeType upgradeType;

    private UpgradeResourceEntity upgradeResourceEntity;

    public UpgradeResource Initialise(UpgradeResourceEntity upgradeResourceEntity)
    {
        this.upgradeResourceEntity = upgradeResourceEntity;

        itemName.text = upgradeResourceEntity.displayName;
        imageIcon.sprite = upgradeResourceEntity.icon;
        cost.text = upgradeResourceEntity.cost.ToString();
        upgradeType = upgradeResourceEntity.upgradeType;

        return this;
    }

    public UpgradeResourceEntity GetEntity()
    {
        return upgradeResourceEntity;
    }
}
