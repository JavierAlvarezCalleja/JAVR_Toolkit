//  Javier Alvarez-Calleja
//  JAVR Framework 2020
//  Hand Pointer
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(LineRenderer)), ]
public class HandPointer : MonoBehaviour
{
    Transform pointerDot;
    LineRenderer lineRenderer;
    [SerializeField]
    private float maxDistance = 100.0f;
    [SerializeField]
    private LayerMask globalLayerMask = new LayerMask();
    [SerializeField]
    private LayerMask uiLayerMask     = new LayerMask();
    private HandPointableButton lastButtonHit;
    [SerializeField]
    private XRNode node;
    [SerializeField]
    private int pointerSegments = 12;
    [SerializeField]
    private AudioSource selectSound;
    [SerializeField]
    private AudioSource confirmSound;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        pointerDot = transform.GetChild( 0 );
        InputDeviceEvents.OnInputDeviceButtonAction += OnButtonPressed;
    }
    private void OnDestroy()
    {
        InputDeviceEvents.OnInputDeviceButtonAction -= OnButtonPressed;
    }
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray();
        ray.direction = transform.forward;
        ray.origin    = transform.position;

        RaycastHit hit;
        if(Physics.Raycast( ray, out hit, maxDistance, globalLayerMask, QueryTriggerInteraction.Collide ))
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = pointerSegments;
            for (int i = 0; i < pointerSegments; i++)
            {
                Vector3 pos = Vector3.Lerp(transform.position, hit.point - (transform.forward * 0.03f), (float)i / (float)pointerSegments);
                lineRenderer.SetPosition(i, pos);
            }
            pointerDot.position = hit.point - (transform.forward * 0.01f);
            pointerDot.forward = -hit.normal.normalized;
            HandPointableButton b = hit.collider.GetComponent<HandPointableButton>();

            if(b != null)
            {
                
                if(lastButtonHit == null)
                {
                    b.OnHandPointerEnter();
                    selectSound.Play();
                    lastButtonHit = b;
                }
                else
                {
                    if(lastButtonHit != b)
                    {
                        lastButtonHit.OnHandPointerExit();
                        b.OnHandPointerEnter();
                        selectSound.Play();
                        lastButtonHit = b;
                    }
                }
            }
            else
            {
                if(lastButtonHit != null)
                {
                    lastButtonHit.OnHandPointerExit();
                    lastButtonHit = null;
                }
            }
        }
        else
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = pointerSegments;
            for (int i = 0; i < pointerSegments; i++)
            {
                Vector3 pos = Vector3.Lerp(transform.position, hit.point - (transform.forward * 0.03f), (float)i / (float)pointerSegments);
                lineRenderer.SetPosition(i, pos);
            }
            pointerDot.position = transform.position + ( transform.forward * maxDistance ) - ( transform.forward * 0.01f );
            if(lastButtonHit != null)
            {
                lastButtonHit.OnHandPointerExit();
                lastButtonHit = null;
            }
        }

        //pointerDot.forward = (  pointerDot.transform.position - Camera.main.transform.position ).normalized;

    }
    private void OnButtonPressed(InputDeviceAction action, InputFeatureUsage usage, XRNode node, float value)
    {
        if(lastButtonHit != null)
        if(usage == (InputFeatureUsage)CommonUsages.trigger && action == InputDeviceAction.Press && node == this.node)
        {
            lastButtonHit.OnHandPointerClick();
            confirmSound.Play();
        }
    }
}
