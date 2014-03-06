using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Class used to get a random name for Victims
/// 
/// Names in Victim_Names.txt sourced from http://deron.meranda.us/data/census-derived-all-first.txt
/// </summary>
public static class VictimNames {

    /// <summary>
    /// An array of names read from the file of names
    /// </summary>
    private static string[] names = File.ReadAllLines(Application.dataPath + "\\Victim_Names.txt");

    /// <summary>
    /// Picks a random name from the list and returns it
    /// </summary>
    /// <returns>The name</returns>
    public static string getRandomName() {
        int rand = Random.Range(0, names.Length);
        return names[rand].Trim(' ', '\n');  // Removes any whitespace or newlines from the front and back then returns it
    }
}
