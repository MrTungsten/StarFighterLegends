using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingScript : MonoBehaviour
{

    public enum PathingType
    {
        normal,
        delayed
    }

    [SerializeField] private GameObject path;
    [SerializeField] private Transform startingWaypoint;
    [SerializeField] private Transform endWaypoint;
    [SerializeField] private PathingType pathType = PathingType.normal;
    private Transform[] totalPathArr;
    private Transform[] pathArr;
    private int waypointsIndex = 0;
    private float pathingSpeed = 5f;
    private float rotationSpeed = 150f;
    private float timer = 0f;
    private float delayedTimer = 5.6f;

    private void Start()
    {
        totalPathArr = path.GetComponentsInChildren<Transform>();
        int startingIndex = Array.IndexOf(totalPathArr, startingWaypoint);
        int endingIndex = Array.IndexOf(totalPathArr, endWaypoint);

        if (startingIndex < endingIndex)
        {
            int count = 0;
            pathArr = new Transform[endingIndex - startingIndex + 1];

            for (int i = startingIndex; i <= endingIndex; i++)
            {
                pathArr[count] = totalPathArr[i];
                count++;
            }
        }
        else
        {
            int count = 0;
            pathArr = new Transform[startingIndex - endingIndex + 1];

            for (int i = startingIndex; i >= endingIndex; i--)
            {
                pathArr[count] = totalPathArr[i];
                count++;
            }
        }
    }

    private void Update()
    {
        if (pathType == PathingType.normal)
        {
            NormalPath();
        }
        else if (pathType == PathingType.delayed)
        {
            DelayedPath();
        }
    }

    private void NormalPath()
    {
        if (transform.position == pathArr[waypointsIndex].position)
        {
            waypointsIndex++;
            if (waypointsIndex >= pathArr.Length)
            {
                Array.Reverse(pathArr);
                waypointsIndex = 0;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pathArr[waypointsIndex].position, pathingSpeed * Time.deltaTime);
            float angle = Mathf.Atan2(pathArr[waypointsIndex].position.y - transform.position.y, pathArr[waypointsIndex].position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void DelayedPath()
    {
        if (transform.position == pathArr[waypointsIndex].position)
        {
            if (timer > delayedTimer)
            {
                waypointsIndex++;
                if (waypointsIndex >= pathArr.Length)
                {
                    Array.Reverse(pathArr);
                    waypointsIndex = 0;
                }
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pathArr[waypointsIndex].position, pathingSpeed * Time.deltaTime);
            float angle = Mathf.Atan2(pathArr[waypointsIndex].position.y - transform.position.y, pathArr[waypointsIndex].position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetPathingSpeed(float speed)
    {
        pathingSpeed = speed;
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
