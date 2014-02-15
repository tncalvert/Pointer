using UnityEngine;
using System.Collections;

/// <summary>
/// Interface to define a steering force
/// Based on demonstration and code by Prof. Kesselman
/// </summary>
public interface SteeringForce {

    /// <summary>
    /// Calculates the force applied by this steering force
    /// </summary>
    /// <param name="maxVelocity">The maximum velocity of the vector calculated</param>
    /// <returns>Returns a vector representing the force applied to an object, limited to maxVelocity</returns>
    Vector3 GetSteeringForce(float maxVelocity);

}
