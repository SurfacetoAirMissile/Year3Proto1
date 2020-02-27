using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public Image icon;
    public Image overlay;

    public float time = 0.0f;
    public float maxTime = 10.0f;

    private void Update()
    {
        time += Time.deltaTime;
        if(time < maxTime) overlay.fillAmount = time / maxTime;
    }
}
