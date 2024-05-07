using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveBatch : MonoBehaviour
{

    [SerializeField] private List<GameObject> planesOnCurve;
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float planeDelay = 0.5f;

    private float planeDelayTimer = 0f;
    private bool isStartingBatch = false;

    private void Update()
    {
        if (startDelay <= 0f && !isStartingBatch)
            StartBatch();
        else
            startDelay -= Time.deltaTime;

        if (isStartingBatch)
        {
            if (planeDelayTimer <= 0f)
            {
                if (planesOnCurve.Count > 0)
                {
                    GameObject currentPlane = planesOnCurve[0];
                    planesOnCurve.RemoveAt(0);
                    currentPlane.GetComponent<EnemyPlaneScript>().enabled = true;
                    currentPlane.GetComponent<BezierCurveFollow>().StartFollow();
                    planeDelayTimer = planeDelay;
                }
            }
            else
            {
                planeDelayTimer -= Time.deltaTime;
            }
        }
    }

    private void StartBatch()
    {
        isStartingBatch = true;
    }

}
