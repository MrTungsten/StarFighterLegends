using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDiverPlaneScript : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject setOfWaypoints;
    [SerializeField] private GameObject enemyDiverVisual;
    [SerializeField] private GameObject deathExplosion;
    private GameObject player;
    private GameManagerScript gameManagerScript;
    private PowerupSpawnerScript powerupSpawnerScript;
    private Transform[] waypointsTransform;
    private Vector3[] waypoints;
    private bool isDashing = false;
    private float dashingPower = 20f;
    private float dashTime = 1.5f;
    private float returnSpeed = 5f;
    private float dashingCooldown = 5f;
    private float timer = 0f;
    private int waypointCount = -1;
    private bool isReturning = false;
    private float hitpoints = 25f;
    private bool hasSpawnedPowerup = false;
    private float rotateSpeed = 1000f;

    private void Start()
    {

        if (setOfWaypoints != null)
        {
            waypointsTransform = setOfWaypoints.GetComponentsInChildren<Transform>();
            Transform[] waypoints2 = waypointsTransform;
            waypointsTransform = new Transform[waypoints2.Length - 1];
            for (int i = 1; i < waypoints2.Length; i++)
            {
                waypointsTransform[i - 1] = waypoints2[i];
            }

            waypoints = new Vector3[waypointsTransform.Length];
            for (int i = 0; i < waypointsTransform.Length; i++)
            {
                waypoints[i] = waypointsTransform[i].position;
            }
        }
        else
        {
            waypoints = new Vector3[] { transform.position };
        }
        

        gameManagerScript = GameObject.FindAnyObjectByType<GameManagerScript>();

        powerupSpawnerScript = GameObject.FindAnyObjectByType<PowerupSpawnerScript>();

        player = GameObject.FindGameObjectWithTag("Player");

        dashingCooldown = Random.Range(4, 7);
        
    }

    private void Update()
    {

        if (!isDashing && !isReturning && gameManagerScript.IsGameActive())
        {
            if (timer > dashingCooldown)
            {
                StartCoroutine(Dash());
                timer = 0f;
                dashingCooldown = Random.Range(3, 5);
            }
            else
            {
                Vector3 direction = player.transform.position - transform.position;
                float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle + 90), rotateSpeed * Time.deltaTime);
                timer += Time.deltaTime;
            }
        }

        if (isReturning)
        {
            Vector3 direction = waypoints[waypointCount] - transform.position;
            float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);

            if (transform.position != waypoints[waypointCount])
            {
                transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointCount], returnSpeed * Time.deltaTime);
            }
            else
            {
                isReturning = false;
            }
        }

        if (!gameManagerScript.IsGameActive())
        {
            StopAllCoroutines();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    private IEnumerator Dash()
    {

        isDashing = true;

        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        rb.AddForce(direction * dashingPower);

        rb.AddForce(transform.up * dashingPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector3.zero;

        
        waypointCount = Random.Range(0, waypoints.Length);

        isDashing = false;

        transform.position = new Vector3(0f, 15f, 0f);

        isReturning = true;

    }

    public void HitByObject(float damageDone)
    {
        hitpoints -= damageDone;

        StartCoroutine(HitEffect());

        if (hitpoints <= 0 && !hasSpawnedPowerup)
        {
            hasSpawnedPowerup = true;

            int randomNum = Random.Range(1, 101);

            if (randomNum <= 10)
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
        enemyDiverVisual.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyDiverVisual.GetComponent<SpriteRenderer>().color = Color.white;
    }

}