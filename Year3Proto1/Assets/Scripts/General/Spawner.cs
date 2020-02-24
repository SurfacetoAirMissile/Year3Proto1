using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    public Transform parent;

    public void Spawn()
    {
        Instantiate(enemies[Random.Range(0, enemies.Length)], parent);
    }
}
