using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtonsFunctionality : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonToHide;
    // Start is called before the first frame update
    public void Hidebutton()
    {
        _buttonToHide.SetActive(false);
    }
    public void ShowButton()
    {
        _buttonToHide.SetActive(true);
    }
}
