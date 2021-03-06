﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows the object containing this script to be moved by a series of steering forces
/// Requires that the object has a rigidbody on it
/// Class based on demonstrations and code by Prof. Kesselman
/// </summary>
public class PlayerSteering : MonoBehaviour {


    /// <summary>
    /// The maximum forces that the steering behaviors can exert on the object
    /// </summary>
    public float maxForce = 40.0f;

    /// <summary>
    /// The maximum velocity that results from the application of a steering force
    /// </summary>
    public float maxVelocity = 80.0f;

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
    Queue<Vector2> path = new Queue<Vector2>();

    /// <summary>
    /// Distance the object can be from its target destination before it is considered to have arrived
    /// </summary>
    float minimumArrivalRadiusSqrd = 9.0f;

    /// <summary>
    /// Distance the object can be from the final point of its path before the path is considered complete
    /// </summary>
    float minimumCompleteArrivalRadiusSqrd = 9.0f;


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
                                                    1.0f,   // Collision Avoidance
                                                    2.0f,   // Fear
                                                    1.2f,   // Seek
                                                    1.5f,   // Separation
                                                    1.0f,   // Wall Avoidance
                                                    0.6f    // Wander
    };


	void Start () { 
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

            Camera.main.GetComponent<CameraIso>().setup(this.gameObject.transform);

        } catch {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
        
    }
	
	void FixedUpdate () {

		if (Inspector.Inspecting) {
			this.rigidbody.velocity -= this.rigidbody.velocity; //TODO remember old velocity when !inspecting
			return;
		}

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }


        // Keep object rotated in the correct direction
        // *** BUGGY ***
        //rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, Quaternion.LookRotation(-rigidbody.velocity, Vector3.up), Time.deltaTime * 100f);

        Vector3 force = new Vector3(0, 0, 0);

        if (pathFinder) {
            if (Input.GetMouseButton(0)) {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(cameraRay, out hit,
                    ((1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Sidewalk"))))) {
                    Vector2 start = new Vector2(this.rigidbody.position.x, this.rigidbody.position.z);
                    Vector2 end = new Vector2(hit.point.x, hit.point.z);
                    path = new Queue<Vector2>(pathFinder.getPath(start, end));
                    Vector2 newLocation = path.Dequeue();
                    steeringBehaviors.targetPosition = new Vector3(newLocation.x, rigidbody.position.y, newLocation.y);
                }
            }
        }

        updatePath();

        // Add forces
        force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity) * behaviorWeights[2];
        force += steeringBehaviors.GetSeekForce(maxVelocity) * behaviorWeights[4];
        force += steeringBehaviors.GetWallAvoidanceForce(maxVelocity) * behaviorWeights[6];

        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force
        rigidbody.AddForce(force - rigidbody.velocity);
	}

    /// <summary>
    /// Updates the path in the event that we have made it to another waypoint
    /// </summary>
    private void updatePath() {
        if (path.Count > 0) {
            // Check if we have arrived
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumArrivalRadiusSqrd) {
                // We are close enough, get the next goal
                Vector2 newLocation = path.Dequeue();
                steeringBehaviors.targetPosition = new Vector3(newLocation.x, rigidbody.position.y, newLocation.y);
            }
            // else don't do anything, continue to let it path closer to goal
        } else {
            // Working on the last one
            // If we are there, stop
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumCompleteArrivalRadiusSqrd) {
                steeringBehaviors.targetPosition = rigidbody.position;
            }
        }
    }
}
