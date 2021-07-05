using PlayerInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 0.05f;
    [SerializeField] private float lerpSpeed = 0.05f;
    private bool followTarget;

    private float maxHeight = 2;
    private float minHeight = 1.5f;

    private void Awake()
    {
        InterfaceManager.CatogoryMenuOpened += OnMenuOpened;
        InterfaceManager.CatogoryMenuClosed += OnMenuClosed;
    } 
    private void OnDestroy()
    {
        InterfaceManager.CatogoryMenuOpened -= OnMenuOpened;
        InterfaceManager.CatogoryMenuClosed -= OnMenuClosed;
    }

    private void OnMenuClosed() { followTarget = false; }
    private void OnMenuOpened() 
    {
        followTarget = true;
        SnapToTarget();
    }

    private void SnapToTarget()
    {
        Vector3 newPos = target.position + target.forward * distance;
        this.transform.position = newPos;

        //newPos.y = Mathf.Clamp(newPos.y, minHeight, maxHeight);
        this.transform.LookAt(2 * transform.position - target.position);
    }

    private void FixedUpdate()
    {
        if (followTarget)
        {
            Vector3 newPos = target.position + target.forward * distance;
            //newPos.y = Mathf.Clamp(newPos.y, minHeight, maxHeight);
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, lerpSpeed);
            this.transform.LookAt(2 * transform.position - target.position);
        }
    }
}
