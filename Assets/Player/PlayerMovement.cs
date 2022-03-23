using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationRatio;
    [SerializeField] private GameObject electricDestroyParticle;
    [SerializeField] private List<Transform> pathPoints;

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
        _inputValue = _playerInput.GetInputValue();
        _horizontalPos = Mathf.Clamp(_horizontalPos + _inputValue, -2.5f, 2.5f);
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
        Vector3 lerpPos = Vector3.Lerp(startTransform.position, targetTransform.position, _distanceTravelled);
        Quaternion lerpRot = Quaternion.Lerp(startTransform.rotation,targetTransform.rotation,_distanceTravelled);

        transform.position = new Vector3(lerpPos.x, transform.position.y, lerpPos.z);
        transform.position  = transform.TransformPoint(0,0,-_horizontalPos);
        
        transform.rotation = lerpRot;
        transform.eulerAngles += new Vector3(0, Mathf.Clamp(_inputValue * rotationRatio,-45,45) , 0);
    }

    private void SetSlowSpeed()
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
