using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Fuel Meter")]
    public Transform needle;
    public PlayerEntity playerEntity;

    private const float MIN_ANGLE = 45.0f;
    private const float MAX_ANGLE = -45.0f;

    [Header("Scrap Counter")]
    public TMP_Text label;

    //Compass
    [Header("Compass")]
    public float pixelsToNorth;
    public GameObject target;

    private Vector3 startPosition;
    private float rotationAngle;

    private void Start()
    {
        //Compass
        startPosition = transform.position;
        rotationAngle = pixelsToNorth / 360.0f;
    }

    private void Update()
    {
        //Fuel Meter
        needle.eulerAngles = new Vector3(0.0f, 0.0f, MIN_ANGLE - (MIN_ANGLE - MAX_ANGLE) * (playerEntity.GetFuel() / 1000.0f));

        //Scrap Counter
        label.text = playerEntity.GetScrap().ToString();

        //Compass
        Vector3 cross = Vector3.Cross(Vector3.forward, target.transform.forward);
        float angle = Vector3.Angle(target.transform.forward, Vector3.forward);
        float direction = Vector3.Dot(cross, Vector3.up);

        transform.position = startPosition + (new Vector3(angle * Mathf.Sign(direction) * rotationAngle, 0, 0));
    }
}
