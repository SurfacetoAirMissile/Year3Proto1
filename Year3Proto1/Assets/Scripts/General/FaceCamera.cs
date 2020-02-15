using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Vector3 offset;

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(offset);
    }
}
