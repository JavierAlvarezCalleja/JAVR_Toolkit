//  Javier Alvarez-Calleja
//  JAVR Framework 2020
//  Hand Pointable Element (Parent class for UI pointer-interactables)
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
//
//  Works by logging received Hand Pointer Data  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Collider))]
public class HandPointableElement : MonoBehaviour
{
    public virtual void OnHandPointerClick()
    {
    }
    public virtual void OnHandPointerEnter()
    {
    }
    public virtual void OnHandPointerExit()
    {
    }
   
}
