using UnityEngine;
using System.Collections;

public class Alignment : SteeringForce {

    public float neighborRadius = 10f;
    public float alignmnetTweak = 1.0f;

    private Rigidbody rb;

    public Alignment(Rigidbody rb) {
        this.rb = rb;
    }

    public Vector3 GetSteeringForce(float maxVelocity) {
        Collider[] neighbors = Physics.OverlapSphere(rb.position,
            neighborRadius, 1 << LayerMask.NameToLayer("Victims"));
        Vector3 avgHeading = Vector3.zero;
        foreach (Collider n in neighbors) {
            avgHeading += n.rigidbody.velocity;
        }
        avgHeading /= neighbors.Length;
        return (avgHeading - rb.velocity) * alignmnetTweak;
    }
}
