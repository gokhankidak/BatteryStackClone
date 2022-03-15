using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBedController : MonoBehaviour
{
    public int capacity = 3;
    Vector3 velocity = Vector3.zero;
    float smoothTime = .1f;
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
}
