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
public class HandPointableButton : MonoBehaviour
{
    PointerEventData data;
    private Button button;
    private float pressedTime = 0.025f;
    private bool blocked = false;
    //[SerializeField]
    //private Button otherGraphic;
    void Start()
    {
        button = GetComponent<Button>();
        data = new PointerEventData( FindObjectOfType<EventSystem>() );
    }
    public virtual void OnHandPointerClick()
    {
        if(blocked) return;
        
        if(button.onClick != null)
        {
            button.onClick.Invoke();
        }
        // button.OnPointerClick( data );   
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine( PressedWait() );
    }
    public virtual void OnHandPointerEnter()
    {
        if (data == null) return;
        button.OnPointerEnter( data );
       /* if(otherGraphic != null)
        {
            otherGraphic.OnPointerEnter( data );
        }*/
    }
    public virtual void OnHandPointerExit()
    {
        if (data == null) return;
        button.OnPointerExit( data );
      /*  if (otherGraphic != null)
        {
            otherGraphic.OnPointerExit(data);
        }*/
    }
    private IEnumerator PressedWait()
    {
        button.OnPointerDown( data );
        yield return new WaitForSeconds(pressedTime);
        button.OnPointerUp( data );
        button.OnDeselect ( data );
    }
}
