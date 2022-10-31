using System;
using UnityEngine;
using UnityEngine.UI;

public class InputCtrl : MonoBehaviour
{
    [SerializeField] private Button _buttonQuit;
    public Action OnPressSpace;
    public Action OnPressF;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnPressSpace.Invoke();
        }
        if (Input.GetKey(KeyCode.F))
        {
            OnPressF.Invoke();
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}