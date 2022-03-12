using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float inputPos;
    private float _textureXOffset = 0;
    private GroundPositions _ground;
    private Rigidbody rb;
    [SerializeField] private float textureSpeed = .5f;
    [SerializeField] private float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMouseSwipe(InputValue input)
    {
        
        inputPos = input.Get<float>();
        Debug.Log("Input Pos is : " + inputPos);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBattery();
        RotateTexture();
    }

    void RotateTexture()
    { 
        _textureXOffset += speed * Time.deltaTime * textureSpeed;
        transform.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2( _textureXOffset,0));
    }
    void MoveBattery()
    {
        rb.velocity = Vector3.right * speed;
    }
}
