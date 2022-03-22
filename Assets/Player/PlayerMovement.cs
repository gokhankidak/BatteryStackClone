using System;
using System.Collections;
using UnityEngine;
using PathCreation;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject electricDestroyParticle;
    [SerializeField] private PathCreator pathCreator;
    
    private float _inputPos,_distanceTravelled = 0;
    private float _inputValue,_velocity;
    private float _currentSpeed,_slowSpeed;
    private float _horizontalPos = 0;
    private float _batteryWidth;
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grinder"))
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
        _inputValue = _playerInput.GetInputValue()/100;
        _horizontalPos = Mathf.Clamp(_horizontalPos + _inputValue, -2.5f, 2.5f);
    }

    void FixedUpdate()
    {
        MoveBattery();
    }

    void MoveBattery()
    {
        _distanceTravelled += _currentSpeed * Time.deltaTime;
        var path = pathCreator.path;
        var point = path.GetPointAtDistance(_distanceTravelled);

        transform.position = new Vector3(point.x, transform.position.y, point.z);
        transform.position  = transform.TransformPoint(0,0,-_horizontalPos);
        transform.eulerAngles = new Vector3(0, _inputValue*1000 + path.GetRotationAtDistance(_distanceTravelled).eulerAngles.y-90, 0);
    }

    private void SetSlowSpeed()
    {
        _distanceTravelled -= _batteryWidth/3;
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
