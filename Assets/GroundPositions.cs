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
        leftBorder = -5;
        rightBorder = -5;
    }
}
