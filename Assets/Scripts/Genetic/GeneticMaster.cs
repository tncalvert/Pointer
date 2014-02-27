using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Main class in genetic algorithm.
/// Will handle figuring out success of victims, and building new generations
/// from the results of the previous level.
/// 
/// This class holds all the necessary data as static variables, so they
/// will persist through level loads (as we are using the same scene for
/// every level, using DontDestroyOnLoad resulted in a new copy of the
/// object being spawned on every load). Then, when starting, the
/// script will call the appropriate function for whichever load this is,
/// either generateInitalPopulation if this is the first load or
/// the series of functions to perform the genetic algorithm if it is a
/// subsequent load.
/// 
/// --NOTES--
/// This will work slightly differently from a standard genetic algorithm
/// We will generate a random population, which will serve as the victims
/// of the first level. After the level is over, victims are evaulated on
/// various criteria:
///     - Survival time         (40%)
///     - Damage done to player (60%)
/// After the level is completed. We will take a subset of the population
/// (50%??) and use them to create a new generation of vicitims.
/// However, we do preserve the other section of the list that has been
/// determined as failures. After running the next level, we will then
/// take the next section from the combination of the victims played
/// in the level, and the older, failed, victims to make sure we aren't
/// introducing failing victims into the game.
/// </summary>
public class GeneticMaster : MonoBehaviour {

    /// <summary>
    /// Class to hold a victim and its corresponding values, success, etc.
    /// </summary>
    public class GeneticVictim {

        /// <summary>
        /// A unique ID for the victim
        /// </summary>
        public uint victimID;

        /// <summary>
        /// Holds the victim's data, such as steering weights, fuzzly logic weights, etc.
        /// </summary>
        public VictimData data;

        /// <summary>
        /// Monitor class that holds the success of the object in the game world
        /// </summary>
        public VictimMonitor monitor;

        /// <summary>
        /// The fitness score of the victim
        /// </summary>
        public float fitness;
    }

    /// <summary>
    /// Prefab for the victims
    /// </summary>
    public GameObject victimPrefab;

    /// <summary>
    /// A list of all victims in the level
    /// </summary>
    public static List<GeneticVictim> victims;

    /// <summary>
    /// A list of old victims that were discarded after the last run
    /// </summary>
    public static List<GeneticVictim> discardedVictims;

    /// <summary>
    /// A list of positions to place the victims
    /// </summary>
    public List<Vector2> initialPositions;
    
    /// <summary>
    /// A incremental count for available victim IDs, starting at 0
    /// </summary>
    private static uint victimIds;

    /// <summary>
    /// The total time it took for the player to complete the level.
    /// Found by taking the max of the times in the victim list.
    /// </summary>
    private float totalLevelTime;

    /// <summary>
    /// The total damage the player took.
    /// Found by summing all the values from the victim list.
    /// </summary>
    private float totalDamageToPlayer;

    void Start() {

        if (victims == null) {
            // This will only happen on the first time the level is loaded
            victims = new List<GeneticVictim>();
            victimIds = 0;
            generateInitialPopulation();
        } else {
            // Reload
            calulateFitnessOfVictims();
            generateNewPopulation();
        }
    }

    /// <summary>
    /// Calculates the fitness of all the victims that were in play during the last level
    /// </summary>
    public void calulateFitnessOfVictims() {

        totalLevelTime = victims.Max(m => m.monitor.timeSpentAlive);
        totalDamageToPlayer = victims.Sum(m => m.monitor.damageToPlayer);

        foreach (GeneticVictim v in victims) {
            v.fitness = getFitness(v);
        }
    }

    /// <summary>
    /// Calculates the fitness of an individual victim.
    /// Fitness is calculated based on the time the victim survived before being killed
    /// in relation to the total time of the round, and the damage dealt to the player,
    /// again in relation to the total damage dealt. Those two values are then weighted
    /// to bring the whole value into the range [0, 100].
    /// </summary>
    /// <param name="v">The victim to calculate the fitness for</param>
    /// <returns>The fitness of the victim</returns>
    private float getFitness(GeneticVictim v) {
        float fitness = 0f;
        float survivalTime = (v.monitor.timeSpentAlive / totalLevelTime) / v.monitor.numberOfTimesStuck;
        float damageDealt = v.monitor.damageToPlayer / totalDamageToPlayer;

        fitness = (survivalTime * 40) + (damageDealt * 60);

        return fitness;
    }

    /// <summary>
    /// Performs the genetic algorithm, generating a new population
    /// from the previous iteration.
    /// </summary>
    private void generateNewPopulation() {

        // Determine the best

    }

