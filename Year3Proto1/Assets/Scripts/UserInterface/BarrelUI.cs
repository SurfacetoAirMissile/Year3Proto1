using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BarrelUI : MonoBehaviour
{
    //public TMP_Text tmpText;
    //public Image image;
    public Transform barrel;
    public GameObject interactionPrompt;
    public GameObject interactionDot;
    public float fuelLevel = 1.0f;                     // 0-1 value

    private CanvasGroup canvas;
    private Image progressBar;

    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        progressBar = transform.Find("InteractionPrompt/Label/HoldCircleProgress").GetComponent<Image>();
    }

    private void LateUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(barrel.position);
        interactionPrompt.transform.position = screenPoint;
        interactionDot.transform.position = screenPoint;

        interactionPrompt.SetActive(screenPoint.z > 0);
        //interactionDot.SetActive(screenPoint.z > 0);              // Show Dot and hide prompt when out of range

        // Progress bar update
        progressBar.fillAmount = 1 - fuelLevel;

        //transform.LookAt(2 * barrel.position - Camera.main.transform.position);
    }
}
