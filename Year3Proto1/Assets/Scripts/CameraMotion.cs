using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField]
    public float ringPosition = 180f;

    [SerializeField]
    public float xRotation = 10f;
    [SerializeField]
    public float xRotationMin = 10f;
    [SerializeField]
    public float xRotationMax = 50f;
    [SerializeField]
    public float sitHeight = 0f;

    [SerializeField]
    public float orbitRadius = 5f;

    [SerializeField]
    public GameObject cameraLookTarget;

    private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 90f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        float mouseX = 0;
        float mouseY = 0;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseX = Mathf.Clamp(Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
            mouseY = Mathf.Clamp(Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
        }

        xRotation -= mouseY;
        ringPosition += mouseX;

        if (ringPosition > 360f) { ringPosition -= 360f; }
        if (ringPosition < 0f) { ringPosition += 360f; }

        if (xRotation > xRotationMax) { xRotation = xRotationMax; }
        if (xRotation < xRotationMin) { xRotation = xRotationMin; }

        Vector3 offset = Vector3.forward * orbitRadius;

        offset = Quaternion.AngleAxis(-xRotation, Vector3.right) * offset;
        offset = Quaternion.AngleAxis(ringPosition, Vector3.up) * offset;

        transform.position = cameraLookTarget.transform.position + offset + Vector3.up * sitHeight;

        transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight);
    }
}
