using UnityEngine;
using System.Collections;

public class CollisionAvoidance : SteeringForce {

    /// <summary>
    /// Bounding radius for our collision sphere
    /// </summary>
    private float boundingRadius;

    /// <summary>
    /// Multiplier for velocity to help guess collisions ahead of time
    /// </summary>
    public float velocityMult = 2.0f;

    /// <summary>
    /// Object's rigidbody
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Center of object
    /// </summary>
    Vector3 rendererCenter;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rb">Object's rigibody</param>
    /// <param name="boundingRadius">Radius of object</param>
    public CollisionAvoidance(Rigidbody rb, float boundingRadius, Vector3 rendererCenter) {
        this.boundingRadius = boundingRadius;
        this.rb = rb;
        this.rendererCenter = rendererCenter;
    }

    /// <summary>
    /// Calculates the steering force needed to avoid collisions with objects
    /// near. Only has an effect when going towards a corner.
    /// </summary>
    /// <param name="maxVelocity">Maximum possible velocity to be applied</param>
    /// <returns></returns>
	public Vector3 GetSteeringForce(float maxVelocity) {

        RaycastHit rayHit = new RaycastHit();
        // Don't want a layer?? Collide with building and other victims/player
        if (Physics.SphereCast(rendererCenter +
            (rb.velocity.normalized * boundingRadius), boundingRadius,
            rb.velocity.normalized, out rayHit, rb.velocity.magnitude * velocityMult,
            Physics.kDefaultRaycastLayers)) {
                Vector3 collisionDirection = (rayHit.point - rayHit.collider.bounds.center).normalized;
                return collisionDirection * maxVelocity;
        }

        return new Vector3(0, 0, 0);
    }

    public void UpdateRendererCenter(Vector3 rendererCenter) {
        this.rendererCenter = rendererCenter;
    }
}
