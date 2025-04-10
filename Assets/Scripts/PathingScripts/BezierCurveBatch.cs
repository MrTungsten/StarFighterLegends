using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveBatch : MonoBehaviour
{

    [SerializeField] private GameObject setOfRoutes;
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float planeDelay = 0.5f;
    [SerializeField] private float batchDelay = 1.5f;

    private List<GameObject> planesOnCurve;
    private Transform[] setOfRoutesTransform;
    private float planeDelayTimer = 0f;
    private bool isStartingBatch = false;
    private int shipCount = 0;
    private bool batchActive = true;
    private int numOfActivePlanes = 0;
    private float chanceOfShooting = 50f;

    private void Start()
    {
        setOfRoutesTransform = setOfRoutes.GetComponentsInChildren<Transform>();
        List<Transform> waypoints2 = setOfRoutesTransform.ToList();
        for (int i = 0; i < waypoints2.Count; i++)
        {
            if  (waypoints2[i].gameObject.name.Contains("BezierRoute") || (waypoints2[i].gameObject.GetComponent<BezierRoute>() != null))
            {
                waypoints2.RemoveAt(i);
            }
        }
        waypoints2.RemoveAt(0);
        setOfRoutesTransform = waypoints2.ToArray();

        SetUpChildren();
    }

    private void BezierCurveFollow_OnBezierFinish(object sender, EventArgs e)
    {
        numOfActivePlanes = 0;
        for (int i = 1; i < planesOnCurve.Count; i++)
        {
            if (planesOnCurve[i].GetComponent<BezierCurveFollow>().IsActive())
            {
                numOfActivePlanes++;
            }
        }
        if (numOfActivePlanes == 0)
        {
            StartCoroutine(BezierCurveRest());
        }
    }

    private void Update()
    {
        if (startDelay <= 0f && !isStartingBatch)
            StartBatch();
        else
            startDelay -= Time.deltaTime;

        if (isStartingBatch && batchActive)
        {
            if ( shipCount < planesOnCurve.Count )
            {
                if (planeDelayTimer <= 0f)
                {
                    if (planesOnCurve[shipCount] != null)
                    {
                        planesOnCurve[shipCount].GetComponent<EnemyPlaneScript>().enabled = true;
                        planesOnCurve[shipCount].GetComponent<BezierCurveFollow>().StartFollow();
                    }
                    shipCount++;
                    planeDelayTimer = planeDelay;
                }
                else
                {
                    planeDelayTimer -= Time.deltaTime;
                }
            }
        }

        if (batchActive)
        {
            for (int i = 0; i < planesOnCurve.Count; i++)
            {
                if (planesOnCurve[i] == null)
                {
                    planesOnCurve.RemoveAt(i);
                    i--;

                    numOfActivePlanes = 0;
                    for (int j = 0; j < planesOnCurve.Count; j++)
                    {
                        if (planesOnCurve[j].GetComponent<BezierCurveFollow>().IsActive())
                        {
                            numOfActivePlanes++;
                        }
                    }
                    if (numOfActivePlanes == 0)
                    {
                        StartCoroutine(BezierCurveRest());
                    }
                }
            }
        }
        
    }

    private void SetUpChildren()
    {
        planesOnCurve = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<EnemyPlaneScript>() != null)
            {
                child.gameObject.GetComponent<EnemyPlaneScript>().SetAutoFire(false);
                planesOnCurve.Add(child.gameObject);
                BezierCurveFollow bezierCurveFollow = child.gameObject.GetComponent<BezierCurveFollow>();
                bezierCurveFollow.OnBezierFinish -= BezierCurveFollow_OnBezierFinish;
                bezierCurveFollow.OnBezierFinish += BezierCurveFollow_OnBezierFinish;
                bezierCurveFollow.SetRoutes(setOfRoutesTransform);
                bezierCurveFollow.ResetVariables();
            }
        }

        for (int i = 0; i < planesOnCurve.Count; i++)
        {
            if (UnityEngine.Random.Range(0, 100f) < chanceOfShooting)
            {
                planesOnCurve[i].GetComponent<EnemyPlaneScript>().SetAutoFire(true);
                planesOnCurve[i].GetComponent<EnemyPlaneScript>().SetFireSpeed(3f);
            }
        }
    }

    private void StartBatch()
    {
        isStartingBatch = true;
    }

    private IEnumerator BezierCurveRest()
    {
        batchActive = false;
        shipCount = 0;
        planeDelayTimer = 0f;
        SetUpChildren();
        yield return new WaitForSeconds(batchDelay);
        batchActive = true;
    }

/*
    private IEnumerator FireAtPlayer()
    {
        yield return new WaitForSeconds(2.5f);
        planesOnCurve[Random.Range(0, planesOnCurve.Count)].GetComponent<EnemyPlaneScript>().FireAtPlayer();
    }
*/
}
