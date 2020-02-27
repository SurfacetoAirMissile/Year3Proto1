using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    ENGINE_SPEED,
    ENGINE_FUEL,
    WEAPON_MINIGUN,
    WEAPON_MINIGUN_ROF,
    WEAPON_MORTAR,
    WEAPON_MORTAR_ROF,
    WEAPON_CANNON,
    WEAPON_WIND_CANNON
};

[System.Serializable]
public class UpgradeResourceEntity
{
    public string displayName, description;
    public int cost;
    public UpgradeType upgradeType;
    public Sprite icon;

    public int GetTierCost(int tier)
    {
        return cost * tier;
    }
}
