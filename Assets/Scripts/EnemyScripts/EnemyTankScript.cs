using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankScript : MonoBehaviour
{

    [SerializeField] private GameObject tankAimer;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyTankVisual;
    [SerializeField] private GameObject deathExplosion;
    private PowerupSpawnerScript powerupSpawnerScript;
    private GameManagerScript gameManagerScript;
    private PathingScript pathingScript;
    private GameObject player;
    private float hitpoints = 7f;
    private float timer = 0f;
    private float bulletCooldown = 3f;
    private float bulletSpeed = 5f;
    private float speed = 2.5f;
    private float rotationSpeed = 125f;
    private bool hasSpawnedPowerup = false;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();

        pathingScript = this.GetComponent<PathingScript>();
        if (pathingScript != null)
        {
            pathingScript.SetPathingSpeed(speed);
            pathingScript.SetRotationSpeed(rotationSpeed);
        }

        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();
    }

    private void Update()
    {
        if (gameManagerScript.IsGameActive())
        {
            FireAtPlayer();
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

            if (randomNum <= 15)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Bomb");
            }
            else if (randomNum <= 25)
            {
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Score");
            }

            ScoreManagerScript.Instance.IncrementScore(gameObject.tag);

            GameObject explosion = Instantiate(deathExplosion, transform.position, transform.rotation);
            explosion.transform.localScale = new Vector3(1.5f, 1.5f, 1);

            Destroy(gameObject);
        }
    }

    private IEnumerator HitEffect()
    {
        enemyTankVisual.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyTankVisual.GetComponent<SpriteRenderer>().color = new Color(214f / 255f, 0f, 1f, 1f);
    }

    private void FireAtPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
        tankAimer.transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        if (timer > bulletCooldown)
        {
            GameObject spawnedBullet = Instantiate(bulletPrefab, tankAimer.transform.position + new Vector3(-0.5f, 0, 0), tankAimer.transform.rotation);
            spawnedBullet.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            bulletCooldown = Random.Range(2f, 4f);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

}