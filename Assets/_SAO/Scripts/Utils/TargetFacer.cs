using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class TargetFacer : MonoBehaviour
{
    private Quaternion _initialRotation;
    [SerializeField] private Transform _target;
    [SerializeField] private float _lerpSpeed = 0.05f;
    [SerializeField] private float _distance = 0.05f;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _rotationOffset;
    [SerializeField] private Vector2 _limitPitch;

    
    [SerializeField] private bool _followTarget = true;
    [SerializeField] private bool _faceTarget = true;

    private void Start()
    {
        _initialRotation = this.transform.rotation;

        if (_followTarget)
            SnapToTarget(1);
    }

    private void SnapToTarget(float lerpSpeed)
    {
        Vector3 newPos = (_target.position + _target.forward * _distance) + _positionOffset;
        //if (_limitPitch != Vector2.zero)
        //{
        //    // Calculate the current pitch
        //    float targetPitch = Mathf.Atan2(_target.rotation.z, _target.rotation.w) * Mathf.Rad2Deg;
        //    float minYPos = _target.position.y + Mathf.Tan(_limitPitch.x * Mathf.Deg2Rad) * _distance + _positionOffset.y;
        //    float maxYPos = _target.position.y + Mathf.Tan(_limitPitch.y * Mathf.Deg2Rad) * _distance + _positionOffset.y;
        //    newPos.y = Mathf.Clamp(newPos.y, minYPos, maxYPos);
        //}
        this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed);

        if (_faceTarget)
        {
            Vector3 lookDirection = _target.position - (transform.position - _positionOffset);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            Quaternion rotationResult = Quaternion.Euler(_initialRotation.eulerAngles) * targetRotation;
            rotationResult = Quaternion.Euler(rotationResult.eulerAngles + _rotationOffset);
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
