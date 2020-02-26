using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField]
    public float ringPosition = 180f;

    [SerializeField]
    public float xRotation = 10f;

    public float scrollOffset = 0f;
    public float scrollMax = 30f;
    public float scrollMin = -30f;
    [SerializeField]
    public float xRotationMin = 10f;
    [SerializeField]
    public float xRotationMax = 50f;
    [SerializeField]
    public float sitHeight = 0f;
    [SerializeField]
    public float sideSitHeight = 0f;

    [SerializeField]
    public float orbitRadius = 5f;

    [SerializeField]
    public GameObject cameraLookTarget;

    private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 90f;

    private bool mortarAimingMode = false;
    public Vector3 mortarAimTarget;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
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
        float mouseScroll = 0;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseX = Mathf.Clamp(Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
            mouseY = Mathf.Clamp(Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime, -50f, 50f);
            mouseScroll = Mathf.Clamp(Input.mouseScrollDelta.y, -50f, 50f);
        }

        xRotation += mouseY;
        ringPosition += mouseX;
        scrollOffset += mouseScroll;

        if (ringPosition > 360f) { ringPosition -= 360f; }
        if (ringPosition < 0f) { ringPosition += 360f; }

        if (xRotation > xRotationMax) { xRotation = xRotationMax; }
        if (xRotation < xRotationMin) { xRotation = xRotationMin; }

        if (scrollOffset > scrollMax) { scrollOffset = scrollMax; }
        if (scrollOffset < scrollMin) { scrollOffset = scrollMin; }

        Vector3 offset = Vector3.forward * orbitRadius;

        offset = Quaternion.AngleAxis(xRotation, Vector3.right) * offset;
        offset = Quaternion.AngleAxis(ringPosition, Vector3.up) * offset;

        transform.position = cameraLookTarget.transform.position + offset + Vector3.up * sitHeight;

        transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight);

        transform.position += transform.right * sideSitHeight;

        transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight + transform.right * sideSitHeight);

        // save this direction for the mortar
        mortarAimTarget = transform.forward;

        if (mortarAimingMode)
        {
            offset = Vector3.forward * orbitRadius;

            offset = Quaternion.AngleAxis(xRotation, Vector3.right) * offset;
            offset = Quaternion.AngleAxis(ringPosition, Vector3.up) * offset;

            transform.position = cameraLookTarget.transform.position + offset + Vector3.up * sitHeight;

            transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight);
            // save this direction for the mortar
            mortarAimTarget = transform.forward;

            offset = Vector3.forward * orbitRadius;
            offset = Quaternion.AngleAxis(ringPosition, Vector3.up) * offset;
            transform.position = cameraLookTarget.transform.position + offset + Vector3.up * sitHeight;

            transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight);

            transform.position += transform.right * sideSitHeight;

            transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight + transform.right * sideSitHeight);

            //Debug.Log(xRotation);
            /*if (xRotation > 0f)
            {
                transform.position += transform.up * xRotation * .07f;

                transform.LookAt(cameraLookTarget.transform.position + Vector3.up * sitHeight + transform.right * sideSitHeight + transform.up * xRotation * .07f);
            }*/
        }
    }

    public void LoadPreset(HovercraftShared.PlayerFocus _playerFocus)
    {
        switch (_playerFocus)
        {
            case HovercraftShared.PlayerFocus.ScimitarNone:
                orbitRadius = 4f;
                xRotationMin = -50f;
                xRotationMax = 5f;
                sitHeight = 0f;
                sideSitHeight = 0f;
                mortarAimingMode = false;
                break;
            case HovercraftShared.PlayerFocus.ScimitarMinigun:
                orbitRadius = 1f;
                xRotationMin = -30f;
                xRotationMax = 60f;
                sitHeight = 0.5f;
                sideSitHeight = 0f;
                mortarAimingMode = false;
                break;
            case HovercraftShared.PlayerFocus.ScimitarWindCannon:
                orbitRadius = 1f;
                xRotationMin = -30f;
                xRotationMax = 60f;
                sitHeight = 0.5f;
                sideSitHeight = 0f;
                mortarAimingMode = false;
                break;
            case HovercraftShared.PlayerFocus.TortoiseNone:
                orbitRadius = 7f;
                xRotationMin = -50f;
                xRotationMax = 5f;
                sitHeight = 0.7f;
                sideSitHeight = 0f;
                mortarAimingMode = false;
                break;
            case HovercraftShared.PlayerFocus.TortoiseMortar:
                orbitRadius = 5.0f;
                xRotationMin = -30f;
                xRotationMax = 60f;
                sitHeight = 1.7f;
                sideSitHeight = 0.7f;
                mortarAimingMode = true;
                break;
            case HovercraftShared.PlayerFocus.TortoiseWindCannon:
                orbitRadius = 3f;
                xRotationMin = -30f;
                xRotationMax = 60f;
                sitHeight = 1f;
                sideSitHeight = 0f;
                mortarAimingMode = false;
                break;
        }

    }
}
