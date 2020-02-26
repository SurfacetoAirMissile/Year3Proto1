using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameWave : MonoBehaviour
{
    //User Interface
    public TMP_Text killText;
    public TMP_Text scrapText;
    public TMP_Text timeText;

    //Statistics
    private int kills;
    private int scrap;
    private int time;
    private int remaining;

    public void AddKill(int kills)
    {
        this.kills += kills;
    }

    public void AddScrap(int scrap)
    {
        this.scrap += scrap;
    }

    public void Refresh()
    {
        killText.text = kills.ToString();
        scrapText.text = scrap.ToString();
        timeText.text = (time / 60) + ":" + ((time / 60) % 60);
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
