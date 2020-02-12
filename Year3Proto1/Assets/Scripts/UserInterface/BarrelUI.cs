using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrelUI : MonoBehaviour
{
    public Image image;
    public Text text;


    private Transform parent;
    private Transform barrel;

    private void Start()
    {
        parent = GetComponentInParent<Transform>();
    }

    private void Update()
    {
        float yOffSet = barrel.transform.position.y + 1.5f;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(parent.position);

        Transform textTransform = text.transform;
        Transform imageTransform = image.transform;

        textTransform.position = new Vector3(screenPoint.x, screenPoint.y + yOffSet, screenPoint.x);
        imageTransform.position = new Vector3(screenPoint.x, screenPoint.y + yOffSet, screenPoint.x);
    }
}
