using System;
using System.Collections;
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && _ground != collision.transform)
        {
            StartCoroutine(RotateBattery());
            _ground = collision.transform;
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
        else if (other.gameObject.layer == LayerMask.NameToLayer("RotationTrigger"))
        {
            StartCoroutine(RotateBattery(other.gameObject));
        }
    }

    private void OnCollisionExit(Collision other)
    {
        SetNormalSpeed();
    }
    #endregion
    
    private IEnumerator RotateBattery(GameObject pivotObject)
    {
        float firstAngle = transform.rotation.eulerAngles.y;
        Vector3 rotationAxis = Vector3.up;
        if (gameObject.transform.position.z < transform.position.z) rotationAxis = Vector3.down;
        
        while (Mathf.Abs(firstAngle - transform.rotation.eulerAngles.y) < 90)
        {
            transform.RotateAround(transform.position, rotationAxis, 50 * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }
    
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
        transform.Translate(new Vector3(0,0,-_inputValue*Time.deltaTime),Space.Self);
        _rb.velocity =transform.TransformDirection(new Vector3(_currentSpeed * Time.deltaTime,_rb.velocity.y,0));

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
            transform.rotation = Quaternion.Slerp(transform.rotation, _ground.transform.rotation, (Time.time - startTime) / _smooth);
            yield return new WaitForSeconds(0.03f);
        }
        yield break;
    }
    
    private void KeepInBorders()
    {
        float batteryWidth = gameObject.GetComponent<Renderer>().localBounds.size.z*transform.localScale.z;
        var localPos = gameObject.transform.InverseTransformPoint(_ground.position)*transform.localScale.z;
        
        Debug.Log("abs : "+Mathf.Abs(_leftBorder + batteryWidth/2));
        
        if (Math.Abs(localPos.z) > Mathf.Abs(_leftBorder + batteryWidth/2))
         {
             transform.Translate(new Vector3(0,0,.1f*Mathf.Sign(localPos.z)),_ground); 
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
