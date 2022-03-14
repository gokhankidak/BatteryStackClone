using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour
{
    [SerializeField] private BatteryPrefSO batteryPref;
    private float _textureXOffset=0;
    private Renderer _renderer;
    // Start is called before the first frame update
    void Awake()
    {
        _renderer = transform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateTexture();
    }
    
    void RotateTexture()
    { 
        _textureXOffset += batteryPref.playerSpeed * Time.deltaTime * batteryPref.textureSpeed;
        _renderer.material.SetTextureOffset("_MainTex", new Vector2( _textureXOffset,0));
    }
}
