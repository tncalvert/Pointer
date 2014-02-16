using UnityEngine;
using System.Collections;

public class Cohesion : SteeringForce {

    public float neighborhoodRadius = 10f;
    public float cohesionTweak = 1.0f;

    private Rigidbody rb;

    public Cohesion(Rigidbody rb) {
        this.rb = rb;
    }

    public Vector3 GetSteeringForce(float maxVelocity) {

        Collider[] neighbors = Physics.OverlapSphere(rb.position,
            neighborhoodRadius, 1 << LayerMask.NameToLayer("Victims"));

        Vector3 com = Vector3.zero;
        foreach (Collider n in neighbors) {
            com += n.rigidbody.position;
        }
        com /= neighbors.Length;
        Vector3 currentPos = rb.position;
        Vector3 desiredVelocity =
            (com - currentPos).normalized * cohesionTweak;
        Vector3 result = desiredVelocity - rb.velocity;
        return result;
    }
}
