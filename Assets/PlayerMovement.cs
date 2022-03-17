using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _smooth = .5f;
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    
    private float _inputPos;
    private float _inputValue,_velocity,_leftBorder,_rightBorder;
    private float _currentSpeed,_slowSpeed;

    private float _batteryWidth;
    private Rigidbody _rb;
    private Transform _ground;
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
            SetNewBorders(collision.gameObject.GetComponent<GroundPositions>());
            StartCoroutine(RotateBattery(_ground));
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
        KeepInBorders();
    }

    void MoveBattery()
    {
        transform.Translate(new Vector3(_currentSpeed * Time.deltaTime,0,-_inputValue*Time.deltaTime),Space.Self);
    }

    void SetNewBorders(GroundPositions positions)
    {
        _leftBorder = positions.LeftBorder;
        _rightBorder = positions.RigtBorder;
    }

    IEnumerator RotateBattery(Transform ground)
    {
        float startTime = Time.time;

        while (Time.time < startTime + _smooth )
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, ground.rotation, (Time.time - startTime)*1000*Time.deltaTime / _smooth);
            if(transform.rotation.y == ground.rotation.y) break;
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    private void KeepInBorders()
    {
        if(_ground == null) return;
        
        var localPos = gameObject.transform.InverseTransformPoint(_ground.position) * transform.localScale.z;
        
        if (Math.Abs(localPos.z) > Mathf.Abs(_leftBorder + _batteryWidth/2) && transform.rotation.y == _ground.rotation.y)
        {
            float distance = Math.Abs(localPos.z) - Mathf.Abs(_leftBorder + _batteryWidth / 2);
            transform.Translate(new Vector3(0,0,distance*Mathf.Sign(localPos.z)),_ground); 
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
