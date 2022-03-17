using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailFollow : MonoBehaviour
{
    [SerializeField] private float followingDistance = .5f;
    public Transform nextBattery;
    [HideInInspector]
    public Vector3 markedPosition;
    [HideInInspector]
    public Quaternion markedRotation;


    private void OnEnable()
    {
        StartCoroutine(FollowDelay(.1f));
    }

    IEnumerator FollowDelay(float _time)
    {
        yield return new WaitForSeconds(_time);
    }
    private void Update()
    {
        MarkPosition();
    }

    void FixedUpdate()
    {
        if(nextBattery != null)
            FollowNext();
    }

    void MarkPosition()
    {
        markedPosition = transform.position;
        markedRotation = transform.rotation;
    }
    void FollowNext()
    {
        if (nextBattery.GetComponent<TailFollow>() == null) return;
        Vector3 targetPos = nextBattery.GetComponent<TailFollow>().markedPosition;
        Quaternion targetRot = nextBattery.GetComponent<TailFollow>().markedRotation;

        transform.position = Vector3.Lerp(transform.position, targetPos, followingDistance*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation,targetRot,followingDistance*Time.deltaTime);
    }
    
}
