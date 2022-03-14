using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryController : MonoBehaviour
{
    [SerializeField] private GameObject firstBattery;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private int batteriesCount = 4;
    private List<GameObject> _batteriesList = new List<GameObject>();

    private void Start()
    {
        
    }

    void Update()
    {
        
    }
}
