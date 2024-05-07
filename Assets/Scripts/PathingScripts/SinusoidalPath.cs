using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidalPath : MonoBehaviour
{

    private Vector3 position;
    private float moveSpeed, frequency, magnitude;
	private float moveDirection = 1f;
    private float timer = 0f;

    private void Start()
    {
        position += transform.position;

        if (moveSpeed == 0)
        {
            SetSettings(1, 4f, 5f, 1f);
        }
    }

	private void Update()
    {
        timer += Time.deltaTime;
		position += transform.up * moveSpeed * Time.deltaTime;
        transform.position = position + transform.right * Mathf.Sin(timer * frequency) * magnitude * moveDirection;
	}

    public void SetSettings(int waveDir, float _moveSpeed, float _frequency, float _magnitude)
    {
        moveSpeed = _moveSpeed;
        frequency = _frequency;
        magnitude = _magnitude;
        moveDirection = waveDir;
    }

}