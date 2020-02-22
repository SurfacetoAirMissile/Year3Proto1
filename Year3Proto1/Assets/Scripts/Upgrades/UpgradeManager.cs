using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    //References to player, guns.

    public void Execute(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.ENGINE_SPEED:
                //player.speed = 10.0f;...
                break;
        }
    }
}
