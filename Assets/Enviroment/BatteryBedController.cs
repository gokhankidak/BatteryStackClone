using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBedController : MonoBehaviour
{
    public int capacity = 3;
    [SerializeField]private List<Transform> batteryBedPositions;
    [SerializeField] private GameObject batteryPrefab,player;
    Vector3 velocity = Vector3.zero;
    float smoothTime = .1f;

    private void Start()
    {
        for (int i = 0; i < capacity; ++i)
        {
            batteryBedPositions.Add(gameObject.transform.GetChild(capacity-i-1));
        }
        Debug.Log("capacity : "+batteryBedPositions.Count);
    }

    public void OnComplete()
    {
        StartCoroutine(MoveObject(transform.position + new Vector3(0, 5, 0)));
        gameObject.GetComponent<Collider>().enabled = false;
    }

    IEnumerator MoveObject(Vector3 targetPosition)
    {
        while (transform.position.y < targetPosition.y)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }

    public void PlaceBattery()
    {
        var batteryPos = batteryBedPositions[capacity - 1].transform.position;
        var battery = Instantiate(batteryPrefab,player.transform.position ,new Quaternion(0,180,0,0));
        battery.transform.DOMove(batteryPos,.3f,true);
        battery.transform.parent = transform;
        capacity--;
    }

}
