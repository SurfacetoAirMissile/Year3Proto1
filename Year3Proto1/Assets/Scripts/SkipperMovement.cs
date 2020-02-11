using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipperMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Hoverballs;

    [SerializeField]
    private float HoverForce;

    private Rigidbody thisRB;

    // Start is called before the first frame update
    void Start()
    {
        thisRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject ball in Hoverballs)
        {
            // Get the distance between that ball and the ground
            if (Physics.Raycast(ball.transform.position, Vector3.down, out RaycastHit groundCast, 2f))
            {
                ball.GetComponent<Rigidbody>().AddForce(Time.deltaTime * HoverForce / Mathf.Clamp(Mathf.Pow(groundCast.distance, 2f), 0f, 1f)  * Vector3.up);

            }
        }

        //thisRB.AddForce(Time.deltaTime * 5f *  HoverForce * -thisRB.velocity);
        //thisRB.AddTorque(Time.deltaTime * )

        if (Input.GetKey("w"))
        {
            thisRB.AddForce(Time.deltaTime * HoverForce * 30f * Vector3.forward);
        }
    }
}