    /// <summary>
    /// Generates the initial population on the first time the game loads
    /// </summary>
    public void generateInitialPopulation() {

        // Default ranges for these values. They will be changed after some training.
        float[] attribRange = { 0.3f, 0.7f };   // Range for bravery, toughness and independence
        float[] maxV = { 10f, 30f };        // max velocity
        float[] maxF = { 20f, 60f };        // max force
        float[] minArriv = { 4f, 16f };    // minimum arrival radius sqrd
        float[] minComp = { 2f, 9f };     // minimum complete arrival radius sqrd
        float[] uniqP = { 0.2f, 0.8f };       // unique path probability
        float[] pathCR = { 6f, 14f };      // path check radius
        float[] sAlign = { 0.75f, 1.25f };      // steering alignment
        float[] sCohes = { 0.75f, 1.25f };      // steering cohesion
        float[] sCollAvoid = { 2f, 4f };  // collision avoidance
        float[] sFear = { 3f, 5f };       // fear
        float[] sSeek = { 0.75f, 1.25f };       // seek
        float[] sSepar = { 1f, 1.4f };      // separation
        float[] sWallAvoid = { 2.25f, 3.25f };  // wall avoidance
        float[] sWand = { 0.3f, 0.9f };       // wander
        float[] sSWL = { 0.75f, 1.25f };        // sidewalk love


        foreach (Vector2 p in initialPositions) {
            GameObject g = (GameObject)Instantiate(victimPrefab, new Vector3(p.x, 1, p.y), Quaternion.identity);
            VictimMonitor vm = g.GetComponent<VictimMonitor>();
            VictimControl vc = g.GetComponent<VictimControl>();
            VictimData vd = g.GetComponent<VictimData>();
            VictimSteering vs = g.GetComponent<VictimSteering>();

            // Set the head values
            vd.AttribBravery = Random.Range(attribRange[0], attribRange[1]);
            vd.AttribIndependence = Random.Range(attribRange[0], attribRange[1]);
            vd.AttribToughness = Random.Range(attribRange[0], attribRange[1]);

            vd.MaxVelocity = Random.Range(maxV[0], maxV[1]);
            vd.MaxForce = Random.Range(maxF[0], maxF[1]);
            vd.MinimumArrivalRadiusSqrd = Random.Range(minArriv[0], minArriv[1]);
            vd.MinimumCompleteArrivalRadiusSqrd = Random.Range(minComp[0], minComp[1]);
            vd.UniquePathProbability = Random.Range(uniqP[0], uniqP[1]);
            vd.PathCheckRadius = Random.Range(pathCR[0], pathCR[1]);

            vd.SteeringAlignment = Random.Range(sAlign[0], sAlign[1]);
            vd.SteeringCohesion = Random.Range(sCohes[0], sCohes[1]);
            vd.SteeringCollisionAvoidance = Random.Range(sCollAvoid[0], sCollAvoid[1]);
            vd.SteeringFear = Random.Range(sFear[0], sFear[1]);
            vd.SteeringSeek = Random.Range(sSeek[0], sSeek[1]);
            vd.SteeringSeparation = Random.Range(sSepar[0], sSepar[1]);
            vd.SteeringWallAvoidance = Random.Range(sWallAvoid[0], sWallAvoid[1]);
            vd.SteeringWander = Random.Range(sWand[0], sWand[1]);
            vd.SteeringSideWalkLove = Random.Range(sSWL[0], sSWL[1]);
            

            // Assign values, and generate a new GeneticVictim for this victim
            vc.head = vd;
            vs.head = vd;

            GeneticVictim gv = new GeneticVictim();
            gv.victimID = victimIds++;
            gv.data = vd;
            gv.monitor = vm;

            vc.updateFromHead();

            vm.victimID = gv.victimID;
            vm.geneticMaster = this;

            victims.Add(gv);
        }
    }

    /// <summary>
    /// A function called by the dying victim prior to being destroyed in order to report
    /// its various successes.
    /// </summary>
    /// <param name="vID">The unique ID of the victim</param>
    /// <param name="data">The VictimData class from the victim</param>
    /// <param name="monitor">The VictimMonitor class from the victim</param>
    public void receiveVictimReport(uint vID, VictimData data, VictimMonitor monitor) {
        GeneticVictim v = victims.Find(m => m.victimID == vID);

        if (v == null) {
            // No object with that ID
            Debug.Log("No victim with ID " + vID + " found");
            return;
        }

        v.data = data;
        v.monitor = monitor;
    }

}
