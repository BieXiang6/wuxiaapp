using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class EmojiClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        string text = gameObject.GetComponentInChildren<TMP_Text>().text;
        string num = TextGainCenter("<sprite=", ">", text);
        string RecentlyEmoji = PlayerPrefs.GetString("RecentlyEmoji", "0;1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;21;22;23;24;25;26;27;28;29");
        List<string> dir = new List<string>(RecentlyEmoji.Split(";"));
        string res = num;
        if(dir.Contains(num))
        {
            dir.Remove(num);
            for(int i=0;i<dir.Count;i++)
                res += ";" + dir[i];
        }
        else
        {
            for(int i=0;i<dir.Count-1;i++)
                res += ";" + dir[i];
        }
        PlayerPrefs.SetString("RecentlyEmoji", res);
        TMP_InputField temp = GameObject.Find("HudongInput").GetComponent<TMP_InputField>();
        temp.text = temp.text + text;
    }

    public static string TextGainCenter(string left, string right, string text)
    {

        if (string.IsNullOrEmpty(left))
            return "";
        if (string.IsNullOrEmpty(right))
            return "";
        if (string.IsNullOrEmpty(text))
            return "";
        int Lindex = text.IndexOf(left);
        if (Lindex == -1)
        {
            return "";
        }
        Lindex = Lindex + left.Length;
        int Rindex = text.IndexOf(right, Lindex);
        if (Rindex == -1)
        {
            return "";
        }
        return text.Substring(Lindex, Rindex - Lindex);
    }
}
