using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraFollower : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private float distance;
    private float lerpSpeed = 0.3f;

    private void Start()
    {
        distance = Vector3.Distance(playerCamera.transform.position, this.transform.position);
    }

    private void Update()
    {
        Vector3 newPos = playerCamera.transform.position + playerCamera.transform.forward * distance;
        this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed);

        //this.transform.LookAt(playerCamera.transform, Vector3.up);
    }
}
