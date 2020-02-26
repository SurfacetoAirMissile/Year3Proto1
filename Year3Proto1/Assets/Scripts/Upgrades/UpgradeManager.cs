using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{

    public void Execute(UpgradeType upgradeType, int _cost)
    {
        GameObject player = GameObject.FindWithTag("Player").transform.parent.gameObject;
        HovercraftShared playerScript = player.GetComponent<HovercraftShared>();
        if (!playerScript.installedUpgrades.Contains(upgradeType))
        {
            if (GameManager.Instance.playerScrap >= _cost)
            {
                GameManager.Instance.playerScrap -= _cost;
                playerScript.InstallUpgrade(upgradeType);
                Debug.Log("Upgraded: " + upgradeType);
            }
            else
            {
                Debug.Log("Player cannot afford upgrade: " + upgradeType);
            }
        }
        else
        {
            Debug.Log("Player already owns upgrade: " + upgradeType);
        }
        
        
        /*
        switch (upgradeType)
        {
            case UpgradeType.ENGINE_SPEED:
                //player.speed = 10.0f;...
                break;
            case UpgradeType.WEAPON_CANNON:
                //player.speed = 10.0f;...
                break;
        }
        */
    }
}
