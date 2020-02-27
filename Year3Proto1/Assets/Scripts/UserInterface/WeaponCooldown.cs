using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownEntity : MonoBehaviour
{
    public Image icon;
    public Image overlay;

    public int time;
    public int maxTime;

    private void Update()
    {
        overlay.fillAmount = time / maxTime;
    }
}
