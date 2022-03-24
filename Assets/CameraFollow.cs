using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   [SerializeField]  PlayerMovement pathPos;
   [SerializeField] Vector3 positionOffset;
   [SerializeField]  Vector3 rotationOffset;

   private Vector3 velocity;
   private void FixedUpdate()
   {
      transform.position = pathPos.playerPos ;
      transform.position  = transform.TransformPoint(positionOffset);
      transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, pathPos.playerRot.eulerAngles + rotationOffset,
         ref velocity, .2f);
   }
}
