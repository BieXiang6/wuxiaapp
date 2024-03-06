using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public GameObject far;
    public void But_clicked()
    {
        GameObject But = Instantiate(Resources.Load("ObjButton")) as GameObject;
        float width = far.GetComponent<RectTransform>().rect.width;
        float height = width / 2;
        But.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        But.transform.SetParent(far.transform);
        
    }

    
}
