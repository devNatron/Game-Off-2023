using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DollyZoom : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    private float zoomMultiplier = 4f;
    private float velocity = 0f;

    [SerializeField]
    private float smoothTime = 0.25f;
    private Camera camera;
    private float initialFrustumHeight;

    void Start()
    {
        camera = GetComponent<Camera>();
        float distanceFromTarget = Vector3.Distance(transform.position, target.position);
        initialFrustumHeight = ComputeFrustumHeight(distanceFromTarget);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float zoomInput = scroll * zoomMultiplier;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float zoomDistance = zoomInput * smoothTime * Time.deltaTime;
            camera.transform.Translate(Vector3.forward * zoomDistance, Space.World);

            float currentDistance = Vector3.Distance(transform.position, target.position);
            camera.fieldOfView = ComputeFieldOfView(initialFrustumHeight, currentDistance);
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
}
