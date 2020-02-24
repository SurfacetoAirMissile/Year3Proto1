using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeInformation : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text description;
    public Image icon;

    public void Refresh(UpgradeResourceEntity upgradeResourceEntity)
    {
        itemName.text = upgradeResourceEntity.displayName;
        description.text = upgradeResourceEntity.description;
        icon.sprite = upgradeResourceEntity.icon;
    }
}
