using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _inputPos;
    private float _textureXOffset = 0;
    [SerializeField] private float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMouseSwipe(InputValue input)
    {
        _inputPos = input.Get<float>();
        Debug.Log("Input Pos is : " + _inputPos);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBattery();
        RotateTexture();
    }

    void RotateTexture()
    { 
        _textureXOffset += speed * Time.deltaTime;
        gameObject.GetComponent<Material>().SetTextureOffset("_MainTex", new Vector2( _textureXOffset,0));
    }
    void MoveBattery()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

}
