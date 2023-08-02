using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Wearable : MonoBehaviour
{

    private bool _isBeingWorn;
    public bool IsBeingWorn => _isBeingWorn;
    private bool _isGrabable => _grabInteractable != null;


    private Transform _parent;
    private Rigidbody _rigidbody;
    private XRGrabInteractable _grabInteractable;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _rotationOffset;

    private Collider[] colliders;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();

        colliders = GetComponentsInChildren<Collider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the object is picked up, we don't want to be worn if we are not grabbed
    }

    public virtual void StartWearing(Transform parent)
    {
        Debug.Log("ah?");
        _grabInteractable.enabled = false;
        _rigidbody.isKinematic = true;
        _rigidbody.interpolation = RigidbodyInterpolation.None;

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        // Set the object to the parent position
        this.transform.position = parent.position + _positionOffset;
        this.transform.SetParent(parent);

        // Set the rotation of the object to the rotation of the parent
        this.transform.rotation = parent.rotation;
        this.transform.Rotate(_rotationOffset);

        _parent = parent;

        _isBeingWorn = true;
    }
}
