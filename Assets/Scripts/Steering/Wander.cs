using UnityEngine;
using System.Collections;

public class Wander : SteeringForce {

    /// <summary>
    /// Radius off of straight
    /// </summary>
    public float wanderRadius = 2f;
    /// <summary>
    /// How far ahead the wandering looks
    /// </summary>
    public float wanderDistance = 4f;
    /// <summary>
    /// Random factor of wander
    /// </summary>
    public float wanderJitter = 25f;

    /// <summary>
    /// Object's rigidbody
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The last wander target stored in object position relative coordinates
    /// </summary>
    Vector3 wanderTarget;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rb">Rigidbody</param>
    public Wander(Rigidbody rb) {
        wanderTarget = new Vector3(0, 0, wanderRadius);
        this.rb = rb;
    }

    /// <summary>
    /// Calculates the steering force for a small wandering factor
    /// </summary>
    /// <param name="maxVelocity">Maximum possible velocity to be applied</param>
    /// <returns></returns>
    public Vector3 GetSteeringForce(float maxVelocity) {
        Vector3 targetJitter = new Vector3(NormalizedRandom() * wanderJitter,
            0, NormalizedRandom() * wanderJitter);
        wanderTarget += targetJitter;
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        Vector3 localTarget = wanderTarget + rb.velocity.normalized * wanderDistance;
        Debug.Log(localTarget);
        return localTarget;
    }

    /// <summary>
    /// Generates a random number between -1 and 1
    /// </summary>
    /// <returns>The random number</returns>
    private float NormalizedRandom() {
        return Random.Range(-1f, 1f);
    }
}
