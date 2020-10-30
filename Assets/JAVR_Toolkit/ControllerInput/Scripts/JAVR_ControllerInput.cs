//  Javier Alvarez Calleja
//  JAVR Toolkit
//  Controller Input (Universal)
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace JAVR
{
    public class JAVR_ControllerInput : MonoBehaviour
    {
        float _previousTriggerValue = 0.0f;
        float _previousGripValue = 0.0f;
        bool _previousMenuButtonValue = false;
        bool _previousPrimaryButton = false;
        bool _previousSecondaryButton = false;

        [SerializeField]
        private XRNode s_controllerNode;

#if USING_STEAMVR
        SteamVR_Input_Sources _hand;
        [SerializeField]
        private SteamVR_Action_Single s_triggerState;
        [SerializeField]
        private SteamVR_Action_Single s_gripState;
   

        private void OnEnable()
        { 
            _hand = controllerNode == XRNode.LeftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
        }

#endif
        private void Update()
        {

            bool controllerValid = false;
            InputDevice controllerDevice = InputDevices.GetDeviceAtXRNode(s_controllerNode);
#if USING_STEAMVR
            controllerValid = true;
#else
            controllerValid = controllerDevice.isValid;
#endif
            // Debug.Log("Input device: " + controllerDevice.ToString());
            if (controllerValid)
            {
                //Trigger
                float triggerValue = 0.0f;
                bool featureGrabbed;
#if USING_STEAMVR
            triggerValue = s_triggerState.GetAxis(_hand);
            featureGrabbed = true;
#else
                featureGrabbed = controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
#endif
                if (featureGrabbed)
                {
                    // Debug.Log("Trigger value: " + triggerValue);
                    if (triggerValue > 0.3f && _previousTriggerValue < 0.3f)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Press, (InputFeatureUsage)CommonUsages.trigger, s_controllerNode, triggerValue);
                        Debug.Log("Trigger pressed: " + s_controllerNode.ToString());
                    }
                    if (triggerValue < 0.001f && _previousTriggerValue > 0.001f)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Release, (InputFeatureUsage)CommonUsages.trigger, s_controllerNode, triggerValue);
                    }
                    _previousTriggerValue = triggerValue;
                }
                else
                {
                    Debug.Log("Getting trigger info failed!");
                }
                //Grip
                float gripValue = 0.0f;
#if USING_STEAMVR
            gripValue = s_gripState.GetAxis(_hand);
#else
                featureGrabbed = controllerDevice.TryGetFeatureValue(CommonUsages.grip, out gripValue);
#endif
                if (featureGrabbed)
                {
                    if (gripValue > 0.01f && _previousGripValue < 0.01f)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Press, (InputFeatureUsage)CommonUsages.grip, s_controllerNode, gripValue);
                        Debug.Log("Grip pressed: " + s_controllerNode.ToString());
                    }
                    if (gripValue < 0.01f && _previousGripValue > 0.01f)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Release, (InputFeatureUsage)CommonUsages.grip, s_controllerNode, gripValue);
                    }
                    _previousGripValue = gripValue;
                }
                else
                {
                    Debug.Log("Getting grip info failed!");
                }
#if USING_STEAMVR
#else
                //MenuButon, ONLY left
                bool menuButtonValue = false;
                featureGrabbed = false;
                if (controllerDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonValue) && s_controllerNode == XRNode.LeftHand)
                {
                    if (menuButtonValue && !_previousMenuButtonValue)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Press, (InputFeatureUsage)CommonUsages.menuButton, s_controllerNode, 1.0f);
                    }
                    if (menuButtonValue && !_previousMenuButtonValue)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Release, (InputFeatureUsage)CommonUsages.menuButton, s_controllerNode, 0.0f);
                    }
                    _previousMenuButtonValue = menuButtonValue;
                }
#endif
                //Primary Button
                bool primaryButtonValue = false;
                if (controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonValue))
                {
                    if (primaryButtonValue && !_previousPrimaryButton)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Press, (InputFeatureUsage)CommonUsages.primaryButton, s_controllerNode, 1.0f);
                        Debug.Log("Primary button Pressed");
                    }
                    if (primaryButtonValue && !_previousPrimaryButton)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Release, (InputFeatureUsage)CommonUsages.primaryButton, s_controllerNode, 0.0f);
                    }
                    _previousPrimaryButton = primaryButtonValue;
                }
                //Secondary Button
                bool secondaryButtonValue = false;
                if (controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonValue))
                {
                    if (secondaryButtonValue && !_previousSecondaryButton)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Press, (InputFeatureUsage)CommonUsages.secondaryButton, s_controllerNode, 1.0f);
                        Debug.Log("Secondary button Pressed");
                    }
                    if (secondaryButtonValue && !_previousSecondaryButton)
                    {
                        JAVR_ControllerInputEvents.InputDeviceButtonAction(JAVR_ControllerInputAction.Release, (InputFeatureUsage)CommonUsages.secondaryButton, s_controllerNode, 0.0f);
                    }
                    _previousSecondaryButton = secondaryButtonValue;
                }

            }
        }
        public void SetNode(XRNode node)
        {
            s_controllerNode = node;
        }
        public static float GetTriggerState(XRNode hand)
        {
            float value = 0.0f;

#if USING_STEAMVR
            SteamVR_Input_Sources svrHand = hand == XRNode.LeftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            value = SteamVR_Input.GetSingle("TriggerState", svrHand, false);
#else
            InputDevice controllerDevice = InputDevices.GetDeviceAtXRNode(hand);
            if (controllerDevice.isValid)
            {
                controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out value);
            }
#endif
            return value;
        }
        public static float GetGripState(XRNode hand)
        {
            float value = 0.0f;

#if USING_STEAMVR
            SteamVR_Input_Sources svrHand = hand == XRNode.LeftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            value = SteamVR_Input.GetSingle("GripState", svrHand, false);
#else
            InputDevice controllerDevice = InputDevices.GetDeviceAtXRNode(hand);
            if (controllerDevice.isValid)
            {
                controllerDevice.TryGetFeatureValue(CommonUsages.grip, out value);
            }
#endif
            return value;
        }
        public static Vector2 GetJoystickState(XRNode hand)
        {
            Vector2 value = Vector2.zero;

#if USING_STEAMVR
            SteamVR_Input_Sources svrHand = hand == XRNode.LeftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            value = SteamVR_Input.GetSingle("2dAxisState", svrHand, false);
#else
            InputDevice controllerDevice = InputDevices.GetDeviceAtXRNode(hand);
            if (controllerDevice.isValid)
            {
                controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out value);
            }
#endif
            return value;
        }
    }
}