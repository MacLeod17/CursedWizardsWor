using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    public Vector3 targetOffset = Vector3.zero;
    public float distance = 2;
    public float height = 4;
    public float smoothSpeed = 5.0f;

    Vector3 cameraOffset;
    Transform cameraTransform;
    bool follow = false;

    void Start()
    {
        follow = (photonView.IsMine || !PhotonNetwork.IsConnected);

        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cameraTransform == null && follow) cameraTransform = Camera.main.transform;
        if (follow) UpdateCamera();
    }

    void UpdateCamera()
    {
        /*
        Vector3 direction = cameraTransform.position - transform.position;
        Vector3 v = Vector3.ClampMagnitude(direction, distance);
        v.y = transform.position.y + height;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, v, smoothSpeed * Time.deltaTime);
        cameraTransform.LookAt(transform.position + targetOffset);
        */

        cameraOffset.z = -distance;
        cameraOffset.y = height;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

        cameraTransform.LookAt(transform.position + targetOffset);
    }
}