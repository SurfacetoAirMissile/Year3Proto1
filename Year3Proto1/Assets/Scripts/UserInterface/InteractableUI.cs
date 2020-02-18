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

    private Vector3 offset = new Vector3(0.0f, 20.0f, 0.0f);
    private bool range = false;

    private void LateUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        prompt.transform.position = screenPoint + offset;
        dot.transform.position = screenPoint + offset;

        float distanceFromObject = Vector3.Distance(Camera.main.transform.position, target.position);

        if (distanceFromObject < (15.0f)) range = true;

        prompt.SetActive(screenPoint.z > 0.0f && distanceFromObject < (15.0f * transform.localScale.x));
        dot.SetActive(screenPoint.z > 0.0f && distanceFromObject > (15.0f * transform.localScale.x));
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

    public bool InRange()
    {
        return range;
    }
}
