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
    public class JAVR_LocomotionTeleport : MonoBehaviour
    {
        [SerializeField]
        private LayerMask s_teleportableSurfaces;
        [SerializeField]
        private Transform s_teleportPoint;
        [SerializeField]
        private GameObject s_teleportGizmo;
        //[SerializeField]
        //private OVRInput.Controller m_controller;
        private InputDevice _handController;
        [SerializeField]
        private XRNode s_handNode;
        private List<Transform> _waypoints;
        private int _state = 0;

        private float _dashingTime = 0.0f;
        private float _dashingSpeed = 75.0f;
        private float _dashingStart = 0.0f;

        private JAVR_LocomotionTeleport otherTeleport;

        [SerializeField]
        private Transform s_cameraRig;
        [SerializeField]
        private Transform s_headTransform;

        private Vector3 _originalPosition;
        private Vector3 _teleportPosition;
        private Vector3 _positionOffset;


        [SerializeField]
        private Transform s_highPointGizmo;
        [SerializeField]
        private Transform s_middlePointGizmo;
        [SerializeField]
        private Transform s_endPointGizmo;
        [SerializeField]
        private LineRenderer s_controllerLineGizmo;
        [SerializeField]
        private LineRenderer s_highLineGizmo;
        [SerializeField]
        private LineRenderer s_parabolicLineGizmo;
        [SerializeField]
        private int s_parabolicLineSegments = 12;
        [SerializeField]
        private bool s_showDebugGizmos = false;

        // private SnapZone previousSnapZone;
        [SerializeField]
        private List<Color> s_colors;
        [SerializeField]
        private Material s_parableMaterial;
        private bool _validContact = false;
        // Start is called before the first frame update
        void Start()
        {
            JAVR_LocomotionTeleport[] teleports = FindObjectsOfType<JAVR_LocomotionTeleport>();

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
            _validContact = false;
            _handController = InputDevices.GetDeviceAtXRNode(s_handNode);
            Vector2 stickInput = JAVR_ControllerInput.GetJoystickState(s_handNode); 
            switch (_state)
            {
                //Controller joystick is idle, nothing happens.
                case 0:

                    s_teleportGizmo.SetActive(false);


                    if (stickInput.y > 0.5f)
                    {

                        _state = 1;

                          if (!otherTeleport.TeleportSwap())
                          {
                              _state = 2;
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
                    if (Physics.Raycast(ray, out hit, 10.0f, s_teleportableSurfaces))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) >= 0.9f)
                        {
                            _validContact = true;
                            s_teleportGizmo.SetActive(true);
                            s_parabolicLineGizmo.enabled = true;
                            s_teleportPoint.position = hit.point + (Vector3.up * 0.05f);
                            //teleportPoint.forward = hit.normal * -1.0f;
                            s_teleportPoint.eulerAngles += Vector3.forward * (Time.fixedDeltaTime) * 360.0f * 0.25f;
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
                                _state = 3;
                                _dashingStart = Time.time;
                                _originalPosition = s_cameraRig.position;
                                _positionOffset = (new Vector3(s_headTransform.position.x, 0.0f, s_headTransform.position.z) - new Vector3(s_cameraRig.position.x, 0.0f, s_cameraRig.position.z));
                                _dashingTime = (_originalPosition - (_teleportPosition + _positionOffset)).magnitude / _dashingSpeed;
                                if (_dashingTime > 0.1f) _dashingTime = 0.1f;
                                _dashingTime = 0.1f;
                                EnableDebugGizmos(false);
                                /*if (sz != null)
                                {
                                    teleportPosition = sz.transform.position;
                                }
                                else
                                {
                                    teleportPosition = hit.point;
                                }*/
                                _teleportPosition = hit.point;
                                break;
                            }
                        }
                        else
                        {
                            s_teleportGizmo.SetActive(false);
                            DrawParable();
                            // parabolicLineGizmo.enabled = false;
                            //  
                        }
                    }
                    else
                    {
                        s_teleportGizmo.SetActive(false);
                        DrawParable();
                        //   parabolicLineGizmo.enabled = false;
                    }
                    if (stickInput.y <= 0.25f)
                    {
                        EnableDebugGizmos(false);
                        _state = 0;
                    }
                    break;
                //if the other teleporter gets activated we waut till joystick is reset.
                case 2:

                    s_teleportGizmo.SetActive(false);
                    EnableDebugGizmos(false);
                    if (stickInput.y <= 0.25f)
                    {
                        _state = 0;
                    }

                    break;
                //Dashing the camera rig
                case 3:

                    s_teleportGizmo.SetActive(false);
                    EnableDebugGizmos(false);
                    if (Time.time - _dashingStart <= _dashingTime)
                    {

                        s_cameraRig.position = Vector3.Lerp(_originalPosition, _teleportPosition - _positionOffset, (Time.time - _dashingStart) / _dashingTime);
                    }
                    else
                    {
                        s_cameraRig.position = _teleportPosition - _positionOffset;
                        _state = 0;
                    }
                    break;
            }
        }
        public bool TeleportSwap()
        {
            if (_state != 3)
            {
                _state = 2;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void EnableDebugGizmos(bool b)
        {
            s_highPointGizmo.gameObject.SetActive(b && s_showDebugGizmos);
            s_middlePointGizmo.gameObject.SetActive(b && s_showDebugGizmos);
            s_endPointGizmo.gameObject.SetActive(b && s_showDebugGizmos);
            s_highLineGizmo.gameObject.SetActive(b && s_showDebugGizmos);
            s_controllerLineGizmo.gameObject.SetActive(b && s_showDebugGizmos);
            s_parabolicLineGizmo.gameObject.SetActive(b);

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
            Vector3 highPoint = new Vector3(s_headTransform.position.x, s_headTransform.position.y + 2.0f, s_headTransform.position.z);
            Vector3 direction = (middlePoint - highPoint).normalized;

            ray.direction = direction;
            ray.origin = middlePoint - (direction * 2.0f);

            //Debug gizmos
            s_highPointGizmo.position = highPoint;
            s_middlePointGizmo.position = middlePoint;
            s_endPointGizmo.position = middlePoint + (direction * 2.0f);

            s_highLineGizmo.SetPosition(0, highPoint);
            s_highLineGizmo.SetPosition(1, highPoint + (direction * 20.0f));

            s_controllerLineGizmo.SetPosition(0, transform.position);
            s_controllerLineGizmo.SetPosition(1, middlePoint);



            return ray;
        }
        private void DrawParable()
        {
            Vector3 end = _validContact ? s_teleportGizmo.transform.position : s_endPointGizmo.position;
            Vector3 controllerToMiddlePoint = /*transform.position -*/ s_middlePointGizmo.position - transform.position;
            Vector3 middlePointToEnd = end - s_middlePointGizmo.position;
            Vector3 parableSection = Vector3.one;
            s_parabolicLineGizmo.positionCount = s_parabolicLineSegments + 1;
            s_parabolicLineGizmo.SetPosition(0, transform.position);
            // parableMaterial.SetColor("_Color", colors[validContact ? 0 : 1]);
            //  parabolicLineGizmo.material.SetColor("_Color", colors[validContact ? 0 : 1]);
            s_parabolicLineGizmo.startColor = s_colors[_validContact ? 0 : 1];
            s_parabolicLineGizmo.endColor = s_colors[_validContact ? 0 : 1];
            //   Debug.Log("Teloeport valid contact: " + validContact);
            //Calculate parabolic Points.
            for (int i = 1; i < s_parabolicLineSegments; i++)
            {
                //Debug.Log("1");
                float f = ((float)i / (float)s_parabolicLineSegments);
                //Debug.Log("2");
                parableSection = (s_middlePointGizmo.position + (middlePointToEnd * f)) - (transform.position + (controllerToMiddlePoint * f));
                //Debug.Log("3");
                Vector3 parablePoint = (transform.position + (controllerToMiddlePoint * f)) + (parableSection * f);
                //Debug.Log("4");
                s_parabolicLineGizmo.SetPosition(i, parablePoint);
            }
            //Debug.Log("5");
            s_parabolicLineGizmo.SetPosition(s_parabolicLineSegments, end);
        }
        public void Activate(bool b = true)
        {
            if (b)
            {
                _state = 0;
            }
            else
            {
                EnableDebugGizmos(false);
                s_teleportGizmo.SetActive(false);
                _state = 100;
            }
        }
    }
}