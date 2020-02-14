using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustEffectController : MonoBehaviour
{
    private Rigidbody rb;
    private ParticleSystem part;
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
        em.rateOverTime = rb.velocity.magnitude * 4;
    }
}
