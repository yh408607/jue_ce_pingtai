using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlowTraget : MonoBehaviour
{



    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public Vector3 fixedRotation;

    private void Start()
    {
        
    }



    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.eulerAngles = fixedRotation;
    }
}
