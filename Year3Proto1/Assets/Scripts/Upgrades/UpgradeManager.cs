using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{

    public void Execute(UpgradeType upgradeType)
    {
        Debug.Log("Upgraded: " + upgradeType);
        switch (upgradeType)
        {
            case UpgradeType.ENGINE_SPEED:
                //player.speed = 10.0f;...
                break;
            case UpgradeType.WEAPON_CANNON:
                //player.speed = 10.0f;...
                break;
        }
    }
}
