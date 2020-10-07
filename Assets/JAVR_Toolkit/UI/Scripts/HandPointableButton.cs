//  Javier Alvarez-Calleja
//  JAVR Framework 2020
//  Hand Pointable Button
//
//  Allows UI elements to interact with a raycast originating from a VR controller's transform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Button))]
public class HandPointableButton : HandPointableElement
{
    PointerEventData _data;
    private Button _button;
    private int _pointersInside = 0;
    void Start()
    {
        _button = GetComponent<Button>();
        _data = new PointerEventData( FindObjectOfType<EventSystem>() );
    }
    public override void OnHandPointerClick()
    {
        if(_button.onClick != null)
        {
            _button.onClick.Invoke();
        }
    }
    public override void OnHandPointerEnter()
    {
        _pointersInside++;
        if (_pointersInside == 2) return;
        if (_data == null) return;
        _button.OnPointerEnter( _data );
    }
    public override void OnHandPointerExit()
    {
        _pointersInside--;
        if (_pointersInside > 0) return;
        if (_data == null) return;
        _button.OnPointerExit( _data );
    }
}
