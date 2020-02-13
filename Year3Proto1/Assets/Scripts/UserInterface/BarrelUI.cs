using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarrelUI : MonoBehaviour
{
    public TMP_Text tmpText;
    public Image image;
    public Transform barrel;

    private void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(barrel.position);
        image.transform.position = screenPoint;
        tmpText.transform.position = screenPoint;

        image.gameObject.SetActive(screenPoint.z > 0);
        tmpText.gameObject.SetActive(screenPoint.z > 0);

        //transform.LookAt(2 * barrel.position - Camera.main.transform.position);
    }
}
