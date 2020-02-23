using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Scrap Counter")]
    public TMP_Text scrap;

    [Header("death Counter")]
    public TMP_Text death;

    public Image[] healthBars;

    int totalHealth = 5;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) totalHealth -= 1;
        for (int i = 0; i < healthBars.Length; i++)
        {
            if (totalHealth > (i))
            {
                healthBars[i].fillAmount = 1.0f;
            }
            else
            {
                healthBars[i].fillAmount = 0.0f;
            }
            int[] color = GetColor(totalHealth * 20);
            healthBars[i].color = new Color(color[0], color[1], color[2]);
        }
    }

    public int[] GetColor (int index)
    {
        int R = (255 * index) / 100;
        int G = (255 * (100 - index)) / 100;
        int B = 0;
        return new int[] { R, G, B };
    }
}
