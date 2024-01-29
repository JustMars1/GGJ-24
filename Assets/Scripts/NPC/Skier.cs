using System.Collections;
using System.Collections.Generic;
// using UnityEditor.UIElements;
using UnityEngine;

public class Skier : Eatable
{
    [SerializeField] 
    LayerMask _slopeMask;

    static Vector3 _globalSlopeDirection = Vector3.right;
    Rigidbody _rb;
    bool _isOnRamp;
    bool _isOnSlope;
    bool _isOnGround;

    float _skiSpeed = 5.0f;
    float _slopeSpeed = 10.0f;
    private Vector3 _rampImpulse = _globalSlopeDirection * 10.0f + Vector3.up * 3.0f;
    
    public bool gameStarted = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isOnGround = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit1, 1.2f);
        
        _rb.AddForce(Vector3.down*9.81f, ForceMode.Acceleration);
        if (_isOnGround)
        {
            _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
        }

        if (GameManager.manager.GetGameState() == GameManager.GameState.PLAYING)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit2, 1.2f, _slopeMask))
            {
                Debug.Log("Skier On Slope");

                Vector3 slopeAxis = Vector3.Cross(_globalSlopeDirection, hit2.normal);
                Vector3 slopeSlideDir = Vector3.Cross(hit2.normal, slopeAxis);
                Debug.DrawLine(transform.position, transform.position + slopeSlideDir, Color.yellow);
            
                _rb.MovePosition(_rb.position + Time.fixedDeltaTime * _slopeSpeed * slopeSlideDir);

                if (Vector3.Dot(slopeSlideDir, Vector3.up) > 0.0f)
                {
                    _isOnRamp = true;
                }

                _isOnSlope = true;
            }
            else
            {
                if (_isOnGround && !_isOnRamp)
                {
                    _rb.MovePosition(_rb.position + Time.fixedDeltaTime * _skiSpeed * _globalSlopeDirection);
                }
            
                if (_isOnRamp)
                {
                    Debug.Log("Launch off ramp!");
                    _rb.AddForce(_globalSlopeDirection*15.0f+Vector3.up*5.0f, ForceMode.Impulse);
                    _isOnRamp = false;
                }

                _isOnSlope = false;
            }
        }
    }
}
