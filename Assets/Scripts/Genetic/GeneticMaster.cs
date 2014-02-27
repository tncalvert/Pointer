using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
///     - Survival time         (30%)
///     - Damage done to player (70%)
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
        public VictimMonitor data;

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
        float survivalTime = v.monitor.timeSpentAlive;
        float damageDealt = v.monitor.damageToPlayer;

        // TODO: We'll need to find percentages for time and damage and use that for
        //       success, then weight them.

        return fitness;
    }

    /// <summary>
    /// Performs the genetic algorithm, generating a new population
    /// from the previous iteration.
    /// </summary>
    private void generateNewPopulation() {

    }

    /// <summary>
    /// Generates the initial population on the first time the game loads
    /// </summary>
    public void generateInitialPopulation() {

        // TODO: Define an acceptable initial value for all the values we will be working with

        foreach (Vector2 p in initialPositions) {
            GameObject g = (GameObject)Instantiate(victimPrefab, new Vector3(p.x, 1, p.y), Quaternion.identity);
            VictimMonitor vm = g.GetComponent<VictimMonitor>();

            GeneticVictim gv = new GeneticVictim();
            gv.victimID = victimIds++;
            gv.monitor = vm;  // Need to get the data one correct

            // TODO: Set initial values, then use SendMessage to instruct all files to update if they need to

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
    public void receiveVictimReport(uint vID, VictimMonitor data, VictimMonitor monitor) {
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
