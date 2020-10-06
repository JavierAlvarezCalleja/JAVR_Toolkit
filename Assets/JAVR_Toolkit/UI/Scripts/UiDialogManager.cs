//  Javier Alvarez-Calleja
//  JAVR Framework 2020
//  VR Menu Manager
//
//  Manages a UI canvas to behave as a VR UI Dialog
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class UiDialogManager : MonoBehaviour
{
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private float offset = 1.150f;

    private float distance = 1.5f;
    private bool active = false;
    [SerializeField]
    private List<GameObject> objectsToShut;
    [SerializeField]
    private List<GameObject> objectsToOpen;
    private Vector3 previousFlatForward;
    private Vector3 previousPosition;
    
    void Start()
    {
        InputDeviceEvents.OnInputDeviceButtonAction += OnButtonPressed;
        ShutMenu();
    }
    private void OnDestroy()
    {
        InputDeviceEvents.OnInputDeviceButtonAction -= OnButtonPressed;
    }
    private void Update()
    {
        if (active)
        {
            transform.localPosition = Vector3.Lerp(previousPosition, new Vector3(headTransform.localPosition.x, offset, headTransform.localPosition.z), 0.005f);
            previousPosition = transform.localPosition;
            Vector3 flatForward = new Vector3(headTransform.forward.x, 0.0f, headTransform.forward.z).normalized;
            //flatForward = Vector3.Lerp(previousFlatForward, flatForward, 0.005f);
            float angle = Mathf.Abs(Vector3.SignedAngle(previousFlatForward, flatForward, Vector3.up));
            if (angle >= 20.0f)
            {
                transform.forward = Vector3.Lerp(previousFlatForward, flatForward, 0.005f);
                previousFlatForward = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
            }
        }
    }
    private void OnButtonPressed(InputDeviceAction action, InputFeatureUsage usage, XRNode node, float value)
    {
        if (usage == (InputFeatureUsage)CommonUsages.menuButton && action == InputDeviceAction.Press && node == XRNode.LeftHand)
        {
            active = !active;
            ShutOpenMenu(active);
            Vector3 flatForward = new Vector3(headTransform.forward.x, 0.0f, headTransform.forward.z).normalized;
            transform.localPosition = new Vector3(headTransform.localPosition.x, offset, headTransform.localPosition.z);
            transform.forward = flatForward;
            previousFlatForward = flatForward;
            previousPosition = transform.localPosition;
        }
    }
    public void ShutMenu()
    {
        active = false;
        ShutOpenMenu(false);
    }
    public bool IsMenuActive()
    {
        return active;
    }
    private void ShutOpenMenu(bool open)
    {
        menuCanvas.SetActive(open);
        for(int i = 0; i < objectsToOpen.Count; i ++)
        {
            objectsToOpen[i].SetActive(open);
        }
        for (int i = 0; i < objectsToShut.Count; i++)
        {
            objectsToShut[i].SetActive(!open);
        }
    }
}
