//  Javier Alvarez Calleja
//  JAVR Toolkit
//  Laser pointer
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace JAVR
{
    [RequireComponent(typeof(LineRenderer)),]
    public class JAVR_LaserPointer : MonoBehaviour
    {
        Transform pointerDot;
        LineRenderer lineRenderer;
        [SerializeField]
        private float maxDistance = 100.0f;
        [SerializeField]
        private LayerMask globalLayerMask = new LayerMask();
        private JAVR_LaserPointableElement lastButtonHit;
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
            pointerDot = transform.GetChild(0);
            JAVR_ControllerInputEvents.OnInputDeviceButtonAction += OnButtonPressed;
        }
        private void OnDestroy()
        {
            JAVR_ControllerInputEvents.OnInputDeviceButtonAction -= OnButtonPressed;
        }
        private void OnDisable()
        {
            lineRenderer.enabled = false;
            pointerDot.gameObject.SetActive(false);
            JAVR_ControllerInputEvents.OnInputDeviceButtonAction -= OnButtonPressed;
        }
        private void OnEnable()
        {
            if (pointerDot == null) return;
            pointerDot.gameObject.SetActive(true);
            JAVR_ControllerInputEvents.OnInputDeviceButtonAction += OnButtonPressed;
        }
        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray();
            ray.direction = transform.forward;
            ray.origin = transform.position;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, globalLayerMask, QueryTriggerInteraction.Collide))
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
                JAVR_LaserPointableElement b = hit.collider.GetComponent<JAVR_LaserPointableElement>();

                if (b != null)
                {

                    if (lastButtonHit == null)
                    {
                        b.OnLaserPointerEnter();
                        selectSound.Play();
                        lastButtonHit = b;
                    }
                    else
                    {
                        if (lastButtonHit != b)
                        {
                            lastButtonHit.OnLaserPointerExit();
                            b.OnLaserPointerEnter();
                            selectSound.Play();
                            lastButtonHit = b;
                        }
                    }
                }
                else
                {
                    if (lastButtonHit != null)
                    {
                        lastButtonHit.OnLaserPointerExit();
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
                pointerDot.position = transform.position + (transform.forward * maxDistance) - (transform.forward * 0.01f);
                if (lastButtonHit != null)
                {
                    lastButtonHit.OnLaserPointerExit();
                    lastButtonHit = null;
                }
            }
        }
        private void OnButtonPressed(JAVR_ControllerInputAction action, InputFeatureUsage usage, XRNode node, float value)
        {
            if (lastButtonHit == null) return;
            if (usage == (InputFeatureUsage)CommonUsages.trigger && action == JAVR_ControllerInputAction.Press && node == this.node)
            {
                lastButtonHit.OnLaserPointerClick();
                confirmSound.Play();
            }
        }
    }
}
