using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack: MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField]private Transform target;

    private void Start() {
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;  // no smooth transition at the start
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
