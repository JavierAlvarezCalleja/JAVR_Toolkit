//  Javier Alvarez Calleja
//  JAVR Toolkit
//  Locomotion walk
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
//using TMPro;

namespace JAVR
{
    public class JAVR_LocomotionWalk : MonoBehaviour
    {
        private CharacterController _userController;
        Vector3 _userControllerVelocity = Vector3.zero;
        //private Rigidbody _userBody;
        private Transform _headTransform;
        private Transform _lHandTransform;
        private Transform _rHandTransform;

        private const float HEAD_OFFSET = 0.1f;
        private const float TOP_OFFSET = 2.25f;
        private const float BODY_OFFSET = 0.0f;
        private const float MAX_SPEED = 20.0f;
        private const float MAX_WALKING_SPEED = 3.0f;
        private const float ANGLE_TURN = 20.0f;
        private const float ANGLE_TURN_TIME = 0.1f;
        private const float ANGLE_WAIT_TIME = 0.05f;

        private Vector3 _previousLeftPosition;
        private Vector3 _previousRightPosition;
        private Vector3 _previousHeadPosition;

        private int _rotationJoystickState = 0;
        private float _rotationJoystickTimer = 0.0f;
        private int _isRotationRight = 0;

        [SerializeField]
        private LayerMask _groundContactMask;

        private InputDevice _walkControllerDevice;
        private InputDevice _turnControllerDevice;

        

