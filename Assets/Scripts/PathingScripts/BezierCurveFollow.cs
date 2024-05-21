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
    private float rotationSpeed = 250f;
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

        while (tParam <= 1)
        {
            tParam += Time.deltaTime * speedModifier;

            if (routeToGo == 0 && tParam <= 0.25f)
                rotationSpeed = 99999999999f;
            else
                rotationSpeed = 250f;

            gameObjectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            float angle = Mathf.Atan2(gameObjectPosition.y - transform.position.y, gameObjectPosition.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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

    public void SetRoutes(Transform[] setOfRoutes)
    {
        routes = setOfRoutes;
    }

    public void ResetVariables()
    {
        transform.gameObject.GetComponent<EnemyPlaneScript>().enabled = false;
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 180));
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