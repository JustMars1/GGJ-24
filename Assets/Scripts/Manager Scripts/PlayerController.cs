using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]  
    Camera _cam;

    PlayerInput _playerInput;
    Vector2 _lookInput;
    Vector2 _moveInput;

    float _cameraYAngle;
    float _cameraXAngle;

    bool _isGamepad;

    float _cameraBoomLength = 6.0f;
    float _cameraSensitivity = 0.5f;
    float _moveSpeed = 10.0f;

    Vector3 _forwardVec;
    Vector3 _rightVec;

    private Vector3 _groundHitLoc = Vector3.zero;

    [SerializeField] 
    LayerMask _slopeMask;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();

        _playerInput.PlayerInputActionMaps.Move.performed += OnMove;
        _playerInput.PlayerInputActionMaps.Look.performed += OnLook;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
        _isGamepad = ctx.control.device is Gamepad;
    }

    void Orient(float deltaTime)
    {
        Vector3 playerForward = transform.forward;
        Vector3 moveInputXZ = new Vector3(_moveInput.x, 0.0f, _moveInput.y);
        Vector3 rotatedMoveInputFwd = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) * moveInputXZ;
        Vector3 rotatedMoveInputRight = Vector3.Cross(rotatedMoveInputFwd, Vector3.up);
        float dotWithFwd = Vector3.Dot(playerForward, rotatedMoveInputFwd);
        float dotWithRight = Vector3.Dot(playerForward, rotatedMoveInputRight);
        float sign = Mathf.Approximately(dotWithRight, 0.0f) ? 1.0f : dotWithRight / Mathf.Abs(dotWithRight);

        dotWithFwd = Mathf.Clamp(dotWithFwd, -1.0f, 1.0f);
        
        float angleToMoveDir = sign * Mathf.Acos(dotWithFwd) * Mathf.Rad2Deg;
        
        if (float.IsNaN(angleToMoveDir))
        {
            Debug.Log("angleToMoveDir is nan"); 
        }
        
        if (moveInputXZ.magnitude > 0.0f)
        {
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0.0f, angleToMoveDir * 0.01f, 0.0f));
        }
    }

    void DrawDebugPoint(Vector3 origin, Color color)
    {
        float s = 0.1f;
        Debug.DrawLine(origin, origin + Vector3.up * s, color, 1.0f);
        Debug.DrawLine(origin, origin + Vector3.right * s, color, 1.0f);
        Debug.DrawLine(origin, origin + Vector3.forward * s, color, 1.0f);
    }

    //private void OnDrawGizmos()
   // {
    //    Gizmos.DrawSphere(_groundHitLoc, 0.1f);
   // }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDirection = _forwardVec * _moveInput.y - _rightVec * _moveInput.x;
        
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit1, 1.2f))
        {
            Debug.Log("Hit");
            _groundHitLoc = hit1.point;
            DrawDebugPoint(_groundHitLoc, Color.red);

            Vector3 slopeAxis = Vector3.Cross(hit1.normal, Vector3.up);
            float slopeAngle = -Mathf.Acos(Vector3.Dot(hit1.normal, Vector3.up)) * Mathf.Rad2Deg;

            moveDirection = Quaternion.AngleAxis(slopeAngle, slopeAxis) * moveDirection;
            Debug.DrawLine(transform.position, transform.position + slopeAxis, Color.blue);
            Debug.DrawLine(transform.position, transform.position + moveDirection, Color.green);
            Debug.Log(slopeAngle);
        }
        else
        {
            Debug.Log("No hit");
        }

        _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
        
        _rb.MovePosition(_rb.position + Time.fixedDeltaTime * _moveSpeed * moveDirection);
    }

    private void LateUpdate()
    {
        float ffInd = Time.deltaTime / (1.0f / 60.0f) * 3.0f;
        _cameraYAngle += _lookInput.x * _cameraSensitivity * (_isGamepad ? ffInd : 1.0f);
        _cameraXAngle -= _lookInput.y * _cameraSensitivity * (_isGamepad ? ffInd : 1.0f);
        _cameraXAngle = Mathf.Clamp(_cameraXAngle, -90, 90);
        
        _forwardVec = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) *  new Vector3(0.0f, 0.0f, 1.0f);
        _rightVec = Vector3.Cross(_forwardVec, Vector3.up);

        Vector3 camPos = -_forwardVec;
        camPos = Quaternion.AngleAxis(-_cameraXAngle, _rightVec) * camPos;
        
        _cam.transform.position = _rb.position + camPos*_cameraBoomLength;
        _cam.transform.rotation = Quaternion.Euler(_cameraXAngle, _cameraYAngle, 0.0f);
        
        Orient(Time.deltaTime);
    }
}