using ExternalPropertyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedUICanvas : MonoBehaviour
{
    [Range(-90, 90)]
    public float Curve = 0;

    public Canvas canvas => GetComponent<Canvas>();
    public float CanvasWidth => canvas.pixelRect.width;
    public float CanvasHeight => canvas.pixelRect.height;

    public void GetElementPosition(RectTransform element, out float zPos, out float yRot)
    {
        // Get the position of the element in the canvas
        Vector3 position = canvas.transform.worldToLocalMatrix.MultiplyPoint3x4(element.position);
        float a = CanvasWidth / position.x;
        float distanceFromCenter = position.x;

        // Calculate the angle (in radians) from the center of the circle
        float angleRad = Mathf.Atan2(distanceFromCenter, CanvasWidth);

        // Calculate how much the element should be moved forward/backwards depending on the circular bending
        position.z = CanvasWidth * Mathf.Cos(angleRad) - CanvasWidth;
        zPos = position.z;

        float angle = Curve / a;
        yRot = angle;
    }
}