        // [SerializeField]
        // private TextMeshProUGUI _leftText;
        // [SerializeField]
        // private TextMeshProUGUI _rightText;
        [Header("Debug lines")]
        [SerializeField]
        private LineRenderer s_slopeForward;
        [SerializeField]
        private LineRenderer s_slopeNormal;
        [SerializeField]
        private LineRenderer s_slopeCross;
        [SerializeField]
        private LineRenderer s_slopeRay;
        // Start is called before the first frame update
        void Start()
        {
            _headTransform = GameObject.Find("Main Camera").transform;
            //_lHandTransform = GameObject.Find("LeftHand").transform;
            //_rHandTransform = GameObject.Find("RightHand").transform;
            _userController = GetComponent<CharacterController>();
            //_userBody = GetComponent<Rigidbody>();
            Time.fixedDeltaTime = 1.0f / XRDevice.refreshRate;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // if(leftControllerDevice == null)
            _walkControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            //  if (rightControllerDevice == null)
            _turnControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            GameObject g;
            if (_lHandTransform == null)
            {
                g = GameObject.Find("LeftHand");
                if (g != null)
                    _lHandTransform = g.transform;
            }
            if (_rHandTransform == null)
            {
                g = GameObject.Find("RightHand");
                if (g != null)
                    _rHandTransform = g.transform;
            }
            if (!(_rHandTransform == null) && !(_lHandTransform == null))
            {
                SetColliderPosition();
                //AddRocketForce();
                //AddControllerRotation();
                AddControllerRotationJoystick();
                AddWalkingForce();
            }
        }
        private bool DetectGround(out Vector3 hitNormal, out Vector3 hitPoint)
        {
            Ray ray = new Ray();
            ray.direction = Vector3.down;
            ray.origin = new Vector3(_headTransform.position.x, transform.position.y + 0.25f, _headTransform.position.z);
            RaycastHit hit;
            bool contact = Physics.Raycast(ray, out hit, BODY_OFFSET + 0.26f, _groundContactMask);
            hitNormal = hit.normal;
            hitPoint = hit.point;
            return contact;
        }
        private bool DetectSlope(Vector3 forwardDirection, out Vector3 hitNormal)
        {
            float radius = 0.275f;
            RaycastHit result = new RaycastHit();
            Vector3.Normalize(forwardDirection);
            Vector3 basePoint = new Vector3(_headTransform.position.x, transform.position.y, _headTransform.position.z);
            Vector3 headFlatForward = new Vector3(_headTransform.forward.x, 0.0f, _headTransform.forward.z).normalized;
            float forwardAngle = Vector3.SignedAngle(Vector3.forward, forwardDirection, Vector3.up);
            Ray ray = new Ray();
            ray.direction = Vector3.down;
            ray.origin = basePoint + (Vector3.up * 0.5f);
            ray.origin += (Quaternion.Euler(0.0f, forwardAngle, 0.0f) * headFlatForward) * radius;
            bool b = Physics.Raycast(ray, out result, 0.75f, _groundContactMask);
            Vector3 slope = (result.point - basePoint);
            hitNormal = Quaternion.AngleAxis(90.0f, Vector3.Cross(slope, Vector3.up).normalized) * slope;
            s_slopeForward.positionCount = 2;
            s_slopeForward.SetPosition(0, basePoint);
            s_slopeForward.SetPosition(1, result.point);
            s_slopeNormal.positionCount = 2;
            s_slopeNormal.SetPosition(0, basePoint);
            s_slopeNormal.SetPosition(1, basePoint + hitNormal);
            s_slopeCross.positionCount = 2;
            s_slopeCross.SetPosition(0, basePoint);
            s_slopeCross.SetPosition(1, basePoint + Vector3.Cross(slope, Vector3.up).normalized);
            s_slopeRay.positionCount = 2;
            s_slopeRay.SetPosition(0, ray.origin);
            s_slopeRay.SetPosition(1, ray.origin + (ray.direction * 0.75f));

            return b;
        }
        private void SetColliderPosition()
        {
            //Body collider pose
            //_userCollider.center = Vector3.zero;
            _userController.height = _headTransform.localPosition.y + HEAD_OFFSET;
            _userController.center = new Vector3(_headTransform.localPosition.x, _userController.height * 0.5f, _headTransform.localPosition.z);
        }
        /*
        private void AddControllerRotation()
        {
            //Hands pose angle tracking
            //Left hand
            bool leftHandTrigger;
            _walkControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out leftHandTrigger);
            if (leftHandTrigger)
            {
                _previousLeftPosition = _lHandTransform.localPosition;
            }
            Vector3 currentLeftVector = _lHandTransform.localPosition - _headTransform.localPosition;
            Vector3 previousLeftVector = _previousLeftPosition - _headTransform.localPosition;
            previousLeftVector.y = 0.0f;
            currentLeftVector.y = 0.0f;
            float leftHandAngle = Vector3.SignedAngle(previousLeftVector, currentLeftVector, Vector3.up);
            _previousLeftPosition = _lHandTransform.localPosition;
            //Right hand
            bool rightHandTrigger;
            _turnControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out rightHandTrigger);
            if (rightHandTrigger)
                _previousRightPosition = _rHandTransform.localPosition;
            Vector3 currentRightVector = _rHandTransform.localPosition - _headTransform.localPosition;
            Vector3 previousRightVector = _previousRightPosition - _headTransform.localPosition;
            previousRightVector.y = 0.0f;
            currentRightVector.y = 0.0f;
            float rightHandAngle = Vector3.SignedAngle(previousRightVector, currentRightVector, Vector3.up);
            _previousRightPosition = _rHandTransform.localPosition;

            float rGripInput;
            _turnControllerDevice.TryGetFeatureValue(CommonUsages.grip, out rGripInput);
            float lGripInput;
            _walkControllerDevice.TryGetFeatureValue(CommonUsages.grip, out lGripInput);

            //Apply body rotation based on input
            if (rGripInput > 0.25f)// && rightHandAngle > 0.005f)
            {
                //_rightText.text = rightHandAngle.ToString("00.000 " + _previousLeftPosition + " / " + _lHandTransform.localPosition + " - " + _headTransform.localPosition + " - " );
                _previousHeadPosition = _headTransform.position;
                transform.Rotate(new Vector3(0.0f, -rightHandAngle * 1.0f, 0.0f));
                // _userBody.MoveRotation(_userBody.transform.rotation *  Quaternion.Euler(new Vector3(0.0f, -rightHandAngle * 0.5f, 0.0f)));
                // transform.Rotate(new Vector3(0.0f, -rightHandAngle * 0.1f, 0.0f));// += new Vector3(0.0f, leftHandAngle, 0.0f);
                _userBody.MovePosition(_userBody.position - (_headTransform.position - _previousHeadPosition));
            }
            //Apply body rotation based on input
            if (lGripInput > 0.25f)// && leftHandAngle > 0.005f)
            {
                // _leftText.text = leftHandAngle.ToString("00.000" + _previousRightPosition + " / " + _rHandTransform.localPosition + " - " + _headTransform.localPosition + " - ");
                _previousHeadPosition = _headTransform.position;
                //_userBody.MoveRotation( Quaternion.Euler(new Vector3(0.0f, rightHandAngle, 0.0f)));
                transform.Rotate(new Vector3(0.0f, -leftHandAngle * 1.0f, 0.0f));// + transform.rotation.eulerAngles);
                _userBody.MovePosition(_userBody.position - (_headTransform.position - _previousHeadPosition));
            }
        }*/
        private void AddControllerRotationJoystick()
        {
            //Hands pose angle tracking
            Vector2 stickInput = JAVR_ControllerInput.GetJoystickState(XRNode.RightHand);
            //Apply body rotation based on input
            /*  if (stickInput > Mathf.Abs(0.65f))// && rightHandAngle > 0.005f)
              {
                  _previousHeadPosition = _headTransform.position;
                  transform.Rotate(new Vector3(0.0f, stickInput * 0.5f, 0.0f));
                  _userBody.MovePosition(_userBody.position - (_headTransform.position - _previousHeadPosition));
              }*/
            switch (_rotationJoystickState)
            {
                case 0:

                    if (Mathf.Abs(stickInput.x) > 0.65f)
                    {
                        _rotationJoystickState = 1;
                        _rotationJoystickTimer = Time.time;
                        _isRotationRight = stickInput.x > 0 ? 1 : -1;
                    }
                    break;
                case 1:

                    if (Time.time - _rotationJoystickTimer <= ANGLE_TURN_TIME)
                    {
                        float f = (ANGLE_TURN / ANGLE_TURN_TIME) * Time.deltaTime * _isRotationRight;
                        Vector3 previousheadPoint = new Vector3(_headTransform.position.x, transform.position.y, _headTransform.position.z);
                        transform.RotateAround(previousheadPoint, Vector3.up, f);
                        //Vector3 currentheadPoint = new Vector3(_headTransform.position.x, transform.position.y, _headTransform.position.z);

                        // transform.position += previousheadPoint - currentheadPoint;
                    }
                    else
                    {
                        _rotationJoystickState = 2;
                        _rotationJoystickTimer = Time.time;
                    }
                    break;
                case 2:
                    if (Time.time - _rotationJoystickTimer >= ANGLE_WAIT_TIME)
                    {
                        _rotationJoystickState = 0;
                    }
                    break;
            }

        }
        /*
        private void AddRocketForce()
        {
            //Get triggers input
            float rTriggerInput;
            _turnControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out rTriggerInput);
            float lTriggerInput;
            _walkControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out lTriggerInput);

            //Apply rocket thrurst based on input
            if (rTriggerInput > 0.05f || lTriggerInput > 0.05f)
            {
                _userBody.AddForce((rTriggerInput * _rHandTransform.up * 4.0f) + (lTriggerInput * _lHandTransform.up * 4.0f));
            }
            if (_userBody.velocity.magnitude > MAX_SPEED)
            {
                _userBody.velocity = _userBody.velocity.normalized * MAX_SPEED;
            }
        }*/
        private void AddWalkingForce()
        {

            Vector2 walkStrafe = JAVR_ControllerInput.GetJoystickState(XRNode.LeftHand);
            walkStrafe.y = walkStrafe.y < 0.25f && walkStrafe.y > -0.15f ? 0.0f : walkStrafe.y;
            walkStrafe.x = walkStrafe.x < 0.25f && walkStrafe.x > -0.15f ? 0.0f : walkStrafe.x;

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.W))
                walkStrafe.y += 1.0f;
            if (Input.GetKey(KeyCode.S))
                walkStrafe.y -= 1.0f;
            if (Input.GetKey(KeyCode.D))
                walkStrafe.x += 1.0f;
            if (Input.GetKey(KeyCode.S))
                walkStrafe.x -= 1.0f;
#endif
            Vector3 hitNormal;
            Vector3 hitPoint;
            bool contact =  _userController.isGrounded;//DetectGround(out hitNormal, out hitPoint);
            bool slopeContact = DetectSlope(new Vector3(walkStrafe.x, 0.0f, walkStrafe.y), out hitNormal);
            hitNormal = Vector3.Dot(hitNormal, Vector3.up) >= 0.95f ? Vector3.up : hitNormal;
            Vector3 forwardVector = Vector3.ProjectOnPlane(new Vector3(_headTransform.forward.x, 0.0f, _headTransform.forward.z), hitNormal).normalized;
            Vector3 rightVector = Vector3.ProjectOnPlane(new Vector3(_headTransform.right.x, 0.0f, _headTransform.right.z), hitNormal).normalized;
            Vector3 walkingForce = forwardVector * walkStrafe.y * 2.0f +
                                   rightVector * walkStrafe.x * 2.0f;


            if (contact || slopeContact)
            {
        //        if (slopeContact)
         //           walkingForce += Vector3.up * walkingForce.y;

                _userControllerVelocity = new  Vector3(_userControllerVelocity.x, 0.0f, _userControllerVelocity.z);
                _userControllerVelocity = walkingForce;
               
            }
            else
            {
                _userControllerVelocity += (Physics.gravity * Time.deltaTime);
                _userControllerVelocity -= _userControllerVelocity * ((0.004f) * Mathf.Pow(_userControllerVelocity.magnitude, 2.0f) * Time.deltaTime);

            }
            _userController.Move(_userControllerVelocity * Time.deltaTime);
        }
    }
}
