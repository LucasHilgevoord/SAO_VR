using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraFollower : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float distance = 2;
    [SerializeField] private float lerpSpeed = 0.05f;

    private float maxHeightDistance = 1;

    private void FixedUpdate()
    {
        Vector3 newPos = playerCamera.transform.position + playerCamera.transform.forward * distance;
        this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed);

        this.transform.LookAt(2 * transform.position - playerCamera.transform.position);
    }
}
