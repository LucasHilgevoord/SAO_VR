using UnityEngine;
using UnityEngine.InputSystem;

namespace AzureNature
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform playerBody;
        [SerializeField] private Camera playerCamera;

        [Header("Movement")]

        [SerializeField] private float walkSpeed = 5.0f;
        [SerializeField] private float runMultiplier = 3.0f;
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = -9.81f;

        private float _horizontalMovement;
        private float _verticalMovement;
        private float _currentSpeed;
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private CharacterController _characterController;
        private bool _isRunning;

        [Header("Mouse Look")]

        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float mouseVerticalClamp = 90.0f;

        private float _yAxis;
        private float _xAxis;
        private float _verticalRotation;

        [Header("Input System (optional)")]

        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference runAction;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            Movement();
            MouseLook();
        }

        private void OnEnable()
        {
            if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
            if (lookAction != null && lookAction.action != null) lookAction.action.Enable();
            if (jumpAction != null && jumpAction.action != null) jumpAction.action.Enable();
            if (runAction != null && runAction.action != null) runAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null && moveAction.action != null) moveAction.action.Disable();
            if (lookAction != null && lookAction.action != null) lookAction.action.Disable();
            if (jumpAction != null && jumpAction.action != null) jumpAction.action.Disable();
            if (runAction != null && runAction.action != null) runAction.action.Disable();
        }


        private void Movement()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            bool jumpPressed = false;
            if (jumpAction != null && jumpAction.action != null)
            {
                jumpPressed = jumpAction.action.triggered;
            }

            if (jumpPressed && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            if (moveAction != null && moveAction.action != null)
            {
                Vector2 mv = moveAction.action.ReadValue<Vector2>();
                _horizontalMovement = mv.x;
                _verticalMovement = mv.y;
            }

            _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;

            if (runAction != null && runAction.action != null)
            {
                _isRunning = runAction.action.ReadValue<float>() > 0.5f;
            }

            _currentSpeed = walkSpeed * (_isRunning ? runMultiplier : 1f);
            _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void MouseLook()
        {
            if (lookAction != null && lookAction.action != null)
            {
                Vector2 look = lookAction.action.ReadValue<Vector2>();
                _xAxis = look.x;
                _yAxis = look.y;
            }

            _verticalRotation += -_yAxis * mouseSensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
            playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, _xAxis * mouseSensitivity, 0);
        }
    }
}