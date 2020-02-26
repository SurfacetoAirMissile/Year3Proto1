using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthComponent
{
    [SerializeField] [Tooltip("Entity Health.")]
    private float health;
    private float maxHealth;
    private string latestAttacker;
    private string killer;

    public HealthComponent(float _health)
    {
        health = _health;
    }

    public void DeductHealth(float _amount)
    {
        health -= _amount;
    }

    public void SetHealth(float _health, bool _andMax)
    {
        health = _health;
        if (_andMax)
        {
            SetMaxHealth(_health);
        }
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetHealthRelative()
    {
        return health / maxHealth;
    }

    public void SetMaxHealth(float _health)
    {
        maxHealth = _health;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetLatestAttacker(string _name)
    {
        latestAttacker = _name;
    }

    public string GetLatestAttacker()
    {
        return latestAttacker;
    }

    public void DealDamage(float _amount, string _attackerName)
    {
        bool aliveBefore = false;
        if (health > 0f)
        {
            aliveBefore = true;
        }
        DeductHealth(_amount);
        if (aliveBefore && health <= 0f)
        {
            killer = _attackerName;
        }
        SetLatestAttacker(_attackerName);
    }

    public string GetKiller()
    {
        if (health <= 0f)
        {
            return killer;
        }
        else
        {
            return "This HealthComp is still kicking.";
        }
    }

    public void RestoreToFull()
    {
        health = maxHealth;
    }
}
