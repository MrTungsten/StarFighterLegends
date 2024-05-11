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

    private Dictionary<int, GameObject> planesOnCurve;
    private Transform[] setOfRoutesTransform;
    private float planeDelayTimer = 0f;
    private bool isStartingBatch = false;
    private int shipCount = 1;
    private bool batchActive = true;
    private int numOfActivePlanes = 0;
    private float chanceOfShooting = 30f;

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
        for (int i = 1; i < (planesOnCurve.Count + 1); i++)
        {
            if (planesOnCurve[i] != null)
            {
                if (planesOnCurve[i].GetComponent<BezierCurveFollow>().IsActive())
                {
                    numOfActivePlanes++;
                }
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
            if ( shipCount < (planesOnCurve.Count + 1) )
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
    }

    private void SetUpChildren()
    {
        planesOnCurve = new Dictionary<int, GameObject>();
        int i = 1;
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<EnemyPlaneScript>() != null)
            {
                child.gameObject.GetComponent<EnemyPlaneScript>().SetAutoFire(false);
                planesOnCurve.Add(i, child.gameObject);
                BezierCurveFollow bezierCurveFollow = child.gameObject.GetComponent<BezierCurveFollow>();
                bezierCurveFollow.OnBezierFinish -= BezierCurveFollow_OnBezierFinish;
                bezierCurveFollow.OnBezierFinish += BezierCurveFollow_OnBezierFinish;
                child.gameObject.GetComponent<BezierCurveFollow>().SetRoutes(setOfRoutesTransform);
                child.gameObject.GetComponent<BezierCurveFollow>().ResetVariables();
            }
            else
            {
                planesOnCurve.Add(i, null);
            }
            i++;
        }

        for (int j = 1; j < planesOnCurve.Count + 1; j++)
        {
            if (UnityEngine.Random.Range(0, 100f) < chanceOfShooting)
            {
                planesOnCurve[j].GetComponent<EnemyPlaneScript>().SetAutoFire(true);
                planesOnCurve[j].GetComponent<EnemyPlaneScript>().SetFireSpeed(3f);
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
        shipCount = 1;
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
