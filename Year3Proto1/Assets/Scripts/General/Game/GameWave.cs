using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameWave : MonoBehaviour
{
    //User Interface
    public TMP_Text killText;
    public TMP_Text scrapText;
    public TMP_Text timeText;

    //Statistics
    private int wave;
    private int kills;
    private int scrap;
    private int time;
    private int remaining;

    public void AddKill(int kills)
    {
        killText.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
        this.kills += kills;
        killText.text = kills.ToString();
    }

    public void AddScrap(int scrap)
    {
        scrapText.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.0f), 0.33f, 1, 1.0f);
        this.scrap += scrap;
        scrapText.text = scrap.ToString();
    }

    public int GetScrap()
    {
        return scrap;
    }
    
    public int GetKills()
    {
        return kills;
    }

    public int GetTime()
    {
        return time;
    }

    public int GetRemaining()
    {
        return remaining;
    }
}
