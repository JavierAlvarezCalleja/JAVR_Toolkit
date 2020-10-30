//  Javier Alvarez Calleja
//  JAVR Toolkit
//  Laser Pointable Button
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JAVR
{
    [RequireComponent(typeof(Collider)), RequireComponent(typeof(Button))]
    public class JAVR_LaserPointableButton : JAVR_LaserPointableElement
    {
        PointerEventData _data;
        private Button _button;
        private int _pointersInside = 0;
        void Start()
        {
            _button = GetComponent<Button>();
            _data = new PointerEventData(FindObjectOfType<EventSystem>());
        }
        public override void OnLaserPointerClick()
        {
            if (_button.onClick != null)
            {
                _button.onClick.Invoke();
            }
        }
        public override void OnLaserPointerEnter()
        {
            _pointersInside++;
            if (_pointersInside == 2) return;
            if (_data == null) return;
            _button.OnPointerEnter(_data);
        }
        public override void OnLaserPointerExit()
        {
            _pointersInside--;
            if (_pointersInside > 0) return;
            if (_data == null) return;
            _button.OnPointerExit(_data);
        }
    }
}