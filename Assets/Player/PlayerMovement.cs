using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float smoothRotationAngle = 2f;
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject electricDestroyParticle;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform _ground ;
    
    private float _inputPos;
    private float _inputValue,_velocity,_leftBorder,_rightBorder;
    private float _currentSpeed,_slowSpeed;
    private float _batteryWidth;
    private bool _isRotating = false;

    private Rigidbody _rb;
    private PlayerInput _playerInput;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        _batteryWidth = gameObject.GetComponent<Renderer>().localBounds.size.z*transform.localScale.z;
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
            _ground = collision.transform;
            StartCoroutine(RotateBattery(_ground));
            _currentSpeed = 0;
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
                collision.gameObject.GetComponent<BatteryBedController>().PlaceBattery();
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
        if(other.gameObject.layer != LayerMask.NameToLayer("Ground"))
            SetNormalSpeed();
    }
    #endregion
    
    private void Update()
    {
        _inputValue = _playerInput.GetInputValue();
    }

    private void LateUpdate()
    {
        KeepInBorders();
    }

    void FixedUpdate()
    {
        MoveBattery();
    }

    void MoveBattery()
    {
        var moveRatio = _inputValue * _currentSpeed * Time.deltaTime / 1000;
        transform.Translate(new Vector3(_currentSpeed * Time.deltaTime,0,-moveRatio),Space.Self);
        if(!_isRotating) transform.eulerAngles = new Vector3(0, moveRatio*200 + _ground.eulerAngles.y, 0);
    }

    void SetNewBorders(GroundPositions positions)
    {
        _leftBorder = positions.LeftBorder;
        _rightBorder = positions.RigtBorder;
    }

    private void KeepInBorders()
    {
        if(_ground == null) return;
        var localPos = gameObject.transform.InverseTransformPoint(_ground.position) * transform.localScale.z;

        if (Math.Abs(localPos.z) > Mathf.Abs(_leftBorder + _batteryWidth/2))
        {
            float distance = Math.Abs(localPos.z) - Mathf.Abs(_leftBorder + _batteryWidth / 2);
            if(!_isRotating) transform.Translate(new Vector3(0,0,distance*Mathf.Sign(localPos.z)),_ground);
        }
    }
    IEnumerator RotateBattery(Transform ground)
    {
        _isRotating = true;
        transform.parent = pivotPoint;
        while (transform.rotation.y < ground.rotation.y)
        {
            pivotPoint.Rotate(new Vector3(0, smoothRotationAngle, 0));
            yield return new WaitForSeconds(0.01f);
        }

        _isRotating = false;
        transform.parent = null;
        transform.rotation = ground.rotation;
        SetNormalSpeed();
    }
    
    private void SetSlowSpeed()
    {
        _currentSpeed = _slowSpeed;
        Instantiate(electricDestroyParticle, transform.position, Quaternion.identity);
        batteryController.isFollowing = false;
    }

    private void SetNormalSpeed()
    {
        _currentSpeed = speed;
        batteryController.isFollowing = true;
    }
}
