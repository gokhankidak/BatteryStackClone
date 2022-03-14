using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _inputPos;
    private float _textureXOffset = 0;
    private GroundPositions _ground;
    private Rigidbody _rb;
    private float _inputValue,_velocity,_leftBorder,_rightBorder;
    
    [SerializeField] private float textureSpeed = .5f;
    [SerializeField] private float speed = 5f;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SetNewBorders(collision.gameObject.GetComponent<GroundPositions>());
        }
    }

    private void Update()
    {
        _inputValue = _playerInput.GetInputValue();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBattery();
        RotateTexture();
    }

    private void LateUpdate()
    {
        KeepInBorders();
    }

    void RotateTexture()
    { 
        _textureXOffset += speed * Time.deltaTime * textureSpeed;
        transform.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2( _textureXOffset,0));
    }
    void MoveBattery()
    {
        _rb.velocity = Vector3.right * speed;
        transform.position = transform.position + new Vector3(0, 0, -_inputValue) * Time.deltaTime;
    }

    void SetNewBorders(GroundPositions positions)
    {
        _leftBorder = positions.LeftBorder;
        _rightBorder = positions.RigtBorder;
        
    }

    private void KeepInBorders()
    {
        float _batteryScale = gameObject.GetComponent<Renderer>().bounds.size.z;
        transform.position = new Vector3(transform.position.x,transform.position.y,Mathf.Clamp(transform.position.z, _leftBorder + _batteryScale / 2,
            _rightBorder - _batteryScale / 2));
    }
}
