using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickThis : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    private bool isPointerDown = false;
    private bool isLongPressTriggered = false;
    private Coroutine longPressCoroutine;
    public float requiredHoldTime = 1f;
    private Image buttonImg;

    private void Click_this()
    {
        if(isLongPressTriggered)
        {
            isLongPressTriggered = false;
            return;
        }
        
        main JS;
        if (SharedVariables.JS == null)
        {
            JS = GameObject.Find("Global controller").GetComponent<main>();
            SharedVariables.JS = JS;
        }
        JS = SharedVariables.JS;
        UIcontroller uicontroller;
        if (SharedVariables.uIcontroller == null)
        {
            uicontroller = GameObject.Find("Global controller").GetComponent<UIcontroller>();
            SharedVariables.uIcontroller = uicontroller;
        }
        uicontroller = SharedVariables.uIcontroller;
        uicontroller.CloseHudong();

        if ( uicontroller.Substr(gameObject.name,0,4) == "020")
        {
            uicontroller.WriteToPop(gameObject.name.Substring(4));
        }
        else
        {
            JS.SendMsg(gameObject.name);
            
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImg = gameObject.GetComponent<Image>();
        if (buttonImg != null)
            buttonImg.color = SharedVariables.pressedColor;
        isPointerDown = true;
        longPressCoroutine = StartCoroutine(LongPressCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        if (buttonImg != null)
            buttonImg.color = SharedVariables.originalColor;
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }
        if (SharedVariables.CmdTip.active)
            SharedVariables.CmdTip.SetActive(false);
    }

    public IEnumerator LongPressCoroutine()
    {
        yield return new WaitForSeconds(requiredHoldTime);

        if (isPointerDown)
        {
            // ³¤°´ÊÂ¼þ´¥·¢
            isLongPressTriggered = true;

            // ÔÚÕâÀïÖ´ÐÐ³¤°´´¥·¢µÄ²Ù×÷
            SharedVariables.CmdTip.SetActive(true);
            GameObject temp = SharedVariables.CmdTip.transform.GetChild(0).gameObject;
            if (temp != null)
            {
                temp.GetComponent<TMP_Text>().text = gameObject.name;
                yield return new WaitForEndOfFrame();
                float width = temp.GetComponent<RectTransform>().rect.width;
                SharedVariables.CmdTip.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 50, 100);
            }
                
            
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click_this();
    }
}
