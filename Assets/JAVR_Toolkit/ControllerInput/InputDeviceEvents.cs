//  Javier Alvarez-Calleja
//  SECAT Prototype 2019
//  VR Input Device Events
//
//  SECAT engine, VR Events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class InputDeviceEvents
{
    public delegate void InputDeviceButtonEvent(InputDeviceAction action, InputFeatureUsage usage , XRNode node, float value = 0.0f );
    public static event  InputDeviceButtonEvent OnInputDeviceButtonAction;

    public static void InputDeviceButtonAction(InputDeviceAction action, InputFeatureUsage usage, XRNode node, float value = 0.0f )
    {
        OnInputDeviceButtonAction?.Invoke(action, usage, node, value );
    }
}
