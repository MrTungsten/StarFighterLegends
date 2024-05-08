using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{

    private GameObject player;
    private static int[] playerStats = null;
    private int[] defaultStats = new int[] { 15, 3, 2 };

    public static PlayerStatsManager Instance { get; private set; }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        CreateSingleton();

        if (playerStats == null)
        {
            ResetStats();
        }
    }

    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    /*
    public void AddStats(int healthToAdd, int bombCountToAdd, int laserCountToAdd)
    {

        int[] initialPlayerStats = new int[] { playerStats[0], playerStats[1], playerStats[2] };

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = new int[] { playerStats[0] + healthToAdd, playerStats[1] + bombCountToAdd, playerStats[2] + laserCountToAdd };

        if (playerStats[0] > 12)
        {
            playerStats[0] = initialPlayerStats[0];
        }

        if (playerStats[1] > 7)
        {
            playerStats[1] = initialPlayerStats[1];
        }

        if (playerStats[2] > 5)
        {
            playerStats[2] = initialPlayerStats[2];
        }

        SetStats();
        Debug.Log("Added Stats!");
    }
    */

    public int[] GetStats()
    {
        if (playerStats != null)
            return playerStats;
        else
            return defaultStats;
    }

    public void UpdatePlayerStats()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerScript>().GetStats();
    }

    public void ChangePlayerStats(int hitpoints, int bombs, int lasers)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerScript>().SetStats(hitpoints, bombs, lasers);
    }

    public void SetStats()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerScript>().SetStats(playerStats[0], playerStats[1], playerStats[2]);
    }

    public void ResetStats()
    {
        playerStats = defaultStats;
    }

}