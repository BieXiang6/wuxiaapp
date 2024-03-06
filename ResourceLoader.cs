using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{

    public bool nightMode = false;
    public GameObject NormalButton, ActButton, ChatText, EmojiButton,
        HudongButtons, HudongText, Input, ObjButton, OutText, RobotCell, RobotObj, PluginButton;

    void Awake()
    {
        int UIMode = PlayerPrefs.GetInt("UIMode", 1);
        if (UIMode == 0)
            nightMode = true;
        else
            nightMode = false;
        NormalButton = Resources.Load("DirButton") as GameObject;
        ActButton = Resources.Load("ActButton") as GameObject;
        ChatText = Resources.Load("ChatText") as GameObject;
        EmojiButton = Resources.Load("EmojiButton") as GameObject;
        HudongButtons = Resources.Load("HudongButtons") as GameObject;
        HudongText = Resources.Load("HudongText") as GameObject;
        Input = Resources.Load("Input") as GameObject;
        ObjButton = Resources.Load("ObjButton") as GameObject;
        OutText = Resources.Load("OutText") as GameObject;
        RobotCell = Resources.Load("RobotCell") as GameObject;
        RobotObj = Resources.Load("RobotObj") as GameObject;
        PluginButton = Resources.Load("PluginButton") as GameObject;
    }

    /// <summary>
    /// 获取对象实例
    /// </summary>
    /// <param name="tempGameObject">获取对象种类</param>
    /// <returns>对象实例</returns>
    public GameObject GetObject(GameObject tempGameObject)
    {
        if(!nightMode)
        {
            return Instantiate(tempGameObject);
        }
        else
        {
            return null;
        }
    }

    public void ChangeModel()
    {
        if(!nightMode)
        {
            nightMode = true;
            PlayerPrefs.SetInt("UIMode", 0);
        }
        else
        {
            nightMode = false;
            PlayerPrefs.SetInt("UIMode", 1);
        }
    }



}
