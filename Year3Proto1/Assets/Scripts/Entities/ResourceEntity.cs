using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ResourceEntity : MonoBehaviour
{
    //Variables
    public int price;
    public int tier;
    public new string name;
    public Sprite icon;

    //User Interface;
    public Image imageIcon;
    public TMP_Text itemName;
    public TMP_Text scrapCounter;
    public UnityEvent unityEvent;

    private void Start()
    {
        scrapCounter.text = price.ToString();
        imageIcon.sprite = icon;
        itemName.text = name;
    }
}
