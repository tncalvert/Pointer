using UnityEngine;
using System.Collections;

public class WallAvoidance : SteeringForce {

    public float feelerFactor = 1.0f;
    public float forceFactor = 3.0f;
    private float boundingRadius;
    public bool showFeelers = true;
    private Rigidbody rb;

    public WallAvoidance(Rigidbody rb, float boundingRadius) {
        this.rb = rb;
        this.boundingRadius = boundingRadius;
    }

    public Vector3 GetSteeringForce(float maxVelocity) {
        // find closest wall
        // calculate feelers
        Vector3[] feelers = new Vector3[3];
        float feelerLength = (rb.velocity.magnitude * feelerFactor) + boundingRadius;
        feelers[0] = rb.velocity.normalized * feelerLength;
        feelers[1] = (feelers[0] + new Vector3(-feelers[0].z, feelers[0].y, feelers[0].x))
            .normalized * feelerLength;
        feelers[2] = (feelers[0] + new Vector3(feelers[0].z, feelers[0].y, -feelers[0].x))
            .normalized * feelerLength;

        if (showFeelers) {
            Debug.DrawLine(rb.position, rb.position + feelers[0], Color.white);
            Debug.DrawLine(rb.position, rb.position + feelers[1], Color.white);
            Debug.DrawLine(rb.position, rb.position + feelers[2], Color.white);
        }

        // check for impacts
        Vector3 result = new Vector3(0, 0, 0);
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < 3; ++i) {
            if (Physics.Linecast(rb.position, rb.position + feelers[i],
                out hit, 1 << LayerMask.NameToLayer("City"))) {
                    result += hit.normal * ((feelers[i].magnitude - hit.distance) * forceFactor);
            }
        }

        if (showFeelers) {
            Debug.DrawLine(rb.position, rb.position + result, Color.yellow);
        }

        return result;
    }
}
