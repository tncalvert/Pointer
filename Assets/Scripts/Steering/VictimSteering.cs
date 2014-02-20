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
    public float maxForce = 20.0f;

    /// <summary>
    /// The maximum velocity that results from the application of a steering force
    /// </summary>
    public float maxVelocity = 40.0f;

    /// <summary>
    /// Object containing methods for steering
    /// </summary>
    SteeringBehaviors steeringBehaviors;

    /// <summary>
    /// Pathfinder to get A* path to new location
    /// </summary>
    PathFinder pathFinder = null;

    /// <summary>
    /// Holds path to follow. Will be available to nearby victims so that they can latch on
    /// </summary>
    public List<Vector2> path = new List<Vector2>();

    /// <summary>
    /// Distance the object can be from its target destination before it is considered to have arrived
    /// </summary>
    float minimumArrivalRadiusSqrd = 25.0f;

    /// <summary>
    /// Distance the object can be from the final point of its path before the path is considered complete
    /// </summary>
    float minimumCompleteArrivalRadiusSqrd = 9.0f;

    /// <summary>
    /// Flag indicating that the object has a path that it is following
    /// </summary>
    bool hasPath = false;

    /// <summary>
    /// The chance that this object will choose its own path over merging with the hive mind
    /// </summary>
    public float uniquePathProbability = 0.0f;

    /// <summary>
    /// Radius of the circle to use when checking for nearby paths
    /// </summary>
    public float pathCheckRadius = 10f;

    List<Street> streets = null;

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
    public float[] behaviorWeights = new float[9] { 1.0f,   // Alignment
                                                    1.0f,   // Cohesion
                                                    3f,   // Collision Avoidance
                                                    4.0f,   // Fear
                                                    1.0f,   // Seek
                                                    1.2f,   // Separation
                                                    2.75f,   // Wall Avoidance
                                                    0.6f,    // Wander
													1.0f 	//Sidewalk stickyness
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

            GameObject master = GameObject.Find("MasterGameObject");
            if (master) {
                try {
                    MasterGame masterScript = master.GetComponent<MasterGame>();
                    streets = masterScript.getStreets();
                } catch {
                    Debug.Log("Master Game Controller does not seem to exist");
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

        // DEBUG - Display the path that we are trying to follow
        for (int i = 0; i < path.Count; ++i) {
            if (i == 0) {
                Debug.DrawLine(rigidbody.position, new Vector3(path[i].x, rigidbody.position.y, path[i].y), Color.white);
            } else {
                Debug.DrawLine(new Vector3(path[i - 1].x, rigidbody.position.y, path[i - 1].y), new Vector3(path[i].x, rigidbody.position.y, path[i].y), Color.white);
            }
        }

        if (pathFinder && !hasPath) {
            // Pick a new path, if we don't have one

            if (streets != null) {

                float rand = Random.Range(0f, 1f);
                Collider[] nearbyVictims = Physics.OverlapSphere(rigidbody.position, pathCheckRadius, 1 << LayerMask.NameToLayer("Victims"));

                if (rand < uniquePathProbability || nearbyVictims.Length == 0) {
                    //Debug.Log("Picking my own path");
                    // Pick own path
                    Vector2 randomStreet = streets[Random.Range(0, streets.Count - 1)].Position;
                    path = new List<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), randomStreet));
                    steeringBehaviors.targetPosition = new Vector3(path[0].x, rigidbody.position.y, path[0].y);
                } else {
                    //Debug.Log("Checking for a path near me");
                    bool foundPath = false;
                    // Take a path from first nearby object with a path
                    foreach (var n in nearbyVictims) {
                        VictimSteering v = n.gameObject.GetComponent<VictimSteering>();
                        if (v.hasPath && v.path.Count != 0) {
                            path = new List<Vector2>(v.path);
                            steeringBehaviors.targetPosition = new Vector3(path[0].x, rigidbody.position.y, path[0].y);
                            foundPath = true;
                            //Debug.Log("Found a path to follow");
                            break;
                        }
                    }

                    if (!foundPath) {
                        // Couldn't find a path, get one of my own
                        //Debug.Log("Couldn't get a path, picking my own");
                        Vector2 randomStreet = streets[Random.Range(0, streets.Count - 1)].Position;
                        path = new List<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), randomStreet));
                        steeringBehaviors.targetPosition = new Vector3(path[0].x, rigidbody.position.y, path[0].y);
                    }
                }

                hasPath = true;

            } else {
                Debug.Log("We don't have a list of streets! This is a HUGE problem!");
            }
        }

        if (path.Count > 0) {
            // Check if we have arrived'
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumArrivalRadiusSqrd) {
				Debug.Log ("At a distance of " + (steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude + 
				           ". Progressing to the next point");
                // We are close enough, get the next goal
                path.RemoveAt(0);
                steeringBehaviors.targetPosition = new Vector3(path[0].x, rigidbody.position.y, path[0].y);
            }
            // else don't do anything, continue to let it path closer to goal
        } else {
            // Working on the last one
            // If we are there, stop
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumCompleteArrivalRadiusSqrd) {
				Debug.Log ("We have arrived, stopping movement");
                hasPath = false;
                steeringBehaviors.targetPosition = rigidbody.position;
            }
        }

        Vector3 force = new Vector3(0, 0, 0);

        // Add forces
        force += steeringBehaviors.GetAlignmentForce(maxVelocity) * behaviorWeights[0];
        force += steeringBehaviors.GetCohesionForce(maxVelocity) * behaviorWeights[1];
        force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity) * behaviorWeights[2];
        force += steeringBehaviors.GetFearForce(maxVelocity) * behaviorWeights[3];
        force += steeringBehaviors.GetSeekForce(maxVelocity) * (hasPath ? behaviorWeights[4] : 0f);  // ignore seek force if have finished path
        force += steeringBehaviors.GetSeparationForce(maxVelocity) * behaviorWeights[5];
        force += steeringBehaviors.GetWallAvoidanceForce(maxVelocity) * behaviorWeights[6];
        force += steeringBehaviors.GetWanderForce(maxVelocity) * behaviorWeights[7];

		//force += steeringBehaviors.GetSideWalkLoveForce (maxVelocity) * behaviorWeights[8];
		//force += steeringBehaviors.PushedByWallsForce (maxVelocity);


        // Limit the force
        force = Vector3.ClampMagnitude(force, maxForce);

        // Apply the force
        rigidbody.AddForce(force - rigidbody.velocity);

    }
}
