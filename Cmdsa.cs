using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Cmdsa : MonoBehaviour
{
    private main JS;
    private UIcontroller uicontroller;

    private void Start()
    {
        if (SharedVariables.JS == null)
        {
            JS = GameObject.Find("Global controller").GetComponent<main>();
            SharedVariables.JS = JS;
        }
        JS = SharedVariables.JS;
        if (SharedVariables.uIcontroller == null)
        {
            uicontroller = GameObject.Find("Global controller").GetComponent<UIcontroller>();
            SharedVariables.uIcontroller = uicontroller;
        }
        uicontroller = SharedVariables.uIcontroller;
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(Click_this);

    }
    private void Click_this()
    {
        string temp = gameObject.name;
        uicontroller.CloseHudong();
        if (uicontroller.Substr(temp, 0, 4) == "020")
        {
            
            uicontroller.WriteToPop(temp.Substring(4));
        }
        else
        {

            JS.SendMsg(temp);

        }
        
    }
}
