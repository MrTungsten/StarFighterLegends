using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeScript : MonoBehaviour
{
    
    public static ScreenShakeScript Instance { get; private set; }
    private Transform cameraTransform;
    private Camera camera;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0.7f;
    private float dampingSpeed = 1.2f;
    private Vector3 initialPosition;
    private bool isShaking = false;

    private void Awake()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraTransform = camera.transform;
        initialPosition = new Vector3(0, 0, -10);

        CreateSingleton();
    }

    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isShaking)
        {
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeIntensity;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
    }

    private IEnumerator Shaking()
    {
        isShaking = true;
        yield return new WaitForSeconds(shakeDuration);
        isShaking = false;
        cameraTransform.localPosition = initialPosition;
    }

    public void Shake(float duration, float intensity)
    {
        initialPosition = cameraTransform.localPosition;
        shakeDuration = duration;
        shakeIntensity = intensity;
        StartCoroutine(Shaking());
    }

}
