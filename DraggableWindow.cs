using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    private RectTransform rectTransform;
    private Vector2 offset;

    private float minX, minY, maxX, maxY;

    private bool isDragging = false;

    private Vector2 newPosition;

    public GameObject newUI;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // 计算最小和最大的合法位置
        CalculateMinMaxPositions();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        // 计算新的位置
        newPosition = eventData.position + offset;

        // 限制位置在合法范围内
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // 更新UI元素位置
        rectTransform.position = newPosition;
    }

    private void CalculateMinMaxPositions()
    {
        Vector2 halfSize = rectTransform.rect.size * 0.5f;
        minX = halfSize.x;
        maxX = Screen.width - halfSize.x;
        minY = halfSize.y;
        maxY = Screen.height - halfSize.y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = (Vector2)rectTransform.position - eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            return;
        }
        int isMovable = PlayerPrefs.GetInt("isMovable", 0);
        if (isMovable == 1)
        {
            if (!newUI.active)
            {
                newUI.SetActive(true);
                int hashCode = SharedVariables.AccountInf.GetHashCode();
                string temp = PlayerPrefs.GetString("Quick:" + hashCode.ToString(), "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
                "\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
                "look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");

                if (temp == "")
                    return;
                temp = Regex.Replace(temp, "\\$br#", "\n");
                int num = newUI.transform.childCount;
                int j = 0;
                for (int i = 0; i < num; i++)
                {
                    var child = newUI.transform.GetChild(j);
                    if (child.gameObject.name != "CloseCmds")
                    {
                        SharedVariables.uIcontroller.Button_Pool.ReleaseObject(child.gameObject);
                    }
                    else
                    {
                        Destroy(child.gameObject);
                        j++;
                    }
                }
                var strs = temp.Split("$zj#");
                float width = newUI.GetComponent<RectTransform>().rect.width;
                float height = newUI.GetComponent<RectTransform>().rect.height;
                newUI.GetComponent<GridLayoutGroup>().cellSize = new Vector2((width - 25) / 6, height / 2);
                newUI.GetComponent<GridLayoutGroup>().spacing = new Vector2(2, 2);
                for (var i = 0; i < 11; i++)
                {
                    if (i < strs.Length)
                    {
                        if (strs[i].Length < 2) continue;
                        var dirs = strs[i].Split(':');
                        SharedVariables.uIcontroller.Button_Pool.GetObject(newUI.transform, SharedVariables.uIcontroller.ColorPut(dirs[1]), dirs[2], 35);
                    }
                    else
                    {
                        SharedVariables.uIcontroller.Button_Pool.GetObject(newUI.transform, "空", "look", 35);
                    }

                    if (i == 4)
                    {
                        GameObject dddd = SharedVariables.RL.GetObject(SharedVariables.RL.ObjButton);
                        QuickWindow e_script = dddd.AddComponent<QuickWindow>();
                        e_script.newUI = newUI;
                        e_script.draggableWindow = gameObject;
                        e_script.InitButton();
                        dddd.GetComponentInChildren<TMP_Text>().text = "关闭";
                        dddd.name = "CloseCmds";
                        dddd.transform.SetParent(newUI.transform, false);

                    }
                }
            }
            else
            {
                int num = newUI.transform.childCount;
                int j = 0;
                for (int i = 0; i < num; i++)
                {
                    var child = newUI.transform.GetChild(j);
                    if (child.gameObject.name != "CloseCmds")
                    {
                        SharedVariables.uIcontroller.Button_Pool.ReleaseObject(child.gameObject);
                    }
                    else
                    {
                        Destroy(child.gameObject);
                        j++;
                    }
                }
                newUI.SetActive(false);
            }
            
        }
        else
        {
            if(SharedVariables.uIcontroller.Mycmds.active)
                SharedVariables.uIcontroller.CloseMycmds();
            else
                SharedVariables.uIcontroller.OpenQuick();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        return;
    }
}
