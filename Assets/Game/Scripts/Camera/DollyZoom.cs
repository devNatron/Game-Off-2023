using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DollyZoom : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    [Range(0.1f, 10f)]
    float maxTargetDistance = 10f;

    [SerializeField]
    LayerMask targetLayerMask;

    [Header("Dolly")]
    [SerializeField]
    private float dollyMultiplier = 100f;

    [Header("Zoom")]
    [SerializeField]
    private float smoothTime = 0.25f;
    private Camera camera;
    private Vector3 originalCameraPosition;
    float frustumHeight;
    Transform lastHit;

    void Start()
    {
        camera = GetComponent<Camera>();
        originalCameraPosition = camera.transform.position;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.0f)
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (
                Physics.Raycast(
                    ray.origin,
                    ray.direction,
                    out hit,
                    maxTargetDistance,
                    targetLayerMask
                )
            )
            {
                if (hit.collider)
                {
                    ApplyDollyZoom(hit.collider.transform, ComputeDollyDistance(scroll));
                }
                else
                {
                    lastHit = null;
                }
            }
        }
        else if (scroll < 0.0f)
        {
            resumeDollyZoom(originalCameraPosition, ComputeDollyDistance(scroll));
        }
    }

    private float ComputeFrustumHeight(float distance)
    {
        return (2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }

    private float ComputeFieldOfView(float height, float distance)
    {
        return (2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg);
    }

    private float ComputeDollyDistance(float input)
    {
        float dollyInput = input * dollyMultiplier * 10f;
        float dollyDistance = dollyInput * smoothTime * Time.deltaTime;

        return dollyDistance;
    }

    private void ApplyDollyZoom(Transform target, float dollyDistance)
    {
        float distanceFromTarget = Vector3.Distance(camera.transform.position, target.position);

        if (lastHit == null)
        {
            originalCameraPosition = camera.transform.position;
            frustumHeight = ComputeFrustumHeight(distanceFromTarget);
        }

        Vector3 directionToTarget = (target.position - camera.transform.position).normalized;

        camera.transform.Translate(directionToTarget * dollyDistance, Space.World);
        float currentDistance = Vector3.Distance(transform.position, target.position);

        camera.fieldOfView = ComputeFieldOfView(frustumHeight, currentDistance);
        lastHit = target;
    }

    private void resumeDollyZoom(Vector3 targetPosition, float dollyDistance)
    {
        Vector3 directionToOrigin = (camera.transform.position - targetPosition).normalized;

        camera.transform.Translate(directionToOrigin * dollyDistance, Space.World);
        float currentDistance = Vector3.Distance(transform.position, lastHit.position);

        camera.fieldOfView = ComputeFieldOfView(frustumHeight, currentDistance);
    }
}
