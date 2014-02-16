using UnityEngine;
using System.Collections;

public class Separation : SteeringForce {

    public float neighborhoodRadius = 10f;
    public float separationTweak = 1.0f;

    private Rigidbody rb;

    public Separation(Rigidbody rb) {
        this.rb = rb;
    }

    public Vector3 GetSteeringForce(float maxVelocity) {
        Collider[] neighbors = Physics.OverlapSphere(rb.position,
            neighborhoodRadius, 1 << LayerMask.NameToLayer("Victims"));
        Vector3 result = Vector3.zero;
        foreach (Collider n in neighbors) {
            Vector3 vectorToUs = rb.position - n.rigidbody.position;
            if (vectorToUs.sqrMagnitude != 0) {
                result += vectorToUs.normalized / vectorToUs.magnitude;
            }
        }
        return result * separationTweak;
    }
}
