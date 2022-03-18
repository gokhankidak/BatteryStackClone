using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailMarker : MonoBehaviour
{
    [SerializeField] private float followingDistance = .5f;
    public Transform nextBattery;
    [HideInInspector]
    public Vector3 markedPosition;
    [HideInInspector]
    public Quaternion markedRotation;

    private void OnEnable()
    {
        StartCoroutine(StartingFollowDelay(.1f));
    }

    IEnumerator StartingFollowDelay(float _time)
    {
        yield return new WaitForSeconds(_time);
    }
    private void FixedUpdate()
    {
        MarkPosition();
    }

    void MarkPosition()
    {
        markedPosition = transform.position;
        markedRotation = transform.rotation;
    }
    public void FollowNext()
    {
        if (nextBattery.GetComponent<TailMarker>() == null) return;
        Vector3 targetPos = nextBattery.GetComponent<TailMarker>().markedPosition;
        Quaternion targetRot = nextBattery.GetComponent<TailMarker>().markedRotation;

        transform.position = Vector3.Lerp(transform.position, targetPos, followingDistance*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation,targetRot,followingDistance*Time.deltaTime);
    }
}
