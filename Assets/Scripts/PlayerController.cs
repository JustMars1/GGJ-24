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

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        _cameraYAngle += _lookInput.x;
        _cameraXAngle += _lookInput.y;
        _forwardVec = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) *  new Vector3(0.0f, 0.0f, 1.0f);
        _rightVec = Vector3.Cross(_forwardVec, Vector3.up);

        Vector3 camPos = -_forwardVec;
        camPos = Quaternion.AngleAxis(-_cameraXAngle, _rightVec) * camPos;
        
        _cam.transform.position = _rb.position + camPos*_cameraBoomLength;
        _cam.transform.rotation = Quaternion.Euler(_cameraXAngle, _cameraYAngle, 0.0f);
    }
}
