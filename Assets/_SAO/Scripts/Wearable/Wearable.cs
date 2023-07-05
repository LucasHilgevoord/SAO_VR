using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Wearable : MonoBehaviour
{

    private bool _isWearing;
    public bool IsWearing => _isWearing;
    private bool _isGrabable => _grabInteractable != null;


    private Transform _parent;
    private Rigidbody _rigidbody;
    private XRGrabInteractable _grabInteractable;
    [SerializeField] private Vector3 _positionOffset;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartWearing(Transform parent)
    {
        _rigidbody.isKinematic = true;

        this.transform.position = parent.position + _positionOffset;
        this.transform.SetParent(parent);
        _parent = parent;
    }
}
