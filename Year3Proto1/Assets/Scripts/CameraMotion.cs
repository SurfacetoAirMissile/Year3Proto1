using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField]
    private GameObject skipper;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = skipper.transform.position - skipper.transform.forward * 3f + skipper.transform.up;
        transform.rotation = skipper.transform.rotation;
    }
}
