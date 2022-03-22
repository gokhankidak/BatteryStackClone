using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailMarker : MonoBehaviour
{
    [SerializeField] private float followingDistance;
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
        var next = nextBattery.GetComponent<TailMarker>();
        if (next == null) return;
        Vector3 targetPos = next.markedPosition;
        Quaternion targetRot = next.markedRotation;

        var ratio = followingDistance*Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPos, ratio);
        transform.rotation = Quaternion.Lerp(transform.rotation,targetRot,ratio);
    }
}
