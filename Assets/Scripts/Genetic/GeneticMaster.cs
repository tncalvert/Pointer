using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

        /// <summary>
        /// Override of Equals
        /// </summary>
        /// <param name="obj">Other object</param>
        /// <returns>True if equals (id is equal)</returns>
        public override bool Equals(object obj) {
            GeneticVictim gv = (GeneticVictim)obj;
            return this.victimID == gv.victimID;
        }

        /// <summary>
        /// Returns a string representing the victim
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string temp = "";
            temp += "Victim " + victimID + " - Fitness: " + fitness + "\n";
            temp += "\tBravery: " + data.AttribBravery;
            temp += "\tToughness: " + data.AttribIndependence;
            temp += "\tIndependence: " + data.AttribToughness + "\n";
            temp += "\tMax Force: " + data.MaxForce;
            temp += "\tMax Velocity: " + data.MaxVelocity + "\n";
            temp += "\tArrival: " + data.MinimumArrivalRadiusSqrd;
            temp += "\tComplete Arrival: " + data.MinimumCompleteArrivalRadiusSqrd;
            temp += "\tUnique Path: " + data.UniquePathProbability;
            temp += "\tPath Check: " + data.PathCheckRadius + "\n";
            temp += "\tSteering Weights\n";
            temp += "\t\tAlignment: " + data.SteeringAlignment;
            temp += "\tCohesion: " + data.SteeringCohesion;
            temp += "\tCollision Avoidance: " + data.SteeringCollisionAvoidance + "\n";
            temp += "\t\tFear: " + data.SteeringFear;
            temp += "\tSeek: " + data.SteeringSeek;
            temp += "\tSeparation: " + data.SteeringSeparation + "\n";
            temp += "\tSidewalk: " + data.SteeringSideWalkLove;
            temp += "\t\tWall Avoidance: " + data.SteeringWallAvoidance;
            temp += "\tWander: " + data.SteeringWander + "\n\n";

            return temp;
        }
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

    /// <summary>
    /// Used to indicate we should log the results of the genetic algorithm to a file
    /// </summary>
    private const bool LOGGING = true;

    /// <summary>
    /// Log file
    /// </summary>
    private StreamWriter logFile;

    /// <summary>
    /// Relative path to log file, without '.txt'
    /// </summary>
    private string logFileName;

    /// <summary>
    /// An integer that will be appended to the log file name and incremented every level
    /// </summary>
    private static uint runCount;

    // Default ranges for values. They will be changed after some training.
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

    void Start() {

        if (LOGGING) {
            // Create proper file name
            logFileName = "genetic_log.txt";
        }

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
        Debug.Log(totalDamageToPlayer);

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
        float survivalTime = (v.monitor.timeSpentAlive / (totalLevelTime != 0 ? totalLevelTime : 1)) /
            (v.monitor.numberOfTimesStuck != 0 ? v.monitor.numberOfTimesStuck : 1);
        float damageDealt = v.monitor.damageToPlayer / (totalDamageToPlayer != 0 ? totalDamageToPlayer : 1);

        fitness = (survivalTime * 40) + (damageDealt * 60);

        return fitness;
    }

    /// <summary>
    /// Performs the genetic algorithm, generating a new population
    /// from the previous iteration.
    /// 
    /// Takes the worst 50% from the victims, and moves them into another list.
    /// Then generate a new half of the population, by picking random victims
    /// from the better half (with a weighed preference for better victims)
    /// and picking random pieces of each. Then we'll add some mutation
    /// on a random factor to some of the new victims.
    /// </summary>
    private void generateNewPopulation() {

        // Log results from previous level
        if (LOGGING) {
            using (logFile = new StreamWriter(logFileName, true, System.Text.Encoding.UTF8)) {
                logFile.WriteLine("Results of Level " + (runCount - 1) + " (" + System.DateTime.Now + ")\n====================");
                foreach (GeneticVictim v in victims) {
                    logFile.Write(v);
                }
            }
        }

        if (victims.Count <= 1) {
            // If we can't split the list, just make the new victims and return
            int sIdx = 0;
            foreach (GeneticVictim v in victims) {
                GameObject g = (GameObject)Instantiate(victimPrefab,
                new Vector3(initialPositions[sIdx].x, 1, initialPositions[sIdx].y), Quaternion.identity);
                sIdx++;
                VictimMonitor vm = g.GetComponent<VictimMonitor>();
                VictimControl vc = g.GetComponent<VictimControl>();
                VictimData vd = g.GetComponent<VictimData>();
                VictimSteering vs = g.GetComponent<VictimSteering>();

                // Clone over values we created earlier
                vd.cloneValues(v.data);

                // Assign necessary values
                vc.head = vd;
                vs.head = vd;
                vm.victimID = v.victimID;
                vm.geneticMaster = this;

                v.data = vd;
                v.monitor = vm;

                // Propagate values from head into other components
                vc.updateFromHead();
            }
            return;
        }

        int halfSize = victims.Count / 2;
        int sizeDiff = victims.Count - halfSize;

        // Combine victims and discardedVictims and sort them
        List<GeneticVictim> sortedVictims = victims.Concat(discardedVictims).ToList();
        sortedVictims = sortedVictims.OrderByDescending(m => m.fitness).ToList();

        // Copy the best of the bad victims to discardedVictims, ignoring the rest
        discardedVictims = sortedVictims.Skip(halfSize).Take(sizeDiff).ToList();
        // Remove the bottom half from victims
        victims = sortedVictims.Take(halfSize).ToList();

        // Now, [discardedVictims] contain those we consider too bad to continue, and [victims] holds
        // only those considered good enough with enough space (kind of) to hold [sizeDiff] new
        // victims created by mixing up the victims still in [victims].

        // Create a weighting for picking random values
        // We are taking 1 + 2 + ... + [victims.Count] and then each item
        // is weighted by the opposite of its position in the list (i.e., the last
        // item is 1/[accum], and the first would be [victims.Count]/[accum]
        // Then, we generate a random number in [0, 1] and we use the index of
        // the value that this random number falls closest to (without going over).
        List<float> weights = new List<float>();
        float accum = (victims.Count * (victims.Count + 1)) / 2f;
        for (int i = victims.Count; i > 0; --i) {
            weights.Add(((float)i) / accum);
        }

        int streetIdx = 0;

        // Here we'll build all the new victims
        for (int i = 0; i < sizeDiff; ++i) {
            // Pick two random victims to breed, make sure we get different parents
            GeneticVictim vic1 = victims[weights.randomIndex()];
            GeneticVictim vic2;
            do {
                vic2 = victims[weights.randomIndex()];
            } while (vic2 == vic1);

            GameObject g = (GameObject)Instantiate(victimPrefab,
                new Vector3(initialPositions[streetIdx].x, 1, initialPositions[streetIdx].y), Quaternion.identity);
            streetIdx++;
            
            // Create a VictimData for the new victim and then randomly pick a value from one of the old victims
            GeneticVictim v = new GeneticVictim();
            VictimMonitor vm = g.GetComponent<VictimMonitor>();
            VictimControl vc = g.GetComponent<VictimControl>();
            VictimData vd = g.GetComponent<VictimData>();
            VictimSteering vs = g.GetComponent<VictimSteering>();

            vd.AttribBravery = (Random.Range(0, 2) == 0 ? vic1.data.AttribBravery : vic2.data.AttribBravery);
            vd.AttribIndependence = (Random.Range(0, 2) == 0 ? vic1.data.AttribIndependence : vic2.data.AttribIndependence);
            vd.AttribToughness = (Random.Range(0, 2) == 0 ? vic1.data.AttribToughness : vic2.data.AttribToughness);

            vd.MaxForce = (Random.Range(0, 2) == 0 ? vic1.data.MaxForce : vic2.data.MaxForce);
            vd.MaxVelocity = (Random.Range(0, 2) == 0 ? vic1.data.MaxVelocity : vic2.data.MaxVelocity);
            vd.MinimumArrivalRadiusSqrd = (Random.Range(0, 2) == 0 ? vic1.data.MinimumArrivalRadiusSqrd : vic2.data.MinimumArrivalRadiusSqrd);
            vd.MinimumCompleteArrivalRadiusSqrd = (Random.Range(0, 2) == 0 ? vic1.data.MinimumCompleteArrivalRadiusSqrd : vic2.data.MinimumCompleteArrivalRadiusSqrd);
            vd.UniquePathProbability = (Random.Range(0, 2) == 0 ? vic1.data.UniquePathProbability : vic2.data.UniquePathProbability);
            vd.PathCheckRadius = (Random.Range(0, 2) == 0 ? vic1.data.PathCheckRadius : vic2.data.PathCheckRadius);

            vd.SteeringAlignment = (Random.Range(0, 2) == 0 ? vic1.data.SteeringAlignment : vic2.data.SteeringAlignment);
            vd.SteeringCohesion = (Random.Range(0, 2) == 0 ? vic1.data.SteeringCohesion : vic2.data.SteeringCohesion);
            vd.SteeringCollisionAvoidance = (Random.Range(0, 2) == 0 ? vic1.data.SteeringCollisionAvoidance : vic2.data.SteeringCollisionAvoidance);
            vd.SteeringFear = (Random.Range(0, 2) == 0 ? vic1.data.SteeringFear : vic2.data.SteeringFear);
            vd.SteeringSeek = (Random.Range(0, 2) == 0 ? vic1.data.SteeringSeek : vic2.data.SteeringSeek);
            vd.SteeringSeparation = (Random.Range(0, 2) == 0 ? vic1.data.SteeringSeparation : vic2.data.SteeringSeparation);
            vd.SteeringSideWalkLove = (Random.Range(0, 2) == 0 ? vic1.data.SteeringSideWalkLove : vic2.data.SteeringSideWalkLove);
            vd.SteeringWallAvoidance = (Random.Range(0, 2) == 0 ? vic1.data.SteeringWallAvoidance : vic2.data.SteeringWallAvoidance);
            vd.SteeringWander = (Random.Range(0, 2) == 0 ? vic1.data.SteeringWander : vic2.data.SteeringWander);

            // With a 50% probability, mutate some data
            if (Random.Range(0f, 1f) <= 0.5f) {
                mutateData(vd);
            }

            // Set up links and data for the victim
            v.victimID = victimIds++;
            v.data = vd;
            v.monitor = vm;

            // Assign necessary values
            vc.head = vd;
            vs.head = vd;
            vm.victimID = v.victimID;
            vm.geneticMaster = this;

            // Propagate values from head into other components
            vc.updateFromHead();

            victims.Add(v);
        }

        // Now we have a list of new GeneticVictims, but no actual victims yet, so we have to instantiate them
        // Now place the old victims in their spots
        for (int i = 0; i < halfSize; ++i) {
            GeneticVictim v = victims[i];

            GameObject g = (GameObject)Instantiate(victimPrefab,
                new Vector3(initialPositions[streetIdx].x, 1, initialPositions[streetIdx].y), Quaternion.identity);
            streetIdx++;
            VictimMonitor vm = g.GetComponent<VictimMonitor>();
            VictimControl vc = g.GetComponent<VictimControl>();
            VictimData vd = g.GetComponent<VictimData>();
            VictimSteering vs = g.GetComponent<VictimSteering>();

            // Clone over values we created earlier
            vd.cloneValues(v.data);

            // Assign necessary values
            vc.head = vd;
            vs.head = vd;
            vm.victimID = v.victimID;
            vm.geneticMaster = this;

            v.data = vd;
            v.monitor = vm;

            // Propagate values from head into other components
            vc.updateFromHead();
        }

        if (LOGGING) {
            using (logFile = new StreamWriter(logFileName, true, System.Text.Encoding.UTF8)) {
                logFile.WriteLine("Level " + (runCount++) + " (" + System.DateTime.Now + ")\n====================");
                foreach (GeneticVictim v in victims) {
                    logFile.Write(v);
                }
            }
        }
    }

    /// <summary>
    /// Mutates some of the data in a victim randomly.
    /// </summary>
    /// <param name="vd">The VictimData to mutate. It's a reference so there's no need to return anything</param>
    private void mutateData(VictimData vd) {

        // Pick the number of mutations to do
        int numOfMutations = Random.Range(1, 4); // 1, 2 or 3 values can be mutated

        // Copy all the values into an array so we can pick random indexes
        float[] valArray = 
        {
            vd.AttribBravery, vd.AttribIndependence, vd.AttribToughness,
            vd.MaxForce, vd.MaxVelocity, vd.MinimumArrivalRadiusSqrd,
            vd.MinimumCompleteArrivalRadiusSqrd, vd.PathCheckRadius,
            vd.UniquePathProbability, vd.SteeringAlignment, vd.SteeringCohesion,
            vd.SteeringCollisionAvoidance, vd.SteeringFear, vd.SteeringSeek,
            vd.SteeringSeparation, vd.SteeringSideWalkLove,
            vd.SteeringWallAvoidance, vd.SteeringWander
        };

        // Loop through and mutate items
        for (int i = 0; i < numOfMutations; ++i) {
            int mutIdx = Random.Range(0, valArray.Length);  // Last value is exlusive, so we won't ever get valArray.length

            float mutAmount;

            // Get a mutation value
            // Since these are floats, we can't use xor without a lot of hacky things that might cause more problems
            // than they solve, so we'll pick a random value to either add or subtract based on which value
            // we're mutating
            if (mutIdx <= 2) { // Attributes
                mutAmount = Random.Range(-0.4f, 0.4f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;  // Don't want to actually hit 0
                } else if (valArray[mutIdx] - mutAmount >= 1f) {
                    mutAmount = -(1f - valArray[mutIdx] - 0.00001f);
                }
            } else if (mutIdx > 2 && mutIdx <= 4) {  // Max velocity/force
                mutAmount = Random.Range(-20f, 20f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;
                } else if (valArray[mutIdx] - mutAmount >= 100f) {
                    mutAmount = -(100f - valArray[mutIdx] - 0.00001f);
                }
            } else if (mutIdx > 4 && mutIdx <= 6) {  // Min arrival *
                mutAmount = Random.Range(-4f, 4f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;
                } else if (valArray[mutIdx] - mutAmount >= 25f) {
                    mutAmount = -(25f - valArray[mutIdx] - 0.00001f);
                }
            } else if (mutIdx == 8) {  // Unique path prob
                mutAmount = Random.Range(-0.6f, 0.6f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;
                } else if (valArray[mutIdx] - mutAmount >= 1f) {
                    mutAmount = -(1f - valArray[mutIdx] - 0.00001f);
                }
            } else if (mutIdx == 7) {  // Path radius
                mutAmount = Random.Range(-8f, 8f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;
                } else if (valArray[mutIdx] - mutAmount >= 20f) {
                    mutAmount = -(20f - valArray[mutIdx] - 0.00001f);
                }
            } else {  // Steering
                mutAmount = Random.Range(-2.5f, 2.5f);
                if (valArray[mutIdx] - mutAmount <= 0f) {
                    mutAmount = valArray[mutIdx] + 0.00001f;
                } else if (valArray[mutIdx] - mutAmount >= 8f) {
                    mutAmount = -(8f - valArray[mutIdx] - 0.00001f);
                }
            }

            valArray[mutIdx] -= mutAmount;
            
        }

        // Restore the values
        vd.AttribBravery = valArray[0];
        vd.AttribIndependence = valArray[1];
        vd.AttribToughness = valArray[2];

        vd.MaxForce = valArray[3];
        vd.MaxVelocity = valArray[4];
        vd.MinimumArrivalRadiusSqrd = valArray[5];
        vd.MinimumCompleteArrivalRadiusSqrd = valArray[6];
        vd.PathCheckRadius = valArray[7];
        vd.UniquePathProbability = valArray[8];

        vd.SteeringAlignment = valArray[9];
        vd.SteeringCohesion = valArray[10];
        vd.SteeringCollisionAvoidance = valArray[11];
        vd.SteeringFear = valArray[12];
        vd.SteeringSeek = valArray[13];
        vd.SteeringSeparation = valArray[14];
        vd.SteeringSideWalkLove = valArray[15];
        vd.SteeringWallAvoidance = valArray[16];
        vd.SteeringWander = valArray[17];

    }

    /// <summary>
    /// Generates the initial population on the first time the game loads
    /// </summary>
    public void generateInitialPopulation() {

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

        discardedVictims = new List<GeneticVictim>();

        // If we are logging, print a record of all the victims
        if (LOGGING) {
            using (logFile = new StreamWriter(logFileName, false, System.Text.Encoding.UTF8)) {
                logFile.WriteLine("Level " + (runCount++) + " (" + System.DateTime.Now + ")\n====================");
                foreach (GeneticVictim v in victims) {
                    logFile.Write(v);
                }
            }
        }
    }

}

/// <summary>
/// Class holding an extension of the List object that I've created.
/// Assistance in the algorithm from http://stackoverflow.com/questions/1761626/weighted-random-numbers
/// </summary>
public static class ListExtension {

    /// <summary>
    /// Extension method for IEnumerables used in picking a random index of a list
    /// The list passed in is filled with weights of any manner but *must* sum to 1.
    /// Then, we pick a random number between 0 and 1 (inclusive) and return the
    /// index of the number that is closest to this random number without going over.
    /// </summary>
    /// <param name="weights">List of weights</param>
    /// <returns>A random index</returns>
    public static int randomIndex(this List<float> weights) {
        // Pick a random number

        float rand = Random.Range(0f, 1f);

        int idx = 0;

        // Iterate the enumerable until the element at [idx] is
        // strictly smaller than rand
        for (; idx < weights.Count; ++idx) {
            if (rand < weights[idx]) {
                break;
            }
            rand -= weights[idx];
        }

        // Return this index
        return idx;
    }

}
