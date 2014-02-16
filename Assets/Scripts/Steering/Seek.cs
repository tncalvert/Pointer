using UnityEngine;
using System.Collections;

public class Seek : SteeringForce {

    Vector3 targetPosition;
    Rigidbody rb;

    public Seek(Rigidbody rb) {
        this.rb = rb;
    }

    public void SetSeekPosition(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }

    public Vector3 GetSteeringForce(float maxVelocity) {
        Vector3 desiredVelocity =
            (targetPosition - rb.position).normalized * maxVelocity;
        return desiredVelocity - rb.velocity;
    }
}
