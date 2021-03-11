using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        if (vThirdPersonCamera.instance != null && vThirdPersonCamera.instance._camera != null)
        {
            transform.LookAt(vThirdPersonCamera.instance._camera.transform.position, Vector3.up);
        }
    }
}
