using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmojiController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gameObject.name == "EmojiRecently")
        {
            GameObject HudongButtons = GameObject.Find("HDButtons");
            if (HudongButtons == null)
                return;
            StartCoroutine(SharedVariables.uIcontroller.LiaoTian_Recently(HudongButtons));
        }
        else if (gameObject.name == "EmojiNext")
        {
            GameObject HudongButtons = GameObject.Find("HDButtons");
            if (HudongButtons == null)
                return;
            if (SharedVariables.uIcontroller.EmojiPage == 4)
                return;
            StartCoroutine(SharedVariables.uIcontroller.LiaoTian_ChangePage(HudongButtons, true));
        }
        else if (gameObject.name == "EmojiPrevious")
        {
            GameObject HudongButtons = GameObject.Find("HDButtons");
            if (HudongButtons == null)
                return;
            if (SharedVariables.uIcontroller.EmojiPage < 2)
                return;
            StartCoroutine(SharedVariables.uIcontroller.LiaoTian_ChangePage(HudongButtons, false));
        }
    }

}
