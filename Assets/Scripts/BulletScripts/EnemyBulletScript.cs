using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    public enum BulletMovement
    {
        Normal,
        Delayed
    }

    [SerializeField] private bool autoMove = true;
    [SerializeField] private BulletMovement bulletMovement = BulletMovement.Normal;
    [SerializeField] private AnimationCurve delayedBulletAnimationCurve;
    private Rigidbody2D bulletRb;
    private float bulletSpeed = 5f;
    private float xBulletBoundary = 50f;
    private float yBulletBoundary = 50f;
    private float bulletLifetime = 0f;

    private void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
        bulletRb.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (autoMove)
        {
            if (bulletMovement == BulletMovement.Normal)
            {
                transform.position += transform.up * bulletSpeed * Time.deltaTime;
            }
            
            if (bulletMovement == BulletMovement.Delayed)
            {
                bulletLifetime += Time.deltaTime;
                bulletSpeed = delayedBulletAnimationCurve.Evaluate(bulletLifetime);
                transform.position += transform.up * bulletSpeed * Time.deltaTime;
            }
        }

        if (transform.position.x < -xBulletBoundary || transform.position.x > xBulletBoundary)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -yBulletBoundary || transform.position.y > yBulletBoundary)
        {
            Destroy(gameObject);
        }

        List<Collider2D> results = new List<Collider2D>();
        int numOfCollisions = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), results);
        foreach(Collider2D coll in results)
        if (coll.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public void SetAutoMove(bool shouldAutoMove)
    {
        autoMove = shouldAutoMove;
    }

}
