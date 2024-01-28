using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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

    Vector3 _calculatedPlayerCameraLoc;
    Quaternion _calculatedPlayerCameraRot;

    Vector3 _cameraInitialLoc;
    Quaternion _cameraInitialRot;

    bool _isGamepad;

    float _cameraBoomLength = 6.0f;
    float _cameraSensitivity = 0.5f;
    float _moveSpeed = 10.0f;
    float _orientSpeed = 0.05f;

    Vector3 _forwardVec;
    Vector3 _rightVec;

    private Vector3 _groundHitLoc = Vector3.zero;

    [SerializeField] 
    LayerMask _slopeMask;

    bool _shouldMoveCameraToPlayerCamLoc = false;
    bool _canUpdatePlayerCamRotations = false;

    bool _canMove = false;

    Vector3 _globalSlopeDirection = Vector3.right;

    public bool isOnSlope = false;
    public bool isOnRamp = false;

    public bool isOnGround = false;

    Vector3 _cameraPivotOffset = new Vector3(0.0f, 2.0f, 0.0f);

    [SerializeField]
    float _krokoFatness = 0.0f;
    
    // bone transforms
    [SerializeField]
    Transform _rig;
    [SerializeField]
    Transform _head;
    
    [SerializeField]
    Transform _leftArm;
    [SerializeField]
    Transform _rightArm;
    
    [SerializeField]
    Transform _leftLeg;
    [SerializeField]
    Transform _rightLeg;
    
    [SerializeField]
    Transform _tail;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();

        _playerInput.PlayerInputActionMaps.Move.performed += OnMove;
        _playerInput.PlayerInputActionMaps.Look.performed += OnLook;
        _playerInput.PlayerInputActionMaps.Debug1.performed += OnDebug1Pressed;

        _cameraInitialLoc = _cam.transform.position;
        _cameraInitialRot = _cam.transform.rotation;
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

    void OnDebug1Pressed(InputAction.CallbackContext ctx)
    {
        StartCameraBlend();
    }

    public void OnFatnessChanged(float newFatness)
    {
        _krokoFatness = newFatness;
        Debug.Log("OnFatnessChanged called");
        Debug.LogFormat("OnFatness = {0}", _krokoFatness);
    }

    public void StartCameraBlend()
    {
        StartCoroutine(StartCameraBlendCo());
    }

    IEnumerator StartCameraBlendCo()
    {
        float time = 0.0f;
        while (time < 1.0f)
        {
            yield return null;
            time += Time.deltaTime / 2.0f;

            _cam.transform.position = Vector3.Lerp(_cameraInitialLoc, _calculatedPlayerCameraLoc, time);
            _cam.transform.rotation = Quaternion.Lerp(_cameraInitialRot, _calculatedPlayerCameraRot, time);
        }
        _shouldMoveCameraToPlayerCamLoc = true;
        _canUpdatePlayerCamRotations = true;
        _canMove = true;
        GameManager.manager.StartPlaying();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Orient()
    {
        if (_canMove)
        {
            // movement direction when walking normally
            Vector3 playerForward = transform.forward;
            Vector3 moveInputXZ = new Vector3(_moveInput.x, 0.0f, _moveInput.y);
            Vector3 rotatedMoveInputFwd = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) * moveInputXZ;
            Vector3 rotatedMoveInputRight = Vector3.Cross(rotatedMoveInputFwd, Vector3.up);
            float dotWithFwd = Vector3.Dot(playerForward, rotatedMoveInputFwd);
            float dotWithRight = Vector3.Dot(playerForward, rotatedMoveInputRight);
            float sign = Mathf.Approximately(dotWithRight, 0.0f) ? 1.0f : dotWithRight / Mathf.Abs(dotWithRight);

            dotWithFwd = Mathf.Clamp(dotWithFwd, -1.0f, 1.0f);
        
            float angleToMoveDir = sign * Mathf.Acos(dotWithFwd) * Mathf.Rad2Deg;

            // movement direction when sliding down slope
            float dotWithSlopeDir = Vector3.Dot(playerForward, _globalSlopeDirection);
            float dotWithSlopeAxis = Vector3.Dot(playerForward, Vector3.Cross(_globalSlopeDirection, Vector3.up));
            
            float signSlope = dotWithSlopeAxis == 0.0f ? 1.0f : dotWithSlopeAxis / Mathf.Abs(dotWithSlopeAxis);

            dotWithSlopeDir = Mathf.Clamp(dotWithSlopeDir, -1.0f, 1.0f);

            float angleToSlopeSlideDir = signSlope * Mathf.Acos(dotWithSlopeDir) * Mathf.Rad2Deg;
 
            if (moveInputXZ.magnitude > 0.0f && !isOnSlope)
            {
                _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0.0f, angleToMoveDir * _orientSpeed, 0.0f));
            }
            if (isOnSlope)
            {
                _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0.0f, angleToSlopeSlideDir * _orientSpeed, 0.0f));
            }
        }
    }

    void UpdatePlayerCamera()
    {
        if (_canUpdatePlayerCamRotations && Time.timeScale > 0.01f)
        {
            float ffInd = Time.deltaTime / (1.0f / 60.0f) * 3.0f;
            _cameraYAngle += _lookInput.x * _cameraSensitivity * (_isGamepad ? ffInd : 1.0f);
            _cameraXAngle -= _lookInput.y * _cameraSensitivity * (_isGamepad ? ffInd : 1.0f);
            _cameraXAngle = Mathf.Clamp(_cameraXAngle, -90, 90);
        }
        
        _forwardVec = Quaternion.AngleAxis(_cameraYAngle, Vector3.up) *  new Vector3(0.0f, 0.0f, 1.0f);
        _rightVec = Vector3.Cross(_forwardVec, Vector3.up);

        Vector3 camPos = -_forwardVec;
        camPos = Quaternion.AngleAxis(-_cameraXAngle, _rightVec) * camPos;
        
        _calculatedPlayerCameraLoc = _rb.position + _cameraPivotOffset + camPos*_cameraBoomLength;
        _calculatedPlayerCameraRot = Quaternion.Euler(_cameraXAngle, _cameraYAngle, 0.0f);
        
        if (_shouldMoveCameraToPlayerCamLoc)
        {
            _cam.transform.position = _calculatedPlayerCameraLoc;
            _cam.transform.rotation = _calculatedPlayerCameraRot;
        }
    }

    void DrawDebugPoint(Vector3 origin, Color color)
    {
        float s = 0.1f;
        Debug.DrawLine(origin, origin + Vector3.up * s, color, 1.0f);
        Debug.DrawLine(origin, origin + Vector3.right * s, color, 1.0f);
        Debug.DrawLine(origin, origin + Vector3.forward * s, color, 1.0f);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_canMove)
        {
            _rb.AddForce(Vector3.down*9.81f, ForceMode.Acceleration);
            if (isOnGround)
            {
                _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
            }
        
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

                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }
        
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit2, 1.2f, _slopeMask))
            {
                Debug.Log("On Slope");

                Vector3 slopeAxis = Vector3.Cross(_globalSlopeDirection, hit2.normal);
                Vector3 slopeSlideDir = Vector3.Cross(hit2.normal, slopeAxis);
                Debug.DrawLine(transform.position, transform.position + slopeSlideDir, Color.yellow);
            
                _rb.MovePosition(_rb.position + Time.fixedDeltaTime * 20.0f * slopeSlideDir + Time.fixedDeltaTime * _moveSpeed * moveDirection);

                if (Vector3.Dot(slopeSlideDir, Vector3.up) > 0.0f)
                {
                    isOnRamp = true;
                }

                isOnSlope = true;
                GetComponent<Animator>().SetBool("onSlope", true);
            }
            else
            {
                _rb.MovePosition(_rb.position + Time.fixedDeltaTime * _moveSpeed * moveDirection);
                if (isOnRamp)
                {
                    Debug.Log("Launch off ramp!");
                    _rb.AddForce(_globalSlopeDirection*15.0f+Vector3.up*5.0f, ForceMode.Impulse);
                    isOnRamp = false;
                }

                isOnSlope = false;
                GetComponent<Animator>().SetBool("onSlope", false);
            }
        }
    }

    private void LateUpdate()
    {
        UpdatePlayerCamera();
        Orient();
        
        _rig.transform.localScale = Vector3.Lerp(Vector3.one*100.0f, Vector3.one*500.0f, _krokoFatness);
        Vector3 limbScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), _krokoFatness);
        _head.transform.localScale = limbScale;
        _leftArm.transform.localScale = limbScale;
        _rightArm.transform.localScale = limbScale;
        _leftLeg.transform.localScale = limbScale;
        _rightLeg.transform.localScale = limbScale;
        _tail.transform.localScale = limbScale;
    }
}