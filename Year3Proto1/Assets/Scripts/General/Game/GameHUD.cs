using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text header;
    public TMP_Text footer;

    [Header("Statistics")]
    public TMP_Text scrap;
    public TMP_Text kills;

    public void Refresh(GameState gameState, int scrap, int kills, int remaining, int wave)
    {
        switch(gameState)
        {
            case GameState.GRACE_PERIOD:
                header.text = "GRACE PERIOD";
                this.footer.text = remaining.ToString() + "Seconds Remain";
                break;
            case GameState.INGAME:
                header.text = "WAVE " + wave;
                this.footer.text = remaining.ToString() + "Enemies Remain";
                break;
        }

        this.scrap.text = scrap.ToString();
        this.kills.text = kills.ToString();
    }
}
