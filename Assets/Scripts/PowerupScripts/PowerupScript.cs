using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupScript : MonoBehaviour
{

    public enum PowerupType
    {
        bomb,
        laser,
        score
    }

    [SerializeField] PowerupType powerupType = PowerupType.bomb;
    private float fallSpeed = 0.75f;
    private float yBoundary = -17f;

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < yBoundary)
        {
            Destroy(gameObject);
        }

        List<Collider2D> objectsHit = new List<Collider2D>();
        int collision = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), objectsHit);

        foreach (Collider2D collider in objectsHit)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                switch (powerupType)
                {
                    case PowerupType.bomb:
                        collider.gameObject.GetComponent<PlayerScript>().GainedPowerup("Bomb");
                        break;
                    case PowerupType.laser:
                        collider.gameObject.GetComponent<PlayerScript>().GainedPowerup("Laser");
                        break;
                    case PowerupType.score:
                        collider.gameObject.GetComponent<PlayerScript>().GainedPowerup("Score");
                        break;
                }

                Destroy(gameObject);
            }
        }
    }

}