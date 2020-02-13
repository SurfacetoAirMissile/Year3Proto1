using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    public string displayname;
    public float level = 0.0f;

    public GameObject prompt;
    public GameObject dot;
    public GameObject progress;

    public Transform target;

    private void LateUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
        prompt.transform.position = screenPoint;
        dot.transform.position = screenPoint;

        float distanceFromObject = Vector3.Distance(Camera.main.transform.position, target.position);

        prompt.SetActive(screenPoint.z > 0.0f && distanceFromObject < 2.0f);
        dot.SetActive(screenPoint.z > 0.0f && distanceFromObject > 2.0f);
    }
}
