using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveFollow : MonoBehaviour
{

    [SerializeField] private Transform[] routes;
    /*[SerializeField]*/ private float speedModifier = 0.4f;
    [SerializeField] private float endToStartDelay = 2f;
    private int routeToGo = 0;
    private float tParam = 0;
    private Vector2 gameObjectPosition;
    private bool coroutineAllowed = true;
    private bool startFollow = false;

    private void Update()
    {
        if (coroutineAllowed && startFollow)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = routes[routeNumber].GetChild(0).position;
        Vector2 p1 = routes[routeNumber].GetChild(1).position;
        Vector2 p2 = routes[routeNumber].GetChild(2).position;
        Vector2 p3 = routes[routeNumber].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            gameObjectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            float angle = Mathf.Atan2(gameObjectPosition.y - transform.position.y, gameObjectPosition.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 250 * Time.deltaTime);

            transform.position = gameObjectPosition;

            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;

        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            StartCoroutine(EndToStartDelay());
        }
        else
        {
            coroutineAllowed = true;
        }
    }

    private IEnumerator EndToStartDelay()
    {
        transform.gameObject.GetComponent<EnemyPlaneScript>().enabled = false;
        yield return new WaitForSeconds(endToStartDelay);
        transform.gameObject.GetComponent<EnemyPlaneScript>().enabled = true;

        coroutineAllowed = true;
        routeToGo = 0;
    }

    public void StartFollow()
    {
        startFollow = true;
    }

}