//  Javier Alvarez-Calleja
//  JAVR Toolkit
//  Laser Pointable Element (Parent class for pointer-interactables)
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
//
//  Works by logging received Hand Pointer Data  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace JAVR
{
    [RequireComponent(typeof(Collider))]
    public class JAVR_LaserPointableElement : MonoBehaviour
    {
        public virtual void OnLaserPointerClick()
        {
        }
        public virtual void OnLaserPointerEnter()
        {
        }
        public virtual void OnLaserPointerExit()
        {
        }

    }
}