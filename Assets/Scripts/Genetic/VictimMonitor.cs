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
    }

    /// <summary>
    /// Function called when the victim dies. Sets the time the victim survived.
    /// </summary>
    void OnDestroy() {
        timeSpentAlive = Time.realtimeSinceStartup - startTime;

        geneticMaster.receiveVictimReport(victimID, this, this);  // TODO: fix scripts
    }

    /// <summary>
    /// Called when this victim deals damage to the player
    /// </summary>
    /// <param name="damageAmount">The amount of damage done</param>
    void DealtDamage(float damageAmount) {
        damageToPlayer += damageAmount;
    }
}
