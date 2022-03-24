using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector3 playerPos
    {
        get => _pathLerpPos;
    }
    public Quaternion playerRot{
        get => _pathlerpRot;
    }
    
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationRatio;
    [SerializeField] private GameObject electricDestroyParticle;
    [SerializeField] private List<Transform> pathPoints;
    private float _rotationLimit = 30;
    private float _horizontalLimit = 2.5f;

    private Vector3 _pathLerpPos;
    private Quaternion _pathlerpRot;
    private Transform startTransform,targetTransform;
    private int index = 0;
    private float _inputPos,_distance,_distanceTravelled = 0;
    private float _inputValue,_velocity;
    private float _currentSpeed,_slowSpeed;
    private float _horizontalPos = 0;
    private float _batteryWidth;
    private PlayerInput _playerInput;

    // Start is called before the first frame update
    void Awake()
    {
        _batteryWidth = gameObject.GetComponent<Renderer>().localBounds.size.z * transform.localScale.z;
        _currentSpeed = speed;
        _slowSpeed = speed / 3;
        _playerInput = GetComponent<PlayerInput>();
        
        targetTransform = pathPoints[index];
        index++;
        SetDestinationPoints();
    }

    #region CollisionMethods
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grinder"))
        {
            batteryController.DestroyBattery();
            OnHitObstacle();
        }
        
        else if(collision.gameObject.layer == LayerMask.NameToLayer("BatteryBed"))
        {
            if (collision.gameObject.GetComponent<BatteryBedController>().capacity > 0)
            {
                collision.gameObject.GetComponent<BatteryBedController>().PlaceBattery();
                batteryController.DestroyBattery();
                OnHitObstacle();
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
        _horizontalPos = Mathf.Clamp(_horizontalPos + _inputValue, -_horizontalLimit, _horizontalLimit);
    }

    void FixedUpdate()
    {
        if(pathPoints.Count >= index)
            MoveBattery();
    }

    void SetDestinationPoints()
    {
        startTransform = targetTransform;
        targetTransform = pathPoints[index];
        _distance = Vector3.Distance(startTransform.position, targetTransform.position);
        _distanceTravelled = 0;
        index++;
    }
    
    void MoveBattery()
    {
        if (_distanceTravelled >= 1) SetDestinationPoints();

        _distanceTravelled += (1 / _distance) * _currentSpeed * Time.deltaTime;
        _pathLerpPos = Vector3.Lerp(startTransform.position, targetTransform.position, _distanceTravelled);
        _pathlerpRot = Quaternion.Lerp(startTransform.rotation,targetTransform.rotation, _distanceTravelled);

        transform.position = new Vector3(_pathLerpPos.x, transform.position.y, _pathLerpPos.z);
        transform.position  = transform.TransformPoint(0,0,-_horizontalPos);
        
        transform.rotation = _pathlerpRot;
        var rotationAmount = Mathf.Clamp(_inputValue * rotationRatio,-_rotationLimit,_rotationLimit);
        transform.eulerAngles += new Vector3(0, rotationAmount , 0);
    }

    private void OnHitObstacle()
    {
        float playerMoveBackDistance = _batteryWidth / _distance/4;
        
        _currentSpeed = _slowSpeed;
        _distanceTravelled -= playerMoveBackDistance;
        Instantiate(electricDestroyParticle, transform.position, Quaternion.identity);
        batteryController.isFollowing = false;
    }

    private void SetNormalSpeed()
    {
        _currentSpeed = speed;
        batteryController.isFollowing = true;
    }
}
