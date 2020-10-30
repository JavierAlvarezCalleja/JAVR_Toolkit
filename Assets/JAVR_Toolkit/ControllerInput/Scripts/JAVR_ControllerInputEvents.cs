//  Javier Alvarez Calleja
//  JAVR Toolkit
//  Controller Input Events
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace JAVR
{
    public static class JAVR_ControllerInputEvents
    {
        public delegate void InputDeviceButtonEvent(JAVR_ControllerInputAction action, InputFeatureUsage usage, XRNode node, float value = 0.0f);
        public static event InputDeviceButtonEvent OnInputDeviceButtonAction;

        public static void InputDeviceButtonAction(JAVR_ControllerInputAction action, InputFeatureUsage usage, XRNode node, float value = 0.0f)
        {
            OnInputDeviceButtonAction?.Invoke(action, usage, node, value);
        }
    }
}
