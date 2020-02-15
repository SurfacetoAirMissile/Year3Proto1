using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryArc : MonoBehaviour
{
    private LineRenderer line;

    public GameObject projectile;
    public float velocity = 10.0f;
    public int resolution = 15;

    private float gravity;
    private float angle;
    private float radAngle;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    void Start()
    {
        UpdateArc();
    }

    void Update()
    {
        angle = -transform.localEulerAngles.x + 360;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var shell = Instantiate(projectile, transform).GetComponent<Rigidbody>();

            shell.velocity = new Vector3(0.0f, 0.0f, velocity);

            Debug.Log(angle);
        }

        UpdateArc();
    }

    public void UpdateArc()
    {
        radAngle = Mathf.Deg2Rad * angle;

        line.positionCount = resolution + 1;
        line.SetPositions(CalculateArc());
    }

    private Vector3[] CalculateArc()
    {
        Vector3[] arc = new Vector3[resolution + 1];

        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radAngle)) / gravity;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arc[i] = CalculatePoint(t, maxDistance);
        }

        return arc;
    }

    private Vector3 CalculatePoint(float t, float maxDistance)
    {
        float l = t * maxDistance;

        float x = Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.y) * l;
        float y = l * Mathf.Tan(radAngle) - (gravity * l * l / (2 * velocity * velocity * Mathf.Cos(radAngle) * Mathf.Cos(radAngle)));
        float z = Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.y) * l;

        return new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
    }
}
