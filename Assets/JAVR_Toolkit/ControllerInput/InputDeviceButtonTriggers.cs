//  Javier Alvarez-Calleja
//  JAVR Framework 2020
//  Input Device Button Triggers (Universal)
//
//  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputDeviceButtonTriggers : MonoBehaviour
{
    float previousTriggerValue    = 0.0f;
    float previousGripValue       = 0.0f;
    bool previousMenuButtonValue  = false;
    bool previousPrimaryButton    = false;
    bool previousSecondaryButton = false;

    [SerializeField]
    private XRNode controllerNode;

    void Update()
    {
        InputDevice controllerDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
       // Debug.Log("Input device: " + controllerDevice.ToString());
        if (controllerDevice.isValid)
        {
         //   Debug.Log("Input device valid");
            //Trigger
            float triggerValue = 0.0f;
            if (controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
            {
               // Debug.Log("Trigger value: " + triggerValue);
                if (triggerValue > 0.01f && previousTriggerValue < 0.01f)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Press, (InputFeatureUsage)CommonUsages.trigger, controllerNode, triggerValue);
                    Debug.Log("Trigger pressed: " + controllerNode.ToString());
                }
                if (triggerValue < 0.001f && previousTriggerValue > 0.001f)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Release, (InputFeatureUsage)CommonUsages.trigger, controllerNode, triggerValue);
                }
                previousTriggerValue = triggerValue;
            }
            else
            {
                Debug.Log("Getting trigger info failed!");
            }
            //Grip
            float gripValue = 0.0f;
            if (controllerDevice.TryGetFeatureValue(CommonUsages.grip, out gripValue))
            { 
                if (gripValue > 0.01f && previousGripValue < 0.01f)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Press, (InputFeatureUsage)CommonUsages.grip, controllerNode, gripValue);
                }
                if (gripValue < 0.01f && previousGripValue > 0.01f)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Release, (InputFeatureUsage)CommonUsages.grip, controllerNode, gripValue);
                }
                previousGripValue = gripValue;
            }
            else 
            {
                Debug.Log("Getting grip info failed!");
            }

            //MenuButon, ONLY left
            bool menuButtonValue = false;
            if(controllerDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonValue) && controllerNode == XRNode.LeftHand)
            {
                if (menuButtonValue && !previousMenuButtonValue)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Press, (InputFeatureUsage)CommonUsages.menuButton, controllerNode, 1.0f);
                }
                if (menuButtonValue && !previousMenuButtonValue)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Release, (InputFeatureUsage)CommonUsages.menuButton, controllerNode, 0.0f);
                }
                previousMenuButtonValue = menuButtonValue;
            }
            //Primary Button
            bool primaryButtonValue = false;
            if (controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonValue))
            {
                if (primaryButtonValue && !previousPrimaryButton)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Press, (InputFeatureUsage)CommonUsages.primaryButton, controllerNode, 1.0f);
                    Debug.Log("Primary button Pressed");
                }
                if (primaryButtonValue && !previousPrimaryButton)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Release, (InputFeatureUsage)CommonUsages.primaryButton, controllerNode, 0.0f);
                }
                previousPrimaryButton = primaryButtonValue;
            }
            //Secondary Button
            bool secondaryButtonValue = false;
            if (controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonValue))
            {
                if (secondaryButtonValue && !previousSecondaryButton)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Press, (InputFeatureUsage)CommonUsages.secondaryButton, controllerNode, 1.0f);
                    Debug.Log("Secondary button Pressed");
                }
                if (secondaryButtonValue && !previousSecondaryButton)
                {
                    InputDeviceEvents.InputDeviceButtonAction(InputDeviceAction.Release, (InputFeatureUsage)CommonUsages.secondaryButton, controllerNode, 0.0f);
                }
                previousSecondaryButton = secondaryButtonValue;
            }

        }
    }
    public void SetNode(XRNode node) 
    {
        controllerNode = node;
    }
}
