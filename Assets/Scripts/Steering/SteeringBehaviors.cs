using UnityEngine;
using System.Collections;

/// <summary>
/// A series of steering behaviors, accessed as functions
/// The code is based on code demonstrated by Prof. Kesselman
/// This contains code for Fear, Seeking, Collision Avoidance,
/// Wall Avoidance, Wander, Alignment, Cohesion, Separation
/// </summary>
public class SteeringBehaviors : MonoBehaviour {

    #region General

    void Start() {

        boundingRadiusCollisionAvoidance = 
            Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y,
                renderer.bounds.size.z) / 2;

        wanderTarget = new Vector3(0, 0, wanderRadius);
    }

    #endregion

    #region Alignment

    public float neighborRadiusAlignment = 10f;
    public float alignmentTweak = 3.0f;

    public Vector3 GetAlignmentForce(float maxVelocity) {
        Collider[] neighbors = Physics.OverlapSphere(rigidbody.position,
            neighborRadiusAlignment, 1 << LayerMask.NameToLayer("Victims"));
        Vector3 avgHeading = Vector3.zero;
        foreach (Collider n in neighbors) {
            avgHeading += n.rigidbody.velocity;
        }
        avgHeading /= (neighbors.Length != 0 ? neighbors.Length : 1);
        return (avgHeading - rigidbody.velocity) * alignmentTweak;
    }

    #endregion

    #region Cohesion

    public float neighborRadiusCohesion = 10f;
    public float cohesionTweak = 3.0f;

    public Vector3 GetCohesionForce(float maxVelocity) {

        Collider[] neighbors = Physics.OverlapSphere(rigidbody.position,
            neighborRadiusCohesion, 1 << LayerMask.NameToLayer("Victims"));

        Vector3 com = Vector3.zero;
        foreach (Collider n in neighbors) {
            com += n.rigidbody.position;
        }
        com /= (neighbors.Length != 0 ? neighbors.Length : 1);
        Vector3 currentPos = rigidbody.position;
        Vector3 desiredVelocity =
            (com - currentPos).normalized * cohesionTweak;
        Vector3 result = desiredVelocity - rigidbody.velocity;
        return result;
    }

    #endregion



    #region CollisionAvoidance

    public float boundingRadiusCollisionAvoidance;
    public float velocityMultCollisionAvoidance = 2f;

    public Vector3 GetCollisionAvoidanceForce(float maxVelocity) {

        // We'll let flocking behaviors handle collision with other victims

        RaycastHit rayHit = new RaycastHit();
        if (Physics.SphereCast(renderer.bounds.center + (rigidbody.velocity.normalized * boundingRadiusCollisionAvoidance),
            	boundingRadiusCollisionAvoidance,
		        rigidbody.velocity.normalized,
		        out rayHit,
            	rigidbody.velocity.magnitude * velocityMultCollisionAvoidance,
		                       /*~((1 << LayerMask.NameToLayer("Victims")) | (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Sidewalk"))))*/ (1<<LayerMask.NameToLayer("City")))) {
			Vector3 collisionDirection = (rayHit.point - rayHit.collider.bounds.center).normalized;
			//collisionDirection = (rayHit.normal + Vector3.Reflect(rigidbody.velocity, rayHit.normal).normalized).normalized;
			collisionDirection = rayHit.normal;
			float xDucker = Mathf.Abs(rigidbody.velocity.x)/rigidbody.velocity.x;
			float zDucker = Mathf.Abs(rigidbody.velocity.z)/rigidbody.velocity.z;

			if (Mathf.Abs (rayHit.normal.z) > .5f && Mathf.Abs (rayHit.normal.x) < .5f){
				collisionDirection = new Vector3(xDucker, 0, 0).normalized;
			}
			if (Mathf.Abs (rayHit.normal.x) > .5f && Mathf.Abs (rayHit.normal.z) < .5f){
				collisionDirection = new Vector3(0, 0, zDucker).normalized;
			}

			RaycastHit rayHit2 = new RaycastHit();
			if (Physics.Raycast( (.5f*(rayHit.point + renderer.bounds.center)) + (collisionDirection * boundingRadiusCollisionAvoidance),
			                       
			                       collisionDirection,
			                       out rayHit2,
			                       collisionDirection.magnitude * velocityMultCollisionAvoidance*2,
			                        (1<<LayerMask.NameToLayer("City")))) {
				collisionDirection = rayHit.normal;
			}


            Debug.DrawLine(rayHit.point, rayHit.point + (collisionDirection * maxVelocity), Color.magenta);
            return collisionDirection * maxVelocity;
        }

        return new Vector3(0, 0, 0);
    }

    #endregion

    #region Fear

    public string fearedTag = "feared";
    public float fearRadius = 10.0f;

    public Vector3 GetFearForce(float maxVelocity) {
        float fearDistanceSquared = fearRadius * fearRadius;

        GameObject fearedObject = GameObject.FindWithTag(fearedTag);

        if (!fearedObject) {
            return new Vector3(0, 0, 0);
        }

        Vector3 desiredVelocity;

        if ((fearedObject.rigidbody.position - rigidbody.position).sqrMagnitude > fearDistanceSquared) {
            // Too far away
            desiredVelocity = rigidbody.velocity;
        } else {
            desiredVelocity = ((rigidbody.position -
                (fearedObject.rigidbody.position + fearedObject.rigidbody.velocity))
                .normalized * maxVelocity);
        }

        return (desiredVelocity - rigidbody.velocity);
    }

    #endregion

    #region Seek

    public Vector3 targetPosition = Vector3.zero;

    public Vector3 GetSeekForce(float maxVelocity) {
        Vector3 desiredVelocity =
            (targetPosition - rigidbody.position).normalized * maxVelocity;
        return desiredVelocity - rigidbody.velocity;
    }

    #endregion

    #region Separation

    public float neighborRadiusSeparation = 10f;
    public float separationTweak = 5.0f;

    public Vector3 GetSeparationForce(float maxVelocity) {
        Collider[] neighbors = Physics.OverlapSphere(rigidbody.position,
            neighborRadiusSeparation, 1 << LayerMask.NameToLayer("Victims"));
        Vector3 result = Vector3.zero;
        foreach (Collider n in neighbors) {
            Vector3 vectorToUs = rigidbody.position - n.rigidbody.position;
            if (vectorToUs.sqrMagnitude != 0) {
                result += vectorToUs.normalized / vectorToUs.magnitude;
            }
        }
        return result * separationTweak;
    }

    #endregion

    #region WallAvoidance

    public float feelerFactor = 0.5f;
    public float forceFactor = 3.0f;
    public float boundingRadiusWallAvoidance;

    public Vector3 GetWallAvoidanceForce(float maxVelocity) {
        // find closest wall
        // calculate feelers
        Vector3[] feelers = new Vector3[3];
        float feelerLength = (rigidbody.velocity.magnitude * feelerFactor) + boundingRadiusWallAvoidance;
        feelers[0] = rigidbody.velocity.normalized * feelerLength * 5;
        feelers[1] = (feelers[0] + new Vector3(-feelers[0].z, feelers[0].y, feelers[0].x))
            .normalized * feelerLength;
        feelers[2] = (feelers[0] + new Vector3(feelers[0].z, feelers[0].y, -feelers[0].x))
            .normalized * feelerLength;

        Debug.DrawLine(rigidbody.position, rigidbody.position + feelers[0], Color.green);
        Debug.DrawLine(rigidbody.position, rigidbody.position + feelers[1], Color.green);
        Debug.DrawLine(rigidbody.position, rigidbody.position + feelers[2], Color.green);

        // check for impacts
        Vector3 result = new Vector3(0, 0, 0);
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < 3; ++i) {
            if (Physics.Linecast(rigidbody.position, rigidbody.position + feelers[i],
                out hit, 1 << LayerMask.NameToLayer("City"))) {
                result += hit.normal * ((feelers[i].magnitude - hit.distance) * forceFactor);
            }
        }

        Debug.DrawLine(rigidbody.position, rigidbody.position + result, Color.yellow);

        return result;
    }

    #endregion

    #region Wander

    public float wanderRadius = 2f;
    public float wanderDistance = 4f;
    public float wanderJitter = 25f;
    public Vector3 wanderTarget;

    public Vector3 GetWanderForce(float maxVelocity) {
        Vector3 targetJitter = new Vector3(NormalizedRandom() * wanderJitter,
            0, NormalizedRandom() * wanderJitter);
        wanderTarget += targetJitter;
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        Vector3 localTarget = wanderTarget + rigidbody.velocity.normalized * wanderDistance;

        return localTarget;
    }

    private float NormalizedRandom() {
        return Random.Range(-1f, 1f);
    }

    #endregion

	#region SidewalkLove
	public Vector3 GetSideWalkLoveForce(float maxVelocity){
		
		//cast out ray to the right and left of movement
		Vector3[] feelers = new Vector3[3];
		float feelerLength = .3f * (rigidbody.velocity.magnitude * 1) + boundingRadiusWallAvoidance;
		feelers[0] = rigidbody.velocity.normalized * feelerLength;
		feelers[1] = (feelers[0] + new Vector3(-feelers[0].z, feelers[0].y, feelers[0].x))
			.normalized * feelerLength;
		feelers[2] = (feelers[0] + new Vector3(feelers[0].z, feelers[0].y, -feelers[0].x))
			.normalized * feelerLength;
		
		//if either intersects a sidewalk, pick shorter, and apply forces towards it, inverse porportional to distance
		RaycastHit rayHitLeft = new RaycastHit ();
		RaycastHit rayHitRight = new RaycastHit ();
		bool hitLeft = Physics.SphereCast (renderer.bounds.center,
		                                   boundingRadiusCollisionAvoidance,
		                                   feelers[1].normalized,
		                                   out rayHitLeft,
		                                   feelers[1].magnitude,
		                                   (1 << LayerMask.NameToLayer ("Sidewalk")));
		bool hitRight = Physics.SphereCast (renderer.bounds.center,
		                                    boundingRadiusCollisionAvoidance,
		                                    feelers[2].normalized,
		                                    out rayHitRight,
		                                    feelers[2].magnitude,
		                                    (1 << LayerMask.NameToLayer ("Sidewalk")));
		
		if (rayHitRight.distance < .3f)
			hitRight = false;
		if (rayHitLeft.distance < .3f)
			hitLeft = false;
		
		RaycastHit rayHit = new RaycastHit ();
		if (hitLeft && hitRight) {
			if (rayHitLeft.distance < rayHitRight.distance) {
				rayHit = rayHitLeft;
			} else {
				rayHit = rayHitRight;
			}
		} else {
			if (hitLeft){
				rayHit = rayHitLeft;
			} else if (hitRight){
				rayHit = rayHitRight;
			}
		}
		
		if (hitLeft || hitRight){
			Vector3 sidewalkLove = (rayHit.collider.bounds.center - renderer.bounds.center).normalized;
			
			Debug.DrawLine(rayHit.point, renderer.bounds.center, Color.blue);
			return sidewalkLove * maxVelocity * (2*rayHit.distance/feelers[0].magnitude);
		}
		
		return new Vector3 (0, 0, 0);
	}
	#endregion

	#region PushedByWallForce
	public Vector3 PushedByWallsForce(float maxVelocity){


	
		RaycastHit rayHit = new RaycastHit();
		if (Physics.SphereCast (renderer.bounds.center,
		                       boundingRadiusCollisionAvoidance*3,
		                       new Vector3(0,1,0),
		                       out rayHit,
		                       2,
		                        (1 << LayerMask.NameToLayer ("City")))) {
						Vector3 collisionDirection = rayHit.normal.normalized;
			Debug.DrawLine(renderer.bounds.center, renderer.bounds.center + (collisionDirection * maxVelocity), Color.red);
			return collisionDirection * 2.5f * maxVelocity * Mathf.Max (9, rayHit.distance / 9)/9;
				}
		return Vector3.zero;
	}
	#endregion

}
