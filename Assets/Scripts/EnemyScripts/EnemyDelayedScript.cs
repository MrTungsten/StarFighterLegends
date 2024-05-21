using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDelayedScript : MonoBehaviour
{
    [SerializeField] private GameObject bullets;
    [SerializeField] private int spawnerAmount;
    [SerializeField] private GameObject enemyDelayedPlaneVisual;
    [SerializeField] private GameObject deathExplosion;
    private GameObject[] spawners;
    private GameObject emptyGameObject;
    private PowerupSpawnerScript powerupSpawnerScript;
    private GameManagerScript gameManagerScript;
    private float hitpoints = 50f;
    private float rotateSpeed = 50f;
    private float rotateAmount = 0f;
    private float rotationMultiplier = 20f;
    private bool hasSpawnedPowerup = false;
    private float firingShotDelay = 0.025f;
    private float shotTimer = 0f;
    private float shotCooldown = 5f;
    private float spawnerRadius = 1f;

    private void Start()
    {
        spawners = new GameObject[spawnerAmount];
        emptyGameObject = new GameObject("EnemyDelayedTurretSpawner");
        Destroy(emptyGameObject);

        for (int i = 0; i < spawnerAmount; i++)
        {
            spawners[i] = Instantiate(emptyGameObject, transform.position, transform.rotation);
            spawners[i].transform.parent = this.transform;
            spawners[i].transform.position = transform.position + new Vector3(Mathf.Cos((360f / spawners.Length) * (i + 1) * Mathf.Deg2Rad), Mathf.Sin((360f / spawners.Length) * (i + 1) * Mathf.Deg2Rad), 0) * spawnerRadius;
            spawners[i].transform.rotation = Quaternion.Euler(0, 0, (360f / spawners.Length) * (i + 1));
        }
        
        shotTimer = shotCooldown;

        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();
        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();
    }

    private void Update()
    {
        rotateAmount += rotationMultiplier * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 10 * rotateAmount), rotateSpeed * Time.deltaTime);

        if (gameManagerScript.IsGameActive())
        {
            if (shotTimer > shotCooldown)
            {
                StartCoroutine(EnemyDelayedPlaneShot());
                shotTimer = 0f;
            }
            else
            {
                shotTimer += Time.deltaTime;
            }
        }
    }

    private IEnumerator EnemyDelayedPlaneShot()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            Instantiate(bullets, spawners[i].transform.position, spawners[i].transform.rotation);
            yield return new WaitForSeconds(firingShotDelay);
        }
    }

    private IEnumerator HitEffect()
    {
        enemyDelayedPlaneVisual.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.1f);

        enemyDelayedPlaneVisual.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);
    }

    public void HitByObject(float damageDone)
    {
        hitpoints -= damageDone;

        StartCoroutine(HitEffect());

        if (hitpoints <= 0 && !hasSpawnedPowerup)
        {
            hasSpawnedPowerup = true;

            int randomNum = Random.Range(1, 101);

            if (randomNum <= 5)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Laser");
            }
            else if (randomNum <= 15)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Bomb");
            }
            else
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Score");
            }

            ScoreManagerScript.Instance.IncrementScore(gameObject.tag);

            GameObject explosion = Instantiate(deathExplosion, transform.position, transform.rotation);
            explosion.transform.localScale = new Vector3(2f, 2f, 1);

            ScreenShakeScript.Instance.Shake(0.5f, 0.3f);
            Destroy(gameObject);
        }
    }
}
