using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustEffectController : MonoBehaviour
{
    private Rigidbody rb;
    private ParticleSystem part;
    private bool closeToGround;
    public int speedMultiplier = 4;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        part = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var em = part.emission;
        em.rateOverTime = rb.velocity.magnitude * speedMultiplier;
        closeToGround = false;
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (Physics.Raycast(sphere.transform.position, Vector3.down, out RaycastHit groundCast, 2f))
        {
            closeToGround = true;
        }
        if (closeToGround)
        {
            part.Play();
        }
        else
        {
            part.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
