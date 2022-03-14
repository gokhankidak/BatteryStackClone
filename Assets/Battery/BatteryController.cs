using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BatteryController : MonoBehaviour
{
    [SerializeField] private  GameObject firstBattery;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private int totalBatteriesCount = 4;
    private List<GameObject> _activeBatteriesPool = new List<GameObject>();
    private Stack<GameObject> _deactiveBatteriesPool = new Stack<GameObject>();

    private void Start()
    {
        AddBatteries(totalBatteriesCount);
    }

    //test purpose only
    public void AddOneBat()
    {
        AddBatteries(1);
    }
    
    public void AddBatteries(int batteryCountToAdd)
    {
        GameObject tempObject;

        if (_activeBatteriesPool.Count == 0)
        {
            tempObject = Instantiate(batteryPrefab);
            tempObject.GetComponent<TailFollow>().nextBattery = firstBattery.transform;
            _activeBatteriesPool.Add(tempObject);
            batteryCountToAdd--;
        }

        for (int i = 0; i <  batteryCountToAdd; i++)
        {
            if (_deactiveBatteriesPool.Count > 0)
            {
                tempObject = _deactiveBatteriesPool.Pop();
                tempObject.SetActive(true);
            }
            else
            {
                tempObject = Instantiate(batteryPrefab);
            }
            
            tempObject.GetComponent<TailFollow>().nextBattery = _activeBatteriesPool[_activeBatteriesPool.Count-1].transform;//to follow next battery
            tempObject.transform.position = firstBattery.transform.position;
            _activeBatteriesPool.Add(tempObject);
        }

        //totalBatteriesCount += batteryCountToAdd;
    }

    public void DestroyBattery()
    {
        GameObject tempObject;
        
        if (_activeBatteriesPool.Count > 0)
        {
            //TODO play anim for situations
            tempObject = _activeBatteriesPool[0];
            firstBattery.transform.position = tempObject.transform.position;
            _activeBatteriesPool[0].GetComponent<TailFollow>().nextBattery = firstBattery.transform;
            _activeBatteriesPool.RemoveAt(0);
            tempObject.SetActive(false);
            _deactiveBatteriesPool.Push(tempObject);
        }
        else
        {
            //Destroy(firstBattery);
            Debug.Log("Game Over");
        }
    }
}
