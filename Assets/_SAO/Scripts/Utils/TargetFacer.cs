using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetFacer : MonoBehaviour
{
    private Quaternion _initialRotation;
    [SerializeField] private Transform _target;
    [SerializeField] private float _lerpSpeed = 0.05f;
    [SerializeField] private float _distance = 0.05f;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _rotationOffset;

    
    [SerializeField] private bool _followTarget = true;
    [SerializeField] private bool _faceTarget = true;
    [SerializeField] private bool _addInitialRotation = true;

    private void Start()
    {
        _initialRotation = this.transform.rotation;

        if (_followTarget)
            SnapToTarget(1);
    }

    private void SnapToTarget(float lerpSpeed)
    {
        Vector3 newPos = (_target.position + _target.forward * _distance) + _positionOffset;
        this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed);

        if (_faceTarget)
        {
            // Instead of using LookAt with a position, use LookRotation with a direction
            Vector3 lookDirection = _target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            
            // Apply the initial rotation first, then set the Y rotation back to 180 degrees
            Quaternion rotationResult = Quaternion.Euler(_initialRotation.eulerAngles) * targetRotation;
            rotationResult = Quaternion.Euler(rotationResult.eulerAngles.x + _rotationOffset.x, rotationResult.eulerAngles.y + _rotationOffset.y, rotationResult.eulerAngles.z + _rotationOffset.z);
            transform.rotation = rotationResult;
        }

        if (_addInitialRotation)
        {
            // Simply add the initial rotation directly using Quaternion multiplication
            Quaternion rotationResult = Quaternion.Euler(_initialRotation.eulerAngles) * transform.rotation;
            transform.rotation = rotationResult;
        }
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 1, Color.red);
        if (_followTarget)
            SnapToTarget(_lerpSpeed);
    }
}
