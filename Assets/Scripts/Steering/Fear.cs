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
        List<GameObject> fearedObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag(fearedTag));
        
        // Sort objects by distance from player
        fearedObjects.Sort(delegate(GameObject x, GameObject y) {
            return (x.rigidbody.position - rb.position).sqrMagnitude
                .CompareTo((y.rigidbody.position - rb.position).sqrMagnitude);
        });
        // Removed all objects that are out of range
        fearedObjects.RemoveAll(x => (x.rigidbody.position - rb.position).sqrMagnitude > fearDistanceSquared);

        // calculate the denominator and the first numerator
        float num = fearedObjects.Count;
        float den = (num * (num + 1)) / 2;

        Vector3 desiredVelocity = new Vector3(0, 0, 0);

        foreach(GameObject f in fearedObjects) {
            // Calculate the vector away from the feared object, clamped to
            // max velocity and then weighted by its position in the list
            desiredVelocity += 
                ((rb.position - f.rigidbody.position).normalized * maxVelocity) *
                (num-- / den);
        }

        return (desiredVelocity - rb.velocity);
    }

}
