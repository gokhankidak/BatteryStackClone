using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _inputPos;
    private Rigidbody _rb;
    private float _inputValue,_velocity,_leftBorder,_rightBorder;
    private Transform _ground;
    [SerializeField] private float _smooth = .5f;
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    private float _currentSpeed,_slowSpeed;
    private PlayerInput _playerInput;
    private bool _onLeft = false, _onRight = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        _currentSpeed = speed;
        _slowSpeed = speed / 3;
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
    }

    #region CollisionMethods
    
    private void OnCollisionEnter(Collision collision)//başka Scriptin içine al
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _ground = collision.transform;
            SetNewBorders(collision.gameObject.GetComponent<GroundPositions>());
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("GroundRotation") && collision.transform != _ground)
        {
            Debug.Log("Collided");
            _ground = collision.transform;
            StartCoroutine(RotateBattery());
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
        int inputSpeed = 1;
        if(_onRight && _inputValue > 0) inputSpeed = 0; 
        if (_onLeft && _inputValue < 0 ) inputSpeed = 0;
        
        transform.Translate(new Vector3(_currentSpeed * Time.deltaTime,0,-_inputValue*Time.deltaTime*inputSpeed),Space.Self);
        //_rb.velocity =transform.TransformDirection(new Vector3(_currentSpeed * Time.deltaTime,_rb.velocity.y,0));
    }

    void SetNewBorders(GroundPositions positions)
    {
        _leftBorder = positions.LeftBorder;
        _rightBorder = positions.RigtBorder;
    }

    IEnumerator RotateBattery()
    {
        float startTime = Time.time;
        while (Time.time < startTime + _smooth )
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _ground.rotation, (Time.time - startTime) / _smooth);
            if(transform.rotation.y == _ground.rotation.y) break;
            yield return new WaitForSeconds(0.02f);
        }
        transform.rotation = _ground.rotation;
    }
    
    private void KeepInBorders()
    {
        float batteryWidth = gameObject.GetComponent<Renderer>().localBounds.size.z*transform.localScale.z;
        var localPos = gameObject.transform.InverseTransformPoint(_ground.position) * transform.localScale.z;
        
        if (Math.Abs(localPos.z) > Mathf.Abs(_leftBorder + batteryWidth/2))
        {
            if (Mathf.Sign((localPos.z)) > 0)
            {
                 _onRight = true;
            }
            else
            {
                 _onLeft = true;
            }
            
            transform.Translate(new Vector3(0,0,.025f*Mathf.Sign(localPos.z)),_ground); 
        }
        else
        {
            _onLeft = _onRight = false;
        }
    }
    
    private void SetSlowSpeed()
    {
        _currentSpeed = _slowSpeed;
    }

    private void SetNormalSpeed()
    {
        _currentSpeed = speed;
    }
}
