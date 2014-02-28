using UnityEngine;
using System.Collections;

/// <summary>
/// Class that monitors the various factors that contribute
/// to the fitness of a victim.
/// </summary>
public class VictimMonitor : MonoBehaviour {

    /// <summary>
    /// An ID for the victim, making it easier to identify
    /// </summary>
    public uint victimID;

    /// <summary>
    /// The survival time of the victim, measured since the level loaded
    /// </summary>
    public float timeSpentAlive;

    /// <summary>
    /// The amount of damage dealt to the player
    /// </summary>
    public float damageToPlayer;

    public uint numberOfTimesStuck;

    /// <summary>
    /// Time marked at the start of the level
    /// </summary>
    private float startTime;

    /// <summary>
    /// The GeneticMaster script, controlling genetic upgrades for victims
    /// </summary>
    public GeneticMaster geneticMaster;

    void Start() {
        startTime = Time.realtimeSinceStartup;
        damageToPlayer = 0;
        numberOfTimesStuck = 0;
    }

    /// <summary>
    /// Function called when the victim dies. Sets the time the victim survived.
    /// </summary>
    void OnDestroy() {
        timeSpentAlive = Time.realtimeSinceStartup - startTime;
    }

    /// <summary>
    /// Called when this victim deals damage to the player
    /// </summary>
    /// <param name="damageAmount">The amount of damage done</param>
    public void DealtDamage(float damageAmount) {
        damageToPlayer += damageAmount;
    }

    /// <summary>
    /// Called when the victim gets stuck, incrementing a counter that is used to
    /// punish the victim (and deprioritize those weights).
    /// </summary>
    public void GotStuck() {
        numberOfTimesStuck++;
    }
}
