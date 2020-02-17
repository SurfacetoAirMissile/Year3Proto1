using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    private float fuel = 1000.0f;
    private int scrap = 0;
    private float health = 100.0f;

    private void Update()
    {
        fuel -= Time.deltaTime * 2.0f;
    }

    public void AddScrap(int scrap)
    {
        this.scrap += scrap;
    }

    public float GetFuel()
    {
        return fuel;
    }

    public int GetScrap()
    {
        return scrap;
    }
}
