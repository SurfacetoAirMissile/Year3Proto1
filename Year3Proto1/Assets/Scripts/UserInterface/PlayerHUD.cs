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

    float totalHealth = 5f;

    GameObject playerChassis;

    private void Start()
    {
        playerChassis = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        totalHealth = playerChassis.GetComponentInParent<HovercraftShared>().healthComponent.GetHealth();
        UpdateHealthUI(totalHealth);
    }

    private void UpdateHealthUI(float _health)
    {

        // for each healthbar
        for (int i = 0; i < healthBars.Length; i++)
        {
            if (totalHealth > (i + 1))
            {
                Vector3 scale = healthBars[i].rectTransform.localScale;
                scale.x = 1f;
                healthBars[i].rectTransform.localScale = scale;
            }
            else
            {
                Vector3 scale = healthBars[i].rectTransform.localScale;
                scale.x = Mathf.Clamp(_health - i, 0f, 1f);
                healthBars[i].rectTransform.localScale = scale;
            }
        }
    }
}
