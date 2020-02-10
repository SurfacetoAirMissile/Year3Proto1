using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Hoverballs;

    [SerializeField]
    private float HoverForce;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject ball in Hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 2.0f))
            {
                ball.GetComponent<Rigidbody>().AddForce(Time.deltaTime * HoverForce / Mathf.Pow(groundCast.distance, 2.0f) * Vector3.up);

            }
        }

        //GetComponent<Rigidbody>().AddForce(Time.del)
    }
}
