using UnityEngine;

public class VRSpawnInFrontOfCamera : MonoBehaviour
{
    public Transform vrCamera; // Assign the VR camera in the Inspector or programmatically

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float originalDistanceToCamera;

    private void Start()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR camera not assigned!");
            return;
        }

        // Save the original position, rotation, and distance to the camera of the object
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalDistanceToCamera = Vector3.Distance(transform.position, vrCamera.position);

        // Move the object to the new position in front of the camera with the same distance and rotation
        MoveToFrontOfCamera();
    }

    private void MoveToFrontOfCamera()
    {
        // Calculate the direction from the camera to the original position of the object
        Vector3 directionToOriginalPosition = (originalPosition - vrCamera.position).normalized;

        // Calculate the new position of the object based on the original distance to the camera and the direction it's facing
        Vector3 newPosition = vrCamera.position + directionToOriginalPosition * originalDistanceToCamera;

        // Calculate the rotation of the object based on the original rotation and the rotation of the camera
        Quaternion newRotation = originalRotation * Quaternion.Inverse(vrCamera.rotation);

        // Set the new position and rotation of the object
        transform.SetPositionAndRotation(newPosition, newRotation);
    }

    private void Update()
    {
        MoveToFrontOfCamera();
    }
}
