using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wearer : MonoBehaviour
{

    private Wearable _currentWearable;
    [SerializeField] private float _detectRadius;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private LayerMask _objectLayer;
    private bool _isWearing;


    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        if (!_isWearing)
        {
            DetectWearable();
        }
    }

    private void DetectWearable()
    {
        // Use a spherecast to detect objects within the range
        Collider[] colliders = Physics.OverlapSphere(transform.position + _positionOffset, _detectRadius, _objectLayer);

        // Check if any objects were detected
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Wearable wearable = colliders[i].gameObject.GetComponent<Wearable>();
                if (wearable != null)
                {
                    // Don't allow the player to wear something that is already being worn
                    if (wearable.IsBeingWorn == false)
                    {
                        _isWearing = true;
                        wearable.StartWearing(transform);
                        _currentWearable = wearable;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _positionOffset, _detectRadius);
    }
}
