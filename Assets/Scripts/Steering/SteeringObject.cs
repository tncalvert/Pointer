using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows the object containing this script to be moved by a series of steering forces
/// Requires that the object has a rigidbody on it
/// Class based on demonstrations and code by Prof. Kesselman
/// </summary>
public class SteeringObject : MonoBehaviour {

    /// <summary>
    /// The maximum forces that the steering behaviors can exert on the object
    /// </summary>
    public float maxForce = 5.0f;

    /// <summary>
    /// The maximum velocity that results from the application of a steering force
    /// </summary>
    public float maxVelocity = 10.0f;

    /// <summary>
    /// Object containing methods for steering
    /// </summary>
    SteeringBehaviors steeringBehaviors;

    /// <summary>
    /// Weights for behaviors
    /// 0: Alignment
    /// 1: Cohesion
    /// 2: Collision Avoidance
    /// 3: Fear
    /// 4: Seek
    /// 5: Separation
    /// 6: Wall Avoidance
    /// 7: Wander
    /// </summary>
    public float[] behaviorWeights = new float[8] { 1.1f,   // Alignment
                                                    1.2f,   // Cohesion
                                                    1.5f,   // Collision Avoidance
                                                    2.0f,   // Fear
                                                    1.0f,   // Seek
                                                    1.5f,   // Separation
                                                    1.5f,   // Wall Avoidance
                                                    0.6f    // Wander
    };

	void Start () { 
        // Check to see if we have a rigid body
        try {
            GetComponent<Rigidbody>();

            steeringBehaviors = gameObject.AddComponent<SteeringBehaviors>();

        } catch (MissingComponentException e) {
            Debug.Log("Object " + this.name + " does not have a Rigidbody.\n" + e.Message);
        }
        
    }
	
	void Update () {

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }

        Vector3 force = new Vector3(0, 0, 0);

        if (Input.GetMouseButton(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                Debug.Log("Clicked");
                steeringBehaviors.targetPosition = hit.point;
            }
        }

        // Add forces
        force += steeringBehaviors.GetAlignmentForce(maxVelocity) * behaviorWeights[0];
        force += steeringBehaviors.GetCohesionForce(maxVelocity) * behaviorWeights[1];
        force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity) * behaviorWeights[2];
        force += steeringBehaviors.GetFearForce(maxVelocity) * behaviorWeights[3];
        force += steeringBehaviors.GetSeekForce(maxVelocity) * behaviorWeights[4];
        force += steeringBehaviors.GetSeparationForce(maxVelocity) * behaviorWeights[5];
        force += steeringBehaviors.GetWallAvoidanceForce(maxVelocity) * behaviorWeights[6];
        force += steeringBehaviors.GetWanderForce(maxVelocity) * behaviorWeights[7];

        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force
        rigidbody.AddForce(force - rigidbody.velocity);
        
	}
}
