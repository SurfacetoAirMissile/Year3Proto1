using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    public Transform[] positions;
    public GameObject[] enemies;

    public void Spawn(int amount)
    {
        if (positions.Length == 0 || enemies.Length == 0) return;
        for (int i = 0; i < amount; i++)
        {
            GameObject gameObject = Instantiate(enemies[Random.Range(0, enemies.Length)], transform);
            gameObject.transform.position = positions[Random.Range(0, positions.Length)].position;
        }
    }
}
