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
    public TMP_Text waveText;

    //Statistics
    public int wave;
    public int kills;
    public int scrap;

    public void Refresh()
    {
        killText.text = kills.ToString();
        scrapText.text = scrap.ToString();
        waveText.text = "Wave " + wave.ToString() + " Stats";
    }
}
