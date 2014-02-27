using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows the object containing this script to be moved by a series of steering forces
/// Requires that the object has a rigidbody on it
/// Class based on demonstrations and code by Prof. Kessleman
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
    public Queue<Vector2> path = new Queue<Vector2>();

    /// <summary>
    /// Holds the current destination of the victim. Used as an access for by other victims.
    /// </summary>
    public Vector2 destination;

    /// <summary>
    /// The game object that the current path was found from
    /// </summary>
    public GameObject masterPathHolder;

    /// <summary>
    /// Distance the object can be from its target destination before it is considered to have arrived
    /// </summary>
    float minimumArrivalRadiusSqrd = 9.0f;

    /// <summary>
    /// Distance the object can be from the final point of its path before the path is considered complete
    /// </summary>
    float minimumCompleteArrivalRadiusSqrd = 4.0f;

    /// <summary>
    /// Flag indicating that the object has a path that it is following
    /// </summary>
    bool hasPath = false;

    /// <summary>
    /// The chance that this object will choose its own path over merging with the hive mind
    /// </summary>
    public float uniquePathProbability = 0.5f;

    /// <summary>
    /// Radius of the circle to use when checking for nearby paths
    /// </summary>
    public float pathCheckRadius = 10f;

    /// <summary>
    /// List of all the street tiles in the game, used as a way to get random destinations
    /// </summary>
    List<Street> streets = null;

    /// <summary>
    /// Count so that every 120 (for now, see end of Update) frames we are checking to see
    /// how far the victim has moved, in an attempt to patch out the stuck units
    /// </summary>
    private int updatesWithNoMovementCount;

    /// <summary>
    /// Holds the distance moved over the course of 120 frames
    /// </summary>
    private Vector3 distanceMoved;

    /// <summary>
    /// The position of the object after the previous frame
    /// </summary>
    private Vector3 previousPosition;

    /// <summary>
    /// A flag that indicates the victim had its path reset because it was stuck
    /// </summary>
    private bool hadPathReset;

    /// <summary>
    /// The monitor for this victim
    /// </summary>
    private VictimMonitor victimMonitor;

    public Vector2[] publicPath;

    private bool DEBUG = true;


	// The VictimData head of the control. It holds the important data, and should be used carefully.
	private VictimData head;
	public VictimData Head {
		get { return this.head; }
		set { 
			this.head = value;
			this.updateFromHead();		
		}
	}
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


	/// <summary>
	/// Updates the values in this instance from the VictimData head.
	/// </summary>
	public void updateFromHead(){
		//TODO implement
	}
	
	/// <summary>
	/// Updates the values in the VictimData head to reflect the values in this instance
	/// </summary>
	public void updateHeadValues(){
		//TODO implement
	}

    void Start() {
        // Check to see if we have a rigid body
        try {
            GetComponent<Rigidbody>();

            steeringBehaviors = gameObject.AddComponent<SteeringBehaviors>();
            steeringBehaviors.targetPosition = rigidbody.position;

            destination = new Vector2(rigidbody.position.x, rigidbody.position.z);

            masterPathHolder = this.gameObject;  // Set path holder to itself

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

            victimMonitor = GetComponent<VictimMonitor>();

            updatesWithNoMovementCount = 0;
            previousPosition = Vector3.zero;
            distanceMoved = Vector3.zero;
            hadPathReset = false;

        } catch {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

    }

    void FixedUpdate() {

        if (!rigidbody) {
            // If there is no rigidbody, abort
            return;
        }

        if (pathFinder && !hasPath) {
            getNewPath();
        }

        if (DEBUG) {
            // DEBUG - Display the path that we are trying to follow
            Vector2[] tempPath = publicPath = path.ToArray();
            Debug.DrawLine(rigidbody.position, steeringBehaviors.targetPosition, Color.white);
            if (path.Count > 0)
                Debug.DrawLine(steeringBehaviors.targetPosition, new Vector3(tempPath[0].x, rigidbody.position.y, tempPath[0].y), Color.white);
            for (int i = 1; i < path.Count; ++i) {
                Debug.DrawLine(new Vector3(tempPath[i - 1].x, rigidbody.position.y, tempPath[i - 1].y),
                    new Vector3(tempPath[i].x, rigidbody.position.y, tempPath[i].y), Color.white);
            }
        }

        updatePath();

        // Force and any steering forces we'll want to be able to inspect later
        Vector3 force = new Vector3(0, 0, 0);

        // Add forces
        force += steeringBehaviors.GetAlignmentForce(maxVelocity) * behaviorWeights[0];
        force += steeringBehaviors.GetCohesionForce(maxVelocity) * behaviorWeights[1];
        force += steeringBehaviors.GetCollisionAvoidanceForce(maxVelocity) * behaviorWeights[2];
        force += steeringBehaviors.GetFearForce(maxForce) * behaviorWeights[3];
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

        // Check for movement
        checkMovement();




    }



    /// <summary>
    /// Checks that the unit has been moving consistently, and is not stuck in a corner (or similar situation)
    /// </summary>
    private void checkMovement() {
        updatesWithNoMovementCount++;
        distanceMoved += (rigidbody.position - previousPosition);
        previousPosition = rigidbody.position;
        if (updatesWithNoMovementCount > 120) { /* arbitrary number */
            // Didn't move far enough, ignoring those who are feared
            if (distanceMoved.sqrMagnitude <= 9 && !isPlayerAround()) {
                Vector2? newDest = null;
                if (path.Count > 0) {
                    newDest = path.ToArray()[path.Count - 1];  // Get last item (destination)
                }
                getNewPath(newDest);
                hadPathReset = true;
                victimMonitor.GotStuck();
            } else {
                hadPathReset = false;  // If we're moving again, reset the flag
            }
            updatesWithNoMovementCount = 0;
            distanceMoved = Vector3.zero;
        }
    }

    /// <summary>
    /// Finds out if the player is nearby, with a slightly larger radius that fear
    /// </summary>
    private bool isPlayerAround() {
        float fearRadius = steeringBehaviors.fearRadius * 1.2f;
        GameObject player = GameObject.FindGameObjectWithTag("feared");
        return ((player.rigidbody.position - rigidbody.position).sqrMagnitude <= (fearRadius * fearRadius));
    }

    /// <summary>
    /// Gets a new path for the victim to follow
    /// </summary>
    public void getNewPath(Vector2? finalPoint=null) {
        // Pick a new path, if we don't have one

        // If we tried to reset the path, but remained stuck, don't attempt to try again, just find a new destination
        if (finalPoint != null && !hadPathReset) {
            // Use this as the destination
            path = new Queue<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), finalPoint.Value));
            destination = path.Dequeue();
            steeringBehaviors.targetPosition = new Vector3(destination.x, rigidbody.position.y, destination.y);
            masterPathHolder = this.gameObject;
            return;
        }

        if (streets != null) {

            float rand = Random.Range(0f, 1f);
            Collider[] nearbyVictims = Physics.OverlapSphere(rigidbody.position, pathCheckRadius, 1 << LayerMask.NameToLayer("Victims"));

            if (rand < uniquePathProbability || nearbyVictims.Length == 0) {
                //Debug.Log("Picking my own path");
                // Pick own path
                Vector2 randomStreet = streets[Random.Range(0, streets.Count - 1)].Position;
                path = new Queue<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), randomStreet));
                //path = new List<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), randomStreet));
                destination = path.Dequeue();
                steeringBehaviors.targetPosition = new Vector3(destination.x, rigidbody.position.y, destination.y);
                masterPathHolder = this.gameObject;
            } else {
                //Debug.Log("Checking for a path near me");
                bool foundPath = false;
                // Take a path from first nearby object with a path
                foreach (var n in nearbyVictims) {
                    VictimSteering v = n.gameObject.GetComponent<VictimSteering>();
                    // Check that the original path holder is actually close enough (otherwise paths can propagate pretty far)
                    if (v.masterPathHolder != null && (v.masterPathHolder.rigidbody.position - rigidbody.position).sqrMagnitude > pathCheckRadius * pathCheckRadius) {
                        // To far, ignore this option
                        continue;
                    }

                    // Check that we can see the person we are getting a path from, if not, don't take the path
                    if (Physics.Raycast(rigidbody.position, (v.gameObject.rigidbody.position - rigidbody.position),
                        (v.gameObject.rigidbody.position - rigidbody.position).magnitude,
                        (1 << LayerMask.NameToLayer("City")) | (1 << LayerMask.NameToLayer("Sidewalk")))) {
                        continue;
                    }

                    if (v.hasPath && v.destination != Vector2.zero && v.path.Count != 0) {
                        path = new Queue<Vector2>(v.path);

                        // If we can't see the destination, put destination into the path, and set the path's object's position
                        // as the first destination
                        Vector3 firstDestination = new Vector3(v.destination.x, rigidbody.position.y, v.destination.y);
                        if (Physics.Raycast(rigidbody.position, (firstDestination - rigidbody.position),
                            (firstDestination - rigidbody.position).magnitude,
                            (1 << LayerMask.NameToLayer("City")) | (1 << LayerMask.NameToLayer("Sidewalk")))) {

                            // Hit, destination is object's position
                            destination = new Vector2(v.gameObject.rigidbody.position.x, v.gameObject.rigidbody.position.z);
                            // Add object's position to the front of the queue
                            List<Vector2> tempPath = new List<Vector2>(path);
                            tempPath.Insert(0, v.destination);
                            path = new Queue<Vector2>(tempPath);
                        } else {
                            // Otherwise, we are good with just using the object's destination
                            destination = v.destination;
                        }
                        steeringBehaviors.targetPosition = new Vector3(destination.x, rigidbody.position.y, destination.y);
                        foundPath = true;
                        masterPathHolder = v.masterPathHolder;
                        //Debug.Log("Found a path to follow");
                        break;
                    }
                }

                if (!foundPath) {
                    // Couldn't find a path, get one of my own
                    //Debug.Log("Couldn't get a path, picking my own");
                    Vector2 randomStreet = streets[Random.Range(0, streets.Count - 1)].Position;
                    path = new Queue<Vector2>(pathFinder.getPath(new Vector2(rigidbody.position.x, rigidbody.position.z), randomStreet));
                    destination = path.Dequeue();
                    steeringBehaviors.targetPosition = new Vector3(destination.x, rigidbody.position.y, destination.y);
                    masterPathHolder = this.gameObject;
                }
            }

            hasPath = true;

        } else {
            Debug.Log("We don't have a list of streets! This is a HUGE problem!");
        }
    }

    /// <summary>
    /// Updates the path in the event that we have made it to another waypoint
    /// </summary>
    private void updatePath() {
        if (path.Count > 0) {
            // Check if we have arrived
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumArrivalRadiusSqrd) {
                // We are close enough, get the next goal
                destination = path.Dequeue();
                steeringBehaviors.targetPosition = new Vector3(destination.x, rigidbody.position.y, destination.y);
                publicPath = path.ToArray();
            }
            // else don't do anything, continue to let it path closer to goal
        } else {
            // Working on the last one
            // If we are there, stop
            if ((steeringBehaviors.targetPosition - rigidbody.position).sqrMagnitude < minimumCompleteArrivalRadiusSqrd) {
                hasPath = false;
                destination = Vector2.zero;
                steeringBehaviors.targetPosition = rigidbody.position;
            }
        }
    }

	/// <summary>
	/// Gets the path finder.
	/// </summary>
	/// <returns>The path finder.</returns>
	public PathFinder getPathFinder(){
		return this.pathFinder;
	}

    /// <summary>
    /// Called from GeneticMaster via SendMessage. Informs the script that the variables in
    /// the VictimHead have been updated and the script should update its variables.
    /// </summary>
    public void updateVariables() {
        // TODO: Implement this
    }
}
