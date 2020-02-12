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
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        /*
        float yOffSet = barrel.transform.position.y + 1.5f;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(barrel.position);

        Transform textTransform = tmpText.transform;
        Transform imageTransform = image.transform;

        float alpha = (textTransform.position.z < 0) ? 0.0f : 1.0f;
        canvasGroup.alpha = alpha;
        bool imageIsActive = (imageTransform.position.z < 0) ? true : false;



        transform.position = new Vector3(screenPoint.x, screenPoint.y + yOffSet, screenPoint.x);
        */

        //transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
}
