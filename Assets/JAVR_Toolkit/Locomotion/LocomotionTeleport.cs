//  Javier Alvarez-Calleja
//  JAVR Toolkit 2020
//  Locomotion Teleport
//
//  Performs joystick based teleport

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace JAVR
{
    public class LocomotionTeleport : MonoBehaviour
    {
        [SerializeField]
        private LayerMask teleportableSurfaces;
        [SerializeField]
        private Transform teleportPoint;
        [SerializeField]
        private GameObject teleportGizmo;
        //[SerializeField]
        //private OVRInput.Controller m_controller;
        private InputDevice handController;
        [SerializeField]
        private XRNode handNode;
        private List<Transform> waypoints;
        private int state = 0;

        private float dashingTime = 0.0f;
        private float dashingSpeed = 75.0f;
        private float dashingStart = 0.0f;

        private LocomotionTeleport otherTeleport;

        [SerializeField]
        private Transform cameraRig;
        [SerializeField]
        private Transform headTransform;

        private Vector3 originalPosition;
        private Vector3 teleportPosition;
        private Vector3 positionOffset;


        [SerializeField]
        private Transform highPointGizmo;
        [SerializeField]
        private Transform middlePointGizmo;
        [SerializeField]
        private Transform endPointGizmo;
        [SerializeField]
        private LineRenderer controllerLineGizmo;
        [SerializeField]
        private LineRenderer highLineGizmo;
        [SerializeField]
        private LineRenderer parabolicLineGizmo;
        [SerializeField]
        private int parabolicLineSegments = 12;
        [SerializeField]
        private bool showDebugGizmos = false;

        // private SnapZone previousSnapZone;
        [SerializeField]
        private List<Color> colors;
        [SerializeField]
        private Material parableMaterial;
        private bool validContact = false;
        // Start is called before the first frame update
        void Start()
        {
            LocomotionTeleport[] teleports = FindObjectsOfType<LocomotionTeleport>();

            for (int i = 0; i < teleports.Length; i++)
            {
                if (teleports[i] != this)
                {
                    otherTeleport = teleports[i];
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            validContact = false;
            handController = InputDevices.GetDeviceAtXRNode(handNode);
            Vector2 stickInput; 
            if (handController.TryGetFeatureValue(CommonUsages.primary2DAxis, out stickInput))
            switch (state)
            {
                //Controller joystick is idle, nothing happens.
                case 0:

                    teleportGizmo.SetActive(false);


                    if (stickInput.y > 0.5f)
                    {

                        state = 1;

                          if (!otherTeleport.TeleportSwap())
                          {
                              state = 2;
                          }
                    }

                    break;
                //If the raycast hits a teleportable surface, the teleport point gizmo will apear at the contact point.
                case 1:

                    RaycastHit hit;
                    Ray ray = CalculateParabolicRay();// new Ray();

                    //ray.direction = transform.forward;
                    //ray.origin = transform.position;
                    EnableDebugGizmos(true);
                    if (Physics.Raycast(ray, out hit, 10.0f, teleportableSurfaces))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) >= 0.9f)
                        {
                            validContact = true;
                            teleportGizmo.SetActive(true);
                            parabolicLineGizmo.enabled = true;
                            teleportPoint.position = hit.point + (Vector3.up * 0.05f);
                            //teleportPoint.forward = hit.normal * -1.0f;
                            teleportPoint.eulerAngles += Vector3.forward * (Time.fixedDeltaTime) * 360.0f * 0.25f;
                            DrawParable();
                           /* SnapZone sz = hit.collider.GetComponent<SnapZone>();
                            if (sz != null)
                            {
                                if (!sz.activated)
                                {
                                    sz.Activate(true);
                                }
                                previousSnapZone = sz;
                            }
                            else
                            {
                                if (previousSnapZone != null)
                                {
                                    previousSnapZone.Activate(false);
                                    previousSnapZone = null;
                                }
                            }*/

                            if (stickInput.y <= 0.25f)
                            {
                                state = 3;
                                dashingStart = Time.time;
                                originalPosition = cameraRig.position;
                                positionOffset = (new Vector3(headTransform.position.x, 0.0f, headTransform.position.z) - new Vector3(cameraRig.position.x, 0.0f, cameraRig.position.z));
                                dashingTime = (originalPosition - (teleportPosition + positionOffset)).magnitude / dashingSpeed;
                                if (dashingTime > 0.1f) dashingTime = 0.1f;
                                dashingTime = 0.1f;
                                EnableDebugGizmos(false);
                                /*if (sz != null)
                                {
                                    teleportPosition = sz.transform.position;
                                }
                                else
                                {
                                    teleportPosition = hit.point;
                                }*/
                                teleportPosition = hit.point;
                                break;
                            }
                        }
                        else
                        {
                            teleportGizmo.SetActive(false);
                            DrawParable();
                            // parabolicLineGizmo.enabled = false;
                            //  
                        }
                    }
                    else
                    {
                        teleportGizmo.SetActive(false);
                        DrawParable();
                        //   parabolicLineGizmo.enabled = false;
                    }
                    if (stickInput.y <= 0.25f)
                    {
                        EnableDebugGizmos(false);
                        state = 0;
                    }
                    break;
                //if the other teleporter gets activated we waut till joystick is reset.
                case 2:

                    teleportGizmo.SetActive(false);
                    EnableDebugGizmos(false);
                    if (stickInput.y <= 0.25f)
                    {
                        state = 0;
                    }

                    break;
                //Dashing the camera rig
                case 3:

                    teleportGizmo.SetActive(false);
                    EnableDebugGizmos(false);
                    if (Time.time - dashingStart <= dashingTime)
                    {

                        cameraRig.position = Vector3.Lerp(originalPosition, teleportPosition - positionOffset, (Time.time - dashingStart) / dashingTime);
                    }
                    else
                    {
                        cameraRig.position = teleportPosition - positionOffset;
                        state = 0;
                    }
                    break;
            }
        }
        public bool TeleportSwap()
        {
            if (state != 3)
            {
                state = 2;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void EnableDebugGizmos(bool b)
        {
            highPointGizmo.gameObject.SetActive(b && showDebugGizmos);
            middlePointGizmo.gameObject.SetActive(b && showDebugGizmos);
            endPointGizmo.gameObject.SetActive(b && showDebugGizmos);
            highLineGizmo.gameObject.SetActive(b && showDebugGizmos);
            controllerLineGizmo.gameObject.SetActive(b && showDebugGizmos);
            parabolicLineGizmo.gameObject.SetActive(b);

        }
        private Ray CalculateParabolicRay()
        {
            Ray ray = new Ray();

            float controllerAngle = Vector3.Angle(Vector3.up * -1.0f, transform.forward);
            if (controllerAngle > 90.0f)
            {
                controllerAngle = 180.0f - controllerAngle;

            }
            float pitch = Mathf.Clamp(controllerAngle, 20.0f, 90.0f);
            float pitchRange = 90.0f - (20.0f);
            float t = (pitch) / (pitchRange); // Normalized pitch within range
            float distance = t * 4.0f;

            Vector3 frontVector = transform.forward - new Vector3(0.0f, transform.forward.y, 0.0f);

            Vector3 middlePoint = (Vector3.up * 0.25f) + transform.position + (frontVector * distance);
            //float height = middlePoint.y - cameraRig.position.y; 
            Vector3 highPoint = new Vector3(headTransform.position.x, headTransform.position.y + 2.0f, headTransform.position.z);
            Vector3 direction = (middlePoint - highPoint).normalized;

            ray.direction = direction;
            ray.origin = middlePoint - (direction * 2.0f);

            //Debug gizmos
            highPointGizmo.position = highPoint;
            middlePointGizmo.position = middlePoint;
            endPointGizmo.position = middlePoint + (direction * 2.0f);

            highLineGizmo.SetPosition(0, highPoint);
            highLineGizmo.SetPosition(1, highPoint + (direction * 20.0f));

            controllerLineGizmo.SetPosition(0, transform.position);
            controllerLineGizmo.SetPosition(1, middlePoint);



            return ray;
        }
        private void DrawParable()
        {
            Vector3 end = validContact ? teleportGizmo.transform.position : endPointGizmo.position;
            Vector3 controllerToMiddlePoint = /*transform.position -*/ middlePointGizmo.position - transform.position;
            Vector3 middlePointToEnd = end - middlePointGizmo.position;
            Vector3 parableSection = Vector3.one;
            parabolicLineGizmo.positionCount = parabolicLineSegments + 1;
            parabolicLineGizmo.SetPosition(0, transform.position);
            // parableMaterial.SetColor("_Color", colors[validContact ? 0 : 1]);
            //  parabolicLineGizmo.material.SetColor("_Color", colors[validContact ? 0 : 1]);
            parabolicLineGizmo.startColor = colors[validContact ? 0 : 1];
            parabolicLineGizmo.endColor = colors[validContact ? 0 : 1];
            //   Debug.Log("Teloeport valid contact: " + validContact);
            //Calculate parabolic Points.
            for (int i = 1; i < parabolicLineSegments; i++)
            {
                //Debug.Log("1");
                float f = ((float)i / (float)parabolicLineSegments);
                //Debug.Log("2");
                parableSection = (middlePointGizmo.position + (middlePointToEnd * f)) - (transform.position + (controllerToMiddlePoint * f));
                //Debug.Log("3");
                Vector3 parablePoint = (transform.position + (controllerToMiddlePoint * f)) + (parableSection * f);
                //Debug.Log("4");
                parabolicLineGizmo.SetPosition(i, parablePoint);
            }
            //Debug.Log("5");
            parabolicLineGizmo.SetPosition(parabolicLineSegments, end);
        }
        public void Activate(bool b = true)
        {
            if (b)
            {
                state = 0;
            }
            else
            {
                EnableDebugGizmos(false);
                teleportGizmo.SetActive(false);
                state = 100;
            }
        }
    }
}