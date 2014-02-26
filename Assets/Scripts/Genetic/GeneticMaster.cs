using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main class in genetic algorithm.
/// Will handle figuring out success of victims, and building new generations
/// from the results of the previous level.
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
    private class GeneticVictim {

    }

    List<GeneticVictim> victims;

    void Start() {
        victims = new List<GeneticVictim>();
    }


}
