using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject bombPowerupPrefab;
    [SerializeField] private GameObject laserPowerupPrefab;
    [SerializeField] private GameObject scorePowerupPrefab;

    public void SpawnPowerup(Transform location, string powerupName)
    {
        if (powerupName == "Bomb")
        {
            Instantiate(bombPowerupPrefab, location.position, transform.rotation);
        }
        else if (powerupName == "Laser")
        {
            Instantiate(laserPowerupPrefab, location.position, transform.rotation);
        }
        else if (powerupName == "Score")
        {
            Instantiate(scorePowerupPrefab, location.position, transform.rotation);
        }
    }

}