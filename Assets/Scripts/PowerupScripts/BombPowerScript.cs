using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPowerScript : MonoBehaviour
{

    [SerializeField] private GameObject payload;
    [SerializeField] private GameObject explosionBubble;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AnimationCurve payloadSpeed;
    private CircleCollider2D explosionCircleCollider;
    private float payloadLifetime = 0f;
    private float payloadDuration = 1.75f;
    private float explosionLifetime = 3f;
    private float expansionSize = 17.5f;
    private float timer = 0f;
    private float explosionCheckTime = 0.4f;

    private void Start()
    {

        explosionCircleCollider = GetComponent<CircleCollider2D>();
        explosionCircleCollider.enabled = false;

        payload.SetActive(true);
        explosionBubble.SetActive(false);

        StartCoroutine(BombMechanism());

    }

    private IEnumerator BombMechanism()
    {
        payload.SetActive(true);
        yield return new WaitForSecondsRealtime(payloadDuration);

        payload.SetActive(false);

        explosionBubble.SetActive(true);
        explosionCircleCollider.enabled = true;

        float elapsedTime = 0f;
        float originalRadius = explosionCircleCollider.radius;

        AudioSource.PlayClipAtPoint(explosionSound, transform.position, 3f);

        while (elapsedTime < (explosionLifetime / 2))
        {
            float currentRadius = Mathf.Lerp(originalRadius, originalRadius * expansionSize, elapsedTime / (explosionLifetime / 4));
            transform.localScale = new Vector3(1, 1, 0) * currentRadius;

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(explosionLifetime);

        Destroy(this.gameObject);
    }

    private void Update()
    {

        if (payload.active)
        {
            payloadLifetime += Time.unscaledDeltaTime;
            transform.position += transform.up * payloadSpeed.Evaluate(payloadLifetime) * Time.unscaledDeltaTime;
        }

        if (explosionCircleCollider.isActiveAndEnabled)
        {
            if (timer > explosionCheckTime)
            {
                List<Collider2D> results = new List<Collider2D>();
                int numOfCollisions = Physics2D.OverlapCollider(explosionCircleCollider, new ContactFilter2D().NoFilter(), results);

                for (int i = 0; i < numOfCollisions; i++)
                {
                    if (results[i].gameObject.layer == 7)
                    {
                        if (results[i].gameObject.CompareTag("EnemyPlane"))
                        {
                            results[i].gameObject.GetComponent<EnemyPlaneScript>().HitByObject(2);
                        }
                        else if (results[i].gameObject.CompareTag("EnemyTank"))
                        {
                            results[i].gameObject.GetComponent<EnemyTankScript>().HitByObject(5);
                        }
                        else if (results[i].gameObject.CompareTag("EnemyTurret"))
                        {
                            results[i].gameObject.GetComponent<EnemyTurretScript>().HitByObject(3);
                        }
                        else if (results[i].gameObject.CompareTag("EnemyDiver"))
                        {
                            results[i].gameObject.GetComponent<EnemyDiverPlaneScript>().HitByObject(5);
                        }
                        else if (results[i].gameObject.CompareTag("EnemySine"))
                        {
                            results[i].gameObject.GetComponent<EnemySinePlaneScript>().HitByObject(5);
                        }
                        else if (results[i].gameObject.CompareTag("EnemyDelayed"))
                        {
                            results[i].gameObject.GetComponent<EnemyDelayedScript>().HitByObject(5);
                        }
                    }
                    else if (results[i].gameObject.CompareTag("EnemyBullet"))
                    {
                        Destroy(results[i].gameObject);
                    }
                }

                timer = 0f;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }
        }
    }

}