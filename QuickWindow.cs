using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickWindow : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public GameObject newUI;
    public GameObject draggableWindow;
    private RectTransform rectTransform;
    private Vector2 offset;
    private Vector2 newPosition;
    private float minX, minY, maxX, maxY;
    private bool isDragging = false;

    public void InitButton()
    {
        rectTransform = newUI.GetComponent<RectTransform>();
        // ������С�����ĺϷ�λ��
        CalculateMinMaxPositions();
    }
   

    private void CalculateMinMaxPositions()
    {
        Vector2 halfSize = rectTransform.rect.size * 0.5f;
        minX = halfSize.x;
        maxX = Screen.width - halfSize.x;

        minY = halfSize.y;
        maxY = Screen.height - halfSize.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        // �����µ�λ��
        newPosition = eventData.position + offset;

        // ����λ���ںϷ���Χ��
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // ����UIԪ��λ��
        rectTransform.position = newPosition;
    }



    public void OnPointerDown(PointerEventData eventData)
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
        Debug.Log(SharedVariables.Hudong_Buttons);
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
        draggableWindow.SetActive(true);
        newUI.SetActive(false);

    }
}
