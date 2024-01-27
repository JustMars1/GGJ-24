using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    float _cameraBoomLength = 6.0f;
    float _moveSpeed = 3.0f;

    Vector3 _forwardVec;
    Vector3 _rightVec;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();

        _playerInput.PlayerInputActionMaps.Move.performed += OnMove;
        _playerInput.PlayerInputActionMaps.Look.performed += OnLook;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }

    void Orient(float deltaTime)
    {
        Vector3 playerForward = transform.forward;
        Vector3 moveInputXZ = new Vector3(_moveInput.x, 0.0f, _moveInput.y);
        Vector3 rotatedMoveInputFwd = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) * moveInputXZ;
        Vector3 rotatedMoveInputRight = Vector3.Cross(rotatedMoveInputFwd, Vector3.up);
        float dotWithFwd = Vector3.Dot(playerForward, rotatedMoveInputFwd);
        float dotWithRight = Vector3.Dot(playerForward, rotatedMoveInputRight);
        float sign = dotWithRight == 0.0f ? 1.0f : dotWithRight / Mathf.Abs(dotWithRight);
        
        Debug.Log(rotatedMoveInputFwd);
        
        if (moveInputXZ.magnitude > 0.0f)
        {
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0.0f, sign*Mathf.Acos(dotWithFwd), 0.0f));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDirection = _forwardVec * _moveInput.y - _rightVec * _moveInput.x;
        _rb.MovePosition(_rb.position + Time.fixedDeltaTime * _moveSpeed * moveDirection);
    }

    private void LateUpdate()
    {
        _cameraYAngle += _lookInput.x;
        _cameraXAngle += _lookInput.y;
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