using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{

    private float damageMultiplier = 1;
    private float bulletSpeed = 25f;
    private float yBulletBoundary = 15f;

    private void Update()
    {
        transform.position += transform.up * Time.unscaledDeltaTime * bulletSpeed;

        if (transform.position.y > yBulletBoundary)
        {
            Destroy(gameObject);
        }

        if (Time.timeScale == 0)
        {
            damageMultiplier = 0.75f;
        }
        else
        {
            damageMultiplier = 1f;
        }

        List<Collider2D> results = new List<Collider2D>();
        int numOfCollisions = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), results);

        for (int i = 0; i < numOfCollisions; i++)
        {
            if (results[i].gameObject.layer == 7)
            {
                if (results[i].gameObject.CompareTag("EnemyPlane"))
                {
                    results[i].gameObject.GetComponent<EnemyPlaneScript>().HitByObject(1 * damageMultiplier);
                    Destroy(gameObject);
                }
                else if (results[i].gameObject.CompareTag("EnemyTank"))
                {
                    results[i].gameObject.GetComponent<EnemyTankScript>().HitByObject(1 * damageMultiplier);
                    Destroy(gameObject);
                }
                else if (results[i].gameObject.CompareTag("EnemyTurret"))
                {
                    results[i].gameObject.GetComponent<EnemyTurretScript>().HitByObject(1.5f * damageMultiplier);
                    Destroy(gameObject);
                }
                else if (results[i].gameObject.CompareTag("EnemyDiver"))
                {
                    results[i].gameObject.GetComponent<EnemyDiverPlaneScript>().HitByObject(1 * damageMultiplier);
                    Destroy(gameObject);
                }
                else if (results[i].gameObject.CompareTag("EnemySine"))
                {
                    results[i].gameObject.GetComponent<EnemySinePlaneScript>().HitByObject(1 * damageMultiplier);
                    Destroy(gameObject);
                }
                else if (results[i].gameObject.CompareTag("EnemyDelayed"))
                {
                    results[i].gameObject.GetComponent<EnemyDelayedScript>().HitByObject(1 * damageMultiplier);
                    Destroy(gameObject);
                }
            }
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyPlane"))
        {
            collision.gameObject.GetComponent<EnemyPlaneScript>().HitByObject(1 * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyTank"))
        {
            collision.gameObject.GetComponent<EnemyTankScript>().HitByObject(1 * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyTurret"))
        {
            collision.gameObject.GetComponent<EnemyTurretScript>().HitByObject(1.5f * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyDiver"))
        {
            collision.gameObject.GetComponent<EnemyDiverPlaneScript>().HitByObject(1f * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemySine"))
        {
            collision.gameObject.GetComponent<EnemySinePlaneScript>().HitByObject(1f * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyDelayed"))
        {
            collision.gameObject.GetComponent<EnemyDelayedScript>().HitByObject(1f * damageMultiplier);
            Destroy(gameObject);
        }
    }
    */

    public void SetDamageMultiplier(float _damageMultiplier)
    {
        damageMultiplier = _damageMultiplier;
    }

}
