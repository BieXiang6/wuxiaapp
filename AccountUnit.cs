using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DeadMosquito.AndroidGoodies;

public class AccountUnit : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool finished = false;

    private string id;
    private string password;
    private string region;
    private string ip;
    private string port;
    private string type;
    private float requiredHoldTime = 2f;

    private bool isPointerDown = false;
    private bool isLongPressTriggered = false;

    private Coroutine longPressCoroutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLongPressTriggered)
        {
            isLongPressTriggered = false;
            return;
        }
        if (!finished)
            return;
        GameObject.Find("menu/id").GetComponent<TMP_InputField>().text = id;
        GameObject.Find("menu/pass").GetComponent<TMP_InputField>().text = password;
        TMP_Dropdown dropdown = GameObject.Find("menu/Region").GetComponent<TMP_Dropdown>();

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == region)
            {
                dropdown.value = i;
                break;
            }
        }

        if (region == "自定义")
        {
            GameObject.Find("ipInput").GetComponent<TMP_InputField>().text = ip + ":" + port;
            if (type == "GBK")
                GameObject.Find("utf8Toggle").GetComponent<Toggle>().isOn = false;
            else
                GameObject.Find("utf8Toggle").GetComponent<Toggle>().isOn = true;
        }
        GameObject AccContent = GameObject.Find("AccountsContent");
        foreach (Transform child in AccContent.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject.Find("AccountView").SetActive(false);
        GameObject.Find("Window").SetActive(false);
    }

    public void InitUnit(string id_i, string password_i, string region_i, string ip_i, string port_i, string type_i)
    {
        id = id_i;
        password = password_i;
        ip = ip_i;
        port = port_i;
        type = type_i;
        finished = true;
        region = region_i;
        string temp_text = id + "--" + region + "\n" + ip + ":" + port + " " + type;
        gameObject.GetComponentInChildren<TMP_Text>().text = temp_text;
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        longPressCoroutine = StartCoroutine(LongPressCoroutine());
    }

    public IEnumerator LongPressCoroutine()
    {
        yield return new WaitForSeconds(requiredHoldTime);

        if (isPointerDown)
        {
            // 长按事件触发
            isLongPressTriggered = true;
            string account_inf = id + "$zj#" + password + "$zj#" + region + "$zj#" + ip + "$zj#" + port + "$zj#" + type;
            string account_ori = PlayerPrefs.GetString("Accounts", "");
            string account_res = "";
            if (account_ori == "")
                yield break;
            else
            {
                var account_j = account_ori.Split("<|>");
                for (int i = 0; i < account_j.Length; i++)
                {
                    if (account_j[i] != account_inf)
                    {
                        account_res += account_j[i] + "<|>";
                    }
                }
            }
            if (account_res.EndsWith("<|>"))
                account_res = account_res.Substring(0, account_res.Length - 3);
            PlayerPrefs.SetString("Accounts", account_res);
            AGUIMisc.ShowToast("删除成功！");
            GameObject AccContent = GameObject.Find("AccountsContent");
            foreach (Transform child in AccContent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject.Find("AccountView").SetActive(false);
            GameObject.Find("Window").SetActive(false);

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }
    }
}
