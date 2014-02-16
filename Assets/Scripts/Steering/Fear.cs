using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fear : SteeringForce {

    /// <summary>
    /// The tag that this force will avoid
    /// </summary>
    public string fearedTag = "feared";

    /// <summary>
    /// Objects rigidbody for position
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// Radius around this object to look for feared objects
    /// </summary>
    public float fearRadius = 10.0f;

    /// <summary>
    /// Constructor. Sets rigidbody
    /// </summary>
    /// <param name="rb">Object's rigidbody</param>
    public Fear(Rigidbody rb) {
        this.rb = rb;
    }

    /// <summary>
    /// Changes the feared tag
    /// </summary>
    /// <param name="tag">New tag to avoid</param>
    public void SetFearedTag(string tag) {
        fearedTag = tag;
    }

    /// <summary>
    /// Calculates the steering force needed to avoid the feared object. Will run from
    /// all feared objects within the fear radius, will closer objects being weighted (significantly)
    /// heavier
    /// </summary>
    /// <param name="maxVelocity">Maximum possible velocity to be applied</param>
    /// <returns></returns>
    public Vector3 GetSteeringForce(float maxVelocity) {
        float fearDistanceSquared = fearRadius * fearRadius;

        GameObject fearedObject = GameObject.FindGameObjectWithTag(fearedTag);

        Vector3 desiredVelocity;

        if ((fearedObject.rigidbody.position - rb.position).sqrMagnitude > fearDistanceSquared) {
            // Too far away
            desiredVelocity = new Vector3(0, 0, 0);
        } else {
            desiredVelocity = ((rb.position - 
                (fearedObject.rigidbody.position + fearedObject.rigidbody.velocity))
                .normalized * maxVelocity);
        }
        Debug.Log(desiredVelocity - rb.velocity);
        return (desiredVelocity - rb.velocity);
    }

}
