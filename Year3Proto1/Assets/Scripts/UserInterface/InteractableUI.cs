using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    public GameObject prompt, dot, progress, icon;
    public Transform target;

    private void LateUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        prompt.transform.position = screenPoint;
        dot.transform.position = screenPoint;

        float distanceFromObject = Vector3.Distance(Camera.main.transform.position, target.position);

        prompt.SetActive(screenPoint.z > 0.0f && distanceFromObject < 10.0f);
        dot.SetActive(screenPoint.z > 0.0f && distanceFromObject > 10.0f);
    }

    public void SetText(string text)
    {
        prompt.GetComponentInChildren<TMP_Text>().text = text;
    }

    public void SetProgress(float progress)
    {
        this.progress.GetComponent<Image>().fillAmount = progress;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.GetComponent<Image>().sprite = sprite;
    }
}
