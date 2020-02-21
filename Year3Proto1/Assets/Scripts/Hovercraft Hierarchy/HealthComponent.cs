using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthComponent
{
    [SerializeField] [Tooltip("Entity Health.")]
    private float health;

    public HealthComponent(float _health)
    {
        health = _health;
    }

    public void DeductHealth(float _amount)
    {
        health -= _amount;
    }

    public float GetHealth()
    {
        return health;
    }
}
