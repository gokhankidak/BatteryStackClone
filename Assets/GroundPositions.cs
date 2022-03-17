using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPositions : MonoBehaviour
{
    private float leftBorder, rightBorder;
    
    public float LeftBorder { get {return leftBorder;}}
    public float RigtBorder { get {return rightBorder;}}

    // Start is called before the first frame update
    void Start()
    {
        leftBorder = gameObject.GetComponent<Collider>().bounds.min.z;
        rightBorder = gameObject.GetComponent<Collider>().bounds.max.z;
        
        //TEST PURPOSE ONLY
        leftBorder = -5;
        rightBorder = 5; 
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
