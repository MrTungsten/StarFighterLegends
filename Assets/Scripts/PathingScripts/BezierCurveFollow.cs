using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveFollow : MonoBehaviour
{

    public event EventHandler OnBezierFinish;

    private Transform[] routes;
    private float speedModifier = 0.4f;
    private int routeToGo = 0;
    private float tParam = 0;
    private Vector2 gameObjectPosition;
    private bool coroutineAllowed = false;
    private bool isActive = false;

    private void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = routes[routeNumber].position;
        Vector2 p1 = routes[routeNumber + 1].position;
        Vector2 p2 = routes[routeNumber + 2].position;
        Vector2 p3 = routes[routeNumber + 3].position;

        gameObjectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;
        float angle = Mathf.Atan2(gameObjectPosition.y - transform.position.y, gameObjectPosition.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
        transform.rotation = targetRotation;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            gameObjectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            angle = Mathf.Atan2(gameObjectPosition.y - transform.position.y, gameObjectPosition.x - transform.position.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 250 * Time.deltaTime);

            transform.position = gameObjectPosition;

            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;

        routeToGo += 4;

        if (routeToGo > routes.Length - 1)
        {
            isActive = false;
            OnBezierFinish?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            coroutineAllowed = true;
        }
    }

    private void Testing_OnBezierFinish(object sender, EventArgs e)
    {
        Debug.Log("Bezier Curve Finished!");
    }

    public void SetRoutes(Transform[] setOfRoutes)
    {
        routes = setOfRoutes;
    }

    public void ResetVariables()
    {
        transform.gameObject.GetComponent<EnemyPlaneScript>().enabled = false;
        coroutineAllowed = false;
        routeToGo = 0;
    }

    public void StartFollow()
    {
        coroutineAllowed = true;
        isActive = true;
    }

    public bool IsActive()
    {
        return isActive;
    }

}