using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _inputPos;
    private GroundPositions _ground;
    private Rigidbody _rb;
    private float _inputValue,_velocity,_leftBorder,_rightBorder;

    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    private float currentSpeed,slowSpeed;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        currentSpeed = speed;
        slowSpeed = speed / 2;
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
    }

    #region CollisionMethods
    
    private void OnCollisionEnter(Collision collision)//başka Scriptin içine al
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SetNewBorders(collision.gameObject.GetComponent<GroundPositions>());
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Grinder"))
        {
            batteryController.DestroyBattery();
            SetSlowSpeed();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("BatteryBed"))
        {
            if (collision.gameObject.GetComponent<BatteryBedController>().capacity > 0)
            {
                collision.gameObject.GetComponent<BatteryBedController>().capacity--;
                batteryController.DestroyBattery();
                SetSlowSpeed();
            }
            else
            {
                collision.gameObject.GetComponent<BatteryBedController>().OnComplete();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Battery"))
        {
            batteryController.AddOneBat();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        SetNormalSpeed();
    }
    
    #endregion
    
    private void Update()
    {
        _inputValue = _playerInput.GetInputValue();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBattery();
    }

    private void LateUpdate()
    {
        KeepInBorders();
    }

    void MoveBattery()
    {
        _rb.velocity =new Vector3(currentSpeed * Time.deltaTime,_rb.velocity.y,_rb.velocity.z);
        transform.position += new Vector3(0, 0, -_inputValue) * Time.deltaTime;
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
    
    private void SetSlowSpeed()
    {
        currentSpeed = slowSpeed;
    }

    private void SetNormalSpeed()
    {
        currentSpeed = speed;
    }

}
