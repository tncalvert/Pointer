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
    public float velocityMultCollisionAvoidance = 2.0f;

    public Vector3 GetCollisionAvoidanceForce(float maxVelocity) {

        // We'll let flocking behaviors handle collision with other victims

        RaycastHit rayHit = new RaycastHit();
        if (Physics.SphereCast(renderer.bounds.center + (rigidbody.velocity.normalized * boundingRadiusCollisionAvoidance),
            	boundingRadiusCollisionAvoidance,
		        rigidbody.velocity.normalized,
		        out rayHit,
            	rigidbody.velocity.magnitude * velocityMultCollisionAvoidance,
            	~((1 << LayerMask.NameToLayer("Victims")) | (1 << LayerMask.NameToLayer("Ground"))))) {
            Vector3 collisionDirection = (rayHit.point - rayHit.collider.bounds.center).normalized;
			//collisionDirection = rayHit.normal;
            Debug.DrawLine(rayHit.point, rayHit.point + (collisionDirection * maxVelocity), Color.white);
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

    public float feelerFactor = 1.0f;
    public float forceFactor = 3.0f;
    public float boundingRadiusWallAvoidance;

    public Vector3 GetWallAvoidanceForce(float maxVelocity) {
        // find closest wall
        // calculate feelers
        Vector3[] feelers = new Vector3[3];
        float feelerLength = (rigidbody.velocity.magnitude * feelerFactor) + boundingRadiusWallAvoidance;
        feelers[0] = rigidbody.velocity.normalized * feelerLength;
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
}
