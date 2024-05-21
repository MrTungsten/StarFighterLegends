using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretScript : MonoBehaviour
{

    [SerializeField] private GameObject bullets;
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private GameObject enemyTurretVisual;
    [SerializeField] private GameObject deathExplosion;
    private PowerupSpawnerScript powerupSpawnerScript;
    private GameManagerScript gameManagerScript;
    private float bulletSpeed = 2.5f;
    private float rotateSpeed = 100f;
    private float rotateAmount = 0f;
    private float rotationMultiplier = 20f;
    private float hitpoints = 75f;
    private bool hasSpawnedPowerup = false;
    private float firingSpeed = 0.4f;
    private float firingStartDelay = 1f;
    private float spawnerRadius = 0.75f;

    private void Start()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].transform.position = transform.position + new Vector3(Mathf.Cos((360f / spawners.Length) * (i + 1) * Mathf.Deg2Rad), Mathf.Sin((360f / spawners.Length) * (i + 1) * Mathf.Deg2Rad), 0) * spawnerRadius;
            spawners[i].transform.rotation = Quaternion.Euler(0, 0, (360f / spawners.Length) * (i + 1));
        }

        StartCoroutine(TurretHailFire());

        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();
        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();
    }

    private void Update()
    {
        rotateAmount += rotationMultiplier * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 10 * rotateAmount), rotateSpeed * Time.deltaTime);
    }

    private IEnumerator TurretHailFire()
    {
        yield return new WaitForSeconds(firingStartDelay);

        while (gameManagerScript.IsGameActive())
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                GameObject spawnedBullet = Instantiate(bullets, spawners[i].transform.position, spawners[i].transform.rotation);
                spawnedBullet.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            }

            yield return new WaitForSeconds(firingSpeed);
        }
    }

    private IEnumerator HitEffect()
    {
        enemyTurretVisual.GetComponent<SpriteRenderer>().color = Color.red;
        foreach (Transform child in enemyTurretVisual.transform)
        {
            child.GetComponent<SpriteRenderer>().color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        enemyTurretVisual.GetComponent<SpriteRenderer>().color = new Color(0f, 208f / 255f, 255f, 1f);
        foreach (Transform child in enemyTurretVisual.transform)
        {
            child.GetComponent<SpriteRenderer>().color = new Color(147f / 255f, 147f / 255f, 147f / 255f, 1f);
        }
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
            else if (randomNum <= 50)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Score");
            }

            ScoreManagerScript.Instance.IncrementScore(gameObject.tag);

            GameObject explosion = Instantiate(deathExplosion, transform.position, transform.rotation);
            explosion.transform.localScale = new Vector3(2f, 2f, 1);

            ScreenShakeScript.Instance.Shake(0.3f, 0.2f);
            Destroy(gameObject);
        }
    }

}