using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.Overworld
{
    [RequireComponent(typeof(CharacterController))]
    public class OverworldController : MonoBehaviour
    {
        //[SerializeField] private Camera _camera;
        private CharacterController _controller;
        private Input _input;

        private bool _isGrounded;
        private float _pitchRotation;
        private Vector2 _mouseDelta;
        private Vector2 _moveDirection;
        public Vector3 velocity;
        [SerializeField] private Vector3 _controllerVelocity;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _gravity = 9.82f;
        [SerializeField] private Vector3 _gravityDirection;
        public bool useGravity = true;
        public Vector3 GravityDirection
        {
            get { return _gravityDirection; }
            set
            {
                _gravityDirection = value;
                _gravityDirection.Normalize();
            }
        }

        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckPosY;
        [SerializeField] private float _groundCheckRadius;
        Vector3 GroundCheckSpherePosition { get { return transform.position + Vector3.up * _groundCheckPosY; } }

        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] private float _turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;

        void Awake()
        {
            _input = new Input();
            _controller = GetComponent<CharacterController>();
            velocity = new Vector3();
            GravityDirection = _gravityDirection;
            //input.FPS.MoveDirection.performed += x => moveDirection = x.ReadValue<Vector2>();
            //input.FPS.MouseDelta.performed += x => mouseDelta = x.ReadValue<Vector2>();
        }

        public Vector3 GetRawVelocity { get { return _controllerVelocity + velocity; } }
        public Vector3 GetVelocity { get { return _controllerVelocity * Time.deltaTime + velocity * Time.deltaTime; } }

        public void SetPosition(Vector3 position)
        {
            _controller.Move(position - transform.position);
        }

        public void Move(Vector3 motion)
        {
            _controller.Move(motion * Time.deltaTime);
        }

        public void DoUpdate()
        {
            DoGroundCheck();
            HandleMovement();
        }

        private void DoGroundCheck()
        {
            // Character controller is a capsule shape hence bottom sphere
            _isGrounded = Physics.CheckSphere(GroundCheckSpherePosition, _groundCheckRadius, _groundMask, QueryTriggerInteraction.UseGlobal);

            if (_isGrounded && velocity.normalized == _gravityDirection)
            {
                velocity = Vector3.zero;
            }
        }

        private void HandleMovement()
        {
            _moveDirection = _input.Overworld.MoveDirection.ReadValue<Vector2>();

            _moveDirection.Normalize();

            Vector3 direction = new Vector3(_moveDirection.x, 0, _moveDirection.y).normalized;

            Vector3 vel = direction * _runSpeed;
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir * _runSpeed * Time.deltaTime);
            }

            if (useGravity)
                velocity += GravityDirection * _gravity * Time.deltaTime;

            _controller.Move(velocity * Time.deltaTime);

            _controllerVelocity = vel;
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheckSpherePosition, _groundCheckRadius);
        }
    }
}