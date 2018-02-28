using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {
    public float shakeAmount;
    public float setShakeTime;
    public float setShakeDelay;

    private Vector3 originalCameraPosition;
    private bool shaking;
    private float shakeTime;
    private float shakeDelay;

    public void Awake()
    {
        originalCameraPosition = transform.position;
        shaking = false;
        shakeTime = setShakeTime;
        shakeDelay = 0f;
    }

    public void Update()
    {
        if (shaking)
        {
            if (shakeDelay <= 0f)
            {
                transform.position = new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), -10f);
                shakeDelay = setShakeDelay;
            }

            shakeDelay -= Time.deltaTime;

            if (shakeTime <= 0f)
            {
                transform.position = originalCameraPosition;
                shaking = false;
            }

            shakeTime -= Time.deltaTime;
        }
    }

    public void StartShake()
    {
        shakeTime = setShakeTime;
        shaking = true;
    }
}
