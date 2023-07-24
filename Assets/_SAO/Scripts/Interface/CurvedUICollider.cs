using CurvedUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CurvedUICollider : MonoBehaviour
{
    public Action InteractionDetected;

    private float planeWidth;
    private float bendFactor;
    [SerializeField] private CurvedUISettings mySettings;
    [SerializeField] private Canvas canvas;
    private BoxCollider boxCollider;

    // Use this for initialization
    void Awake()
    {
        if (mySettings == null)
            mySettings = GetComponentInParent<CurvedUISettings>();
        bendFactor = mySettings.Angle;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        planeWidth = canvas.pixelRect.width;
    }

    public void Init(Vector3 size, out CurvedUICollider c) {
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = size;
        c = this;
    }

    private void UpdatePosition()
    {
        // Update the rotation and position of the object to match the UI Canvas bend
        Vector3 positionInCanvasSpace = mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(this.transform.parent.position);
        transform.position = mySettings.CanvasToCurvedCanvas(positionInCanvasSpace);
    }

    private void UpdateRotation()
    {
        // Set the rotation
        Vector3 localPosition = canvas.transform.InverseTransformPoint(transform.position);
        float a = planeWidth / localPosition.x;
        Quaternion newRotation = Quaternion.Euler(0f, bendFactor / a, 0f);
        transform.rotation = canvas.transform.rotation * newRotation;
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateRotation();

        // Check if there is an interaction with the collider
#if UNITY_EDITOR
        if (InputHandler.Instance.WasLMousePressedThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputHandler.Instance.GetMousePos());
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1.0f);
            RaycastHit hitInfo;
            if (boxCollider.Raycast(ray, out hitInfo, 1000))
            {
                InteractionDetected?.Invoke();
            }
        }
#endif
    }
}
