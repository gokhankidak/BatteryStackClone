using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailFollow : MonoBehaviour
{
    [SerializeField] private float followingDistance = .5f;
    public Transform nextBattery;

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowNext();
    }

    void FollowNext()
    {
        transform.position = Vector3.Lerp(transform.position, nextBattery.position, followingDistance);
        transform.rotation = Quaternion.Lerp(transform.rotation,nextBattery.rotation,followingDistance);
    }
    
}
