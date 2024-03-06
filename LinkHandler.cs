using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DeadMosquito.AndroidGoodies;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{


    public void OnPointerClick(PointerEventData eventData)
    {


        main JS;
        if (SharedVariables.JS == null)
        {
            JS = GameObject.Find("Global controller").GetComponent<main>();
            SharedVariables.JS = JS;
        }
        JS = SharedVariables.JS;
        UIcontroller uIcontroller;
        if (SharedVariables.uIcontroller == null)
        {
            uIcontroller = GameObject.Find("Global controller").GetComponent<UIcontroller>();
            SharedVariables.uIcontroller = uIcontroller;
        }
        uIcontroller = SharedVariables.uIcontroller;
        TextMeshProUGUI textMeshPro = eventData.pointerPress.GetComponent<TextMeshProUGUI>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, Camera.main);

        if (linkIndex != -1) // if a link is found
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            uIcontroller.CloseHudong();
            string id = linkInfo.GetLinkID();
            if (id.Contains(":cmds:"))
            {
                JS.SendMsg(id.Substring(6));
            }
            else
            {
                OpenBrowser(id.Substring(1));
            }

        }

    }

    public void OpenBrowser(string url)
    {
        if (url == "")
            return;
        Application.OpenURL(url);
    }


}



