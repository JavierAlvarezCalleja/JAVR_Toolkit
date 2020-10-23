using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtonsFunctionality : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonTooHide;
    // Start is called before the first frame update
    public void Hidebutton()
    {
        _buttonTooHide.SetActive(false);
    }
    public void ShowButton()
    {
        _buttonTooHide.SetActive(true);
    }
}
