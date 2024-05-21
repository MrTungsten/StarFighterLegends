using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneScript : MonoBehaviour
{
    public enum ShotType
    {
        normal,
        triple,
        cross
    }

    [SerializeField] private GameObject planeAimer1;
    [SerializeField] private GameObject planeAimer2;
    [SerializeField] private GameObject planeAimer3;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private bool autoMove = true;
    [SerializeField] private bool autoFire = true;
    [SerializeField] private ShotType shotType = ShotType.normal;
    [SerializeField] private GameObject enemyPlaneVisual;
    [SerializeField] private GameObject deathExplosion;
    private PowerupSpawnerScript powerupSpawnerScript;
    private GameManagerScript gameManagerScript;
    private PathingScript pathingScript;
    private GameObject player;
    private float speed = 5f;
    private float xBoundary = 14f;
    private float moveDir = 1f;
    private float timer = 0f;
    private float bulletCooldown = 1f;
    private float bulletSpeed = 5f;
    private bool hasSpawnedPowerup = false;
    private float hitpoints = 5f;
    private bool currentlyFiring = false;
    private int amountOfTimesFired = 0;

    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");

        if (Random.Range(0, 2) == 0)
            moveDir = 1f;
        else
            moveDir = -1f;

        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();
        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();

        pathingScript = this.GetComponent<PathingScript>();
        if (pathingScript != null)
        {
            pathingScript.SetPathingSpeed(speed);
            pathingScript.SetRotationSpeed(0);
        }

        switch (shotType)
        {
            case ShotType.normal:
                hitpoints = 8f;
                bulletSpeed = 7f;
                speed = 5f;
                break;
            case ShotType.triple:
                hitpoints = 10f;
                speed = 4f;
                break;
            case ShotType.cross:
                hitpoints = 15f;
                speed = 2f;
                bulletSpeed = 6f;
                break;
        }

    }

    private void Update()
    {

        if (autoMove)
        {
            Movement();
        }
        
        if (gameManagerScript.IsGameActive() && !currentlyFiring && autoFire)
        {
            FireAtPlayer();
        }
        else
        {
            timer = 0f;
        }

    }

    public void FireAtPlayer()
    {
        if (shotType == ShotType.normal)
        {
            planeAimer2.transform.up = (player.transform.position - planeAimer2.transform.position).normalized;

            if (timer > bulletCooldown)
            {
                NormalShot();
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else if (shotType == ShotType.triple)
        {
            planeAimer1.transform.rotation = Quaternion.Euler(0, 0, 167.5f);
            planeAimer2.transform.rotation = Quaternion.Euler(0, 0, 180f);
            planeAimer3.transform.rotation = Quaternion.Euler(0, 0, 192.5f);

            if (timer > bulletCooldown)
            {
                StartCoroutine(TripleShotBurst());
                timer = 0f;
                bulletCooldown = Random.Range(3f, 6f);
                amountOfTimesFired++;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else if (shotType == ShotType.cross)
        {
            if (timer > bulletCooldown)
            {
                StartCoroutine(CrossShotBurst());
                timer = 0f;
                bulletCooldown = Random.Range(5f, 10f);
                amountOfTimesFired++;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void NormalShot()
    {
        currentlyFiring = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GameObject spawnedBullet = Instantiate(bulletPrefab, planeAimer2.transform.position, planeAimer2.transform.rotation);
        spawnedBullet.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
        bulletSpeed += .1f;
        currentlyFiring = false;
    }

    private IEnumerator TripleShotBurst()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        currentlyFiring = true;
        speed = 0.25f;

        for (int i = 0; i < Mathf.Clamp(4 + amountOfTimesFired, 4, 8); i++)
        {
            GameObject spawnedBullet1 = Instantiate(bulletPrefab, planeAimer1.transform.position, planeAimer1.transform.rotation);
            GameObject spawnedBullet2 = Instantiate(bulletPrefab, planeAimer2.transform.position, planeAimer2.transform.rotation);
            GameObject spawnedBullet3 = Instantiate(bulletPrefab, planeAimer3.transform.position, planeAimer3.transform.rotation);
            spawnedBullet1.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            spawnedBullet2.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            spawnedBullet3.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            yield return new WaitForSeconds(0.25f);
        }

        speed = 3f;
        bulletSpeed += .1f;
        currentlyFiring = false;
    }

    private IEnumerator CrossShotBurst()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        currentlyFiring = true;
        speed = 0.05f;

        planeAimer1.transform.up = player.transform.position - planeAimer1.transform.position;
        planeAimer3.transform.up = player.transform.position - planeAimer3.transform.position;
        planeAimer1.transform.Rotate(new Vector3(10, 0, 0), Space.Self);
        planeAimer3.transform.Rotate(new Vector3(-10, 0, 0), Space.Self);

        for (int i = 0; i < Mathf.Clamp(8 + amountOfTimesFired, 8, 14); i++)
        {   
            GameObject spawnedBullet1 = Instantiate(bulletPrefab, planeAimer1.transform.position, planeAimer1.transform.rotation);
            GameObject spawnedBullet3 = Instantiate(bulletPrefab, planeAimer3.transform.position, planeAimer3.transform.rotation);
            spawnedBullet1.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            spawnedBullet3.GetComponent<EnemyBulletScript>().SetSpeed(bulletSpeed);
            yield return new WaitForSeconds(0.15f);
        }

        speed = 2f;
        bulletSpeed += .1f;
        currentlyFiring = false;
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
                powerupSpawnerScript.GetComponent<PowerupSpawnerScript>().SpawnPowerup(transform, "Bomb");
            }
            else if (randomNum <= 15)
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

    private void Movement()
    {
        if (shotType == ShotType.triple && gameManagerScript.IsGameActive())
        {
            if (transform.position.x < (player.transform.position.x - 3f))
            {
                if (transform.position.x > xBoundary)
                {
                    moveDir = -1f;
                }
                else
                {
                    moveDir = 1f;
                }
            }
            else if (transform.position.x >= (player.transform.position.x + 3f))
            {
                if (transform.position.x < -xBoundary)
                {
                    moveDir = 1f;
                }
                else
                {
                    moveDir = -1f;
                }
            }
        }
        else
        {
            if (transform.position.x > xBoundary)
            {
                moveDir = -1f;
            }
            else if (transform.position.x < -xBoundary)
            {
                moveDir = 1f;
            }            
        }
        
        transform.position += new Vector3(moveDir, 0, 0) * speed * Time.deltaTime;
    }

    public void SetAutoFire(bool _autoFire)
    {
        autoFire = _autoFire;
        timer = 0;
    }

    public void SetFireSpeed(float fireSpeed)
    {
        bulletCooldown = fireSpeed;
    }

}