using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySinePlaneScript : MonoBehaviour
{

    [SerializeField] private GameObject planeAimer1;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyPlaneVisual;
    [SerializeField] private GameObject deathExplosion;
    private GameManagerScript gameManagerScript;
    private PowerupSpawnerScript powerupSpawnerScript;
    private GameObject player;
    private float timer = 0f;
    private float bulletCooldown = 4f;
    private float bulletSpeed = 8f;
    private float sineWaveFrequency = 5f;
    private bool isShooting = false;
    private int amountOfBullets = 15;
    private int maxAmountOfBullets = 25;
    private float bulletFiringDelay = 0.08f;
    private bool hasSpawnedPowerup = false;
    private float hitpoints = 20f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();
        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();

        timer = bulletCooldown;
    }

    private void Update()
    {
        if (gameManagerScript.IsGameActive())
        {
            FireAtPlayer();
        }
    }

    private void FireAtPlayer()
    {
        if (!isShooting)
        {
            if (timer > bulletCooldown)
            {
                Vector3 dir = player.transform.position - transform.position;
                float angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
                planeAimer1.transform.rotation = Quaternion.Euler(0, 0, angle + 90);

                StartCoroutine(ShootHelix());

                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private IEnumerator ShootHelix()
    {
        isShooting = true;

        for (int i = 0; i < amountOfBullets; i++)
        {
            GameObject spawnedBullet1 = Instantiate(bulletPrefab, planeAimer1.transform.position, planeAimer1.transform.rotation);
            GameObject spawnedBullet2 = Instantiate(bulletPrefab, planeAimer1.transform.position, planeAimer1.transform.rotation);
            spawnedBullet1.GetComponent<SinusoidalPath>().enabled = true;
            spawnedBullet1.GetComponent<EnemyBulletScript>().SetAutoMove(false);
            spawnedBullet1.GetComponent<SinusoidalPath>().SetSettings(1, bulletSpeed, sineWaveFrequency, 1.5f);
            spawnedBullet2.GetComponent<SinusoidalPath>().enabled = true;
            spawnedBullet2.GetComponent<EnemyBulletScript>().SetAutoMove(false);
            spawnedBullet2.GetComponent<SinusoidalPath>().SetSettings(-1, bulletSpeed, sineWaveFrequency, 1.5f);
            yield return new WaitForSeconds(bulletFiringDelay);
        }

        if (amountOfBullets < maxAmountOfBullets)
        {
            amountOfBullets++;
        }

        if (bulletSpeed < 10f)
            bulletSpeed += .25f;
        if (sineWaveFrequency < 7f)
            sineWaveFrequency += .25f;

        isShooting = false;
    }

    public void HitByObject(float damageDone)
    {
        hitpoints -= damageDone;

        StartCoroutine(HitEffect());

        if (hitpoints <= 0 && !hasSpawnedPowerup)
        {
            hasSpawnedPowerup = true;

            int randomNum = Random.Range(1, 101);

            if (randomNum <= 3)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Laser");
            }
            else if (randomNum <= 20)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Bomb");
            }
            else if (randomNum <= 30)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Score");
            }

            ScoreManagerScript.Instance.IncrementScore(gameObject.tag);

            Instantiate(deathExplosion, transform.position, transform.rotation);

            ScreenShakeScript.Instance.Shake(0.3f, 0.2f);
            Destroy(gameObject);
        }
    }

    private IEnumerator HitEffect()
    {
        enemyPlaneVisual.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyPlaneVisual.GetComponent<SpriteRenderer>().color = Color.white;
    }

}