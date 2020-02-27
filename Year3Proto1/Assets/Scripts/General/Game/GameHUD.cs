using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text header;
    public TMP_Text footer;

    [Header("Statistics")]
    public TMP_Text scrap;
    public TMP_Text kills;

    [Header("Health Bar")]
    public Image[] healthBars;
    public float totalHealth = 5f;

    GameObject playerChassis;

    private void Start()
    {
        playerChassis = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        float health = playerChassis.GetComponentInParent<HovercraftShared>().healthComponent.GetHealth();
        if (playerChassis != null)
        {
            for (int i = 0; i < healthBars.Length; i++)
            {
                if (health > (i + 1))
                {
                    Vector3 scale = healthBars[i].rectTransform.localScale;
                    scale.x = 1f;
                    healthBars[i].rectTransform.localScale = scale;
                }
                else
                {
                    Vector3 scale = healthBars[i].rectTransform.localScale;
                    scale.x = Mathf.Clamp(health - i, 0f, 1f);
                    healthBars[i].rectTransform.localScale = scale;
                }
            }
        }
    }

    public void Refresh(GameState gameState, int scrap, int kills, int remaining, int wave)
    {
        switch (gameState)
        {
            case GameState.INTERVAL:
                header.text = "INTERVAL";
                this.footer.text = remaining.ToString() + " Seconds Remain";
                break;
            case GameState.INGAME:
                header.text = "WAVE " + wave;
                this.footer.text = remaining.ToString() + " Enemies Remain";
                break;
        }

        this.scrap.text = scrap.ToString();
        this.kills.text = kills.ToString();
    }
}
