using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float sideSpeed = 0.015f;

    private Vector3 _localChange;
    private float _startPosX;
    private float _velocity;


    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchBegan();
        }
        
        if (Input.GetMouseButton(0))
        {
            HandleTouchMoved();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnded();
        }
        
    }

    private void HandleTouchEnded()
    {
        _localChange=Vector3.zero;
    }

    private void HandleTouchMoved()
    {
        var position=Input.mousePosition.x;
        var deltaX = position - _startPosX;
        if (Math.Abs(deltaX) < 0.000001f)
        {
            _localChange=Vector3.zero;
            return;
        }
        _localChange = (Vector3.right * (deltaX * sideSpeed));
        //_startPosX = position;
    }

    private void HandleTouchBegan()
    {
        _startPosX = Input.mousePosition.x;
        _localChange=Vector3.zero;
    }

    public float GetInputValue()
    {
        return _localChange.x;
    }

}

