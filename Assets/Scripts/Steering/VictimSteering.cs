using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows the object containing this script to be moved by a series of steering forces
/// Requires that the object has a rigidbody on it
/// Class based on demonstrations and code by Prof. Kesselman
/// </summary>
public class VictimSteering : MonoBehaviour {

    /// <summary>
    /// The maximum forces that the steering behaviors can exert on the object
    /// </summary>
    public float maxForce = 40.0f;

    /// <summary>
    /// The maximum velocity that results from the application of a steering force
    /// </summary>
    public float maxVelocity = 80.0f;

	/// <summary>
	/// Distance the object can be from its target destination before it is considered to have arrived
	/// </summary>
	float minimumArrivalRadiusSqrd = 25.0f;
	
	/// <summary>
	/// Distance the object can be from the final point of its path before the path is considered complete
	/// </summary>
	float minimumCompleteArrivalRadiusSqrd = 9.0f;

    /// <summary>
    /// Object containing methods for steering
    /// </summary>
    SteeringBehaviors steeringBehaviors;

    /// <summary>
    /// Pathfinder to get A* path to new location
    /// </summary>
    PathFinder pathFinder = null;

    /// <summary>
    /// Holds path to follow
    /// </summary>
	List<Vector2> path = new List<Vector2>();

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

    void Start() {
        // Check to see if we have a rigid body
        try {
            GetComponent<Rigidbody>();

            steeringBehaviors = gameObject.AddComponent<SteeringBehaviors>();
            steeringBehaviors.targetPosition = rigidbody.position;

            GameObject pf = GameObject.Find("Pathfinder");
            if (pf) {
                try {
                    pathFinder = pf.GetComponent<PathFinder>();
                } catch {
                    Debug.Log("Pathfinder object does not have the correct script attached, and so path have not been generated");
                }
            }

        } catch {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

    }

    void Update() {

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }

        if (pathFinder) {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(cameraRay, out hit)) {
                    Vector2 start = new Vector2(this.rigidbody.position.x, this.rigidbody.position.z);
                    Vector2 end = new Vector2(hit.point.x, hit.point.z);
                    this.path = new List<Vector2>(this.pathFinder.getPath(start, end));
                }
            }
        }

		if (path.Count > 0) {
			// Check if we have arrived
			if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumArrivalRadiusSqrd) {
				// We are close enough, get the next goal
				steeringBehaviors.targetPosition = new Vector3(path[0].x, rigidbody.position.y, path[0].y);
				path.RemoveAt(0);

			}
			// else don't do anything, continue to let it path closer to goal
		} else {
			// Working on the last one
			// If we are there, stop
			if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumCompleteArrivalRadiusSqrd) {
				steeringBehaviors.targetPosition = rigidbody.position;

			}
		}

        Vector3 force = new Vector3(0, 0, 0);

        // Add forces
        //force += steeringBehaviors.GetAlignmentForce(maxVelocity) * behaviorWeights[0];
        force += steeringBehaviors.GetCohesionForce(maxVelocity) * behaviorWeights[1];
        //force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity) * behaviorWeights[2];
        //force += steeringBehaviors.GetFearForce(maxVelocity) * behaviorWeights[3];
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
