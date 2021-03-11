using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [Range(0, 1)]
    public float shakeAmount = 0;

    public float maxDistance = 1;

    vThirdPersonCamera vCam;


    void Awake()
    {
        vCam = GetComponent<vThirdPersonCamera>();
    }


    void Update()
    {
        Shake();
    }

    void Shake()
    {
        if (shakeAmount > 0)
        {
            float x = Random.Range(-maxDistance * shakeAmount, maxDistance * shakeAmount);
            float y = Random.Range(-maxDistance * shakeAmount, maxDistance * shakeAmount);
            float z = Random.Range(-maxDistance * shakeAmount, maxDistance * shakeAmount);

            /*
            vCam.currentState.rotationOffSet.x = x;
            vCam.currentState.rotationOffSet.y = y;
            vCam.currentState.rotationOffSet.z = z;*/
        }
    }
}