using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine.Android;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using DeadMosquito.AndroidGoodies;
using MiniJSON;
using UnityEngine.Networking;
using System.IO;

public class main : MonoBehaviour
{

    public TcpClient sock;
    public Thread clock;
    public GameObject passText;
    public GameObject idText;
    public GameObject region;
    public GameObject LoginButton;
    public GameObject Notice;
    public GameObject Window;
    public GameObject VersionText;
    public GameObject PingText;
    public GameObject IpInput;
    public GameObject AccPanel;
    public GameObject updateSlider, updateInf, updateRate;



    private NetworkStream networkStream;
    private UIcontroller uicontroller;
    private string ip;
    private string id;
    private string pass;
    private int port;
    private bool UTF8 = false;
    private bool isConnected_once = false;
    private Coroutine rotate;
    private string HashCode;


    private ConcurrentQueue<string> PluginLine = new ConcurrentQueue<string>();

    private Thread ClientTask;

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.INTERNET"))
        {
            string[] permissions = { "android.permission.INTERNET" };
            Permission.RequestUserPermissions(permissions);
        }

        VersionText.GetComponent<TMP_Text>().text = "µ±Ç°°æ±¾£º" + Application.version;

        StartCoroutine(VersionCheck());
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        int Performance = PlayerPrefs.GetInt("Performance", 1);
        switch (Performance)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Screen.SetResolution(0, 0, true, 0);
                Application.targetFrameRate = 90;
                break;
            case 3:
                Screen.SetResolution(0, 0, true, 0);
                Application.targetFrameRate = 120;
                break;
        }
        if (SharedVariables.uIcontroller == null)
        {
            uicontroller = gameObject.GetComponent<UIcontroller>();
            SharedVariables.uIcontroller = uicontroller;
        }
        uicontroller = SharedVariables.uIcontroller;
        //PlayerPrefs.SetString("Accounts", "");
        string account_inf = PlayerPrefs.GetString("Accounts", "");
        if (account_inf != "")
        {
            string account_ori = account_inf.Split("<|>")[0];
            var inf_ = account_ori.Split("$zj#");
            if (inf_.Length < 6)
                return;
            idText.GetComponent<TMP_InputField>().text = inf_[0];
            passText.GetComponent<TMP_InputField>().text = inf_[1];
            TMP_Dropdown dropdown = GameObject.Find("menu/Region").GetComponent<TMP_Dropdown>();

            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == inf_[2])
                {
                    dropdown.value = i;
                    break;
                }
            }
            if (inf_[2] == "×Ô¶¨Òå")
            {
                GameObject.Find("ipInput").GetComponent<TMP_InputField>().text = inf_[3] + ":" + inf_[4];
                if (inf_[5] == "GBK")
                    GameObject.Find("utf8Toggle").GetComponent<Toggle>().isOn = false;
                else
                    GameObject.Find("utf8Toggle").GetComponent<Toggle>().isOn = true;
            }
        }


        idText.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("id");
        passText.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("pass");
        region.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("region",0);

        LoginButton.GetComponent<Button>().onClick.AddListener(LoginListener);

    }

    private void LoginListener()
    {
        id = idText.GetComponent<TMP_InputField>().text;
        ip = "124.220.17.206";
        pass = passText.GetComponent<TMP_InputField>().text;
        StartCoroutine(LoginCheck());
    }


    private void Update()
    {
        if(isConnected_once)
        {
            isConnected_once = false;
            StartCoroutine(Clock());
        }

        if(PluginLine.Count > 0)
        {
            int length = PluginLine.Count;
            for(int i=0;i<length;i++)
            {

                string content;
                if(PluginLine.TryDequeue(out content))
                {
                    if (SharedVariables.WV != null && !SharedVariables.WV.WebView.IsDisposed)
                        SharedVariables.WV.WebView.PostMessage(content.Replace("\r", "").Replace("\n", ""));
                }
                else
                {
                    continue;
                }


            }

        }
    }


    private void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        uicontroller.SetForegroundService(false);
        if (sock.Connected)
        {
            networkStream = null;
            sock.Close();
            if (ClientTask.IsAlive)
                ClientTask.Abort();
        }
    }

    public void Connect()
    {
        sock = new TcpClient();
        int option = region.GetComponent<TMP_Dropdown>().value;
        PlayerPrefs.SetInt("region", option);
        switch(option)
        {
            case 0:
                port = 4017;
                break;
            case 1:
                port = 4027;
                break;
            case 2:
                port = 4037;
                break;
            case 3:
                port = 3067;
                break;
            case 4:
                port = 3070;
                break;
            case 5:
                port = 3060;
                break;
            case 6:
                port = 3077;
                break;
            case 7:
                port = 3090;
                break;
            case 8:
                string temp = GameObject.Find("ipInput").GetComponent<TMP_InputField>().text;
                var step = temp.Split(":");
                if (step.Length != 2)
                {
                    AGUIMisc.ShowToast("¸ñÊ½²»ÕýÈ·£¡");
                    return;
                }
                ip = step[0];
                int.TryParse(step[1], out port);
                break;

        }

        try
        {
            sock.Connect(IPAddress.Parse(ip), port);
            networkStream = sock.GetStream();
            ClientTask = new Thread(ReceiveMessage);
            ClientTask.IsBackground = true;
            ClientTask.Priority = System.Threading.ThreadPriority.AboveNormal;
            ClientTask.Start();
            SendMsg("zjmDMaIpOvxdb");
            isConnected_once = true;
            
        }
        catch (Exception e)
        {
            GameObject.Find("LoginInf").GetComponent<TMP_Text>().text = e.Message;

            throw;
        }

        PlayerPrefs.SetString("id", id);
        PlayerPrefs.SetString("pass", pass);
        string type = "GBK";
        if (UTF8)
            type = "UTF8";
        string account_inf = id + "$zj#" + pass + "$zj#" + region.GetComponent<TMP_Dropdown>().captionText.text + "$zj#"
            + ip + "$zj#" + port.ToString() + "$zj#" + type;
        SharedVariables.AccountInf = account_inf;
        string account_ori = PlayerPrefs.GetString("Accounts", "");
        string account_res = "";
        if (account_ori == "")
            account_res = account_inf;
        else
        {
            var account_j = account_ori.Split("<|>");
            bool isIn = false;
            for (int i=0;i<account_j.Length;i++)
            {
                if (account_j[i] == account_inf)
                {
                    isIn = true;
                    account_res = account_j[i] + "<|>" + account_res;
                }
                else
                {
                    account_res += account_j[i] + "<|>";
                }
            }
            if (!isIn)
                account_res = account_inf + "<|>" + account_ori;
        }
        if (account_res.EndsWith("<|>"))
            account_res = account_res.Substring(0, account_res.Length - 3);
        PlayerPrefs.SetString("Accounts", account_res);

    }

    IEnumerator Clock()
    {
        while(true)
        {
            //Ping a = new Ping("180.102.17.2");
            //while (!a.isDone)
            //{
            //    yield return new WaitForSeconds(0.1f);
            //}
            //SharedVariables.Ping = a.time;
            //long ping_part = SharedVariables.Ping;
            //string tempText = "";
            //if (ping_part < 100 && ping_part > 0)
            //{
            //    tempText = "<color=#669900>" + ping_part + "ms</color>";
            //}
            //else if (ping_part >= 100 && ping_part < 300)
            //{
            //    tempText = "<color=#FF9900>" + ping_part + "ms</color>";

            //}
            //else if(ping_part>=300 &&ping_part<=3500)
            //{
            //    tempText = "<color=#FF0000>" + ping_part + "ms</color>";

            //}
            //else
            //{
            //    tempText = "<color=#FF0000>¶ÏÏß</color>";
            //}

            DateTime tempTime = System.DateTime.Now;
            string Hour = tempTime.Hour.ToString();
            string Minute = tempTime.Minute.ToString();
            if (Hour.Length == 1)
                Hour = "0" + Hour;
            if (Minute.Length == 1)
                Minute = "0" + Minute;
            string tempText = Hour + ":" + Minute;
            tempText += "\n<color=#00B3B3>" + Math.Round(1 / Time.deltaTime) + "FPS</color>";
            
            PingText.GetComponent<TMP_Text>().text = tempText;



            if(!sock.Connected)
            {
                uicontroller.ShowInf("[1;36mÓëÓÎÏ··þÎñÁ¬½ÓÖÐ¶Ï£¬¼´½«ÖØÁ¬¡£¡£¡£[0;0m");

                while(true)
                {
                    yield return new WaitForSeconds(1f);
                    try
                    {
                        uicontroller.ShowInf("[1;36mÕýÔÚÖØÁ¬ÖÐ¡£¡£¡£[0;0m");
                        sock.Close();
                        sock.Dispose();
                        sock = null;
                        sock = new TcpClient();
                        sock.Connect(IPAddress.Parse(ip), port);
                        networkStream.Close();
                        networkStream = null;
                        networkStream = sock.GetStream();
                        ClientTask.Abort();
                        ClientTask = null;
                        ClientTask = new Thread(ReceiveMessage);
                        ClientTask.Start();
                        SendMsg("zjmDMaIpOvxdb");
                        SendMsg(id + "¨U" + pass + "¨UAbcd1234Zwy¨U3213@qq.com");
                        break;

                    }
                    catch (Exception e)
                    {
                        uicontroller.ShowInf("[1;36mÖØÁ¬Ê§°Ü£¬Ô­Òò£º[0;0m\n" + e.Message);

                        throw;
                    }
                }
            }

            yield return new WaitForSeconds(3.5f);
        }
    }

    public void MessageProcessor(string str)
    {
        var tempstr = str.Split("\n");
        for (var i = 0; i < tempstr.Length; i++)
        {
            uicontroller.StrSelector(tempstr[i]);
            MessageParser(tempstr[i]);
        }

    }

    public string ClearFormat(string str)
    {
        string strs = str;
        string temp = Regex.Replace(strs, "\\[\\d.*?m", "");
        temp = Regex.Replace(temp, "\\[.*?]", "");
        return temp;
    }

    public void MessageParser(string message)
    {
        string data = message;
        if (data.Length < 4)
            return;

        data = data.Replace("[2;37;0m", "[0;0m");

        if (data.StartsWith("[0;0m"))
            data = data.Replace("[0;0m", "");

        if (data.Length < 4)
            return;
        string temp = data.Substring(0, 4);
        string str = data.Substring(4);
        if (temp == "100")//ÁÄÌìÐÅÏ¢
        {
            SharedVariables.Chat += ClearFormat(str) + "%SV%";
            PluginLine.Enqueue("chat¨U@¨U" + SharedVariables.Chat);

        }
        else if (temp == "002")//µØµã
        {
            SharedVariables.Obj = "";
            string strk = ClearFormat(str);
            strk = strk.Replace("\r", "").Replace("\n", "");
            SharedVariables.Here = strk;


                PluginLine.Enqueue("obj¨U@¨U" + SharedVariables.Obj);
                PluginLine.Enqueue("here¨U@¨U" + SharedVariables.Here);

        }
        else if (temp == "003")//³ö¿ÚºÍ×ßÎ»
        {
            SharedVariables.Exits = str;
                PluginLine.Enqueue("exits¨U@¨U" + SharedVariables.Exits);

        }
        else if (temp == "004")//longÄÚÎÄ±¾£¬³¤ÃèÊö
        {

            SharedVariables.Long = str;
            PluginLine.Enqueue("long¨U@¨U" + SharedVariables.Long);
            int index = str.IndexOf("Ã÷ÏÔµÄ³ö¿Ú£º");
            if (index == -1)
                return;
            if (str.Length < index + 6)
                return;
            string res = str.Substring(index + 6);
            string strk = ClearFormat(res);
            strk = strk.Replace("\r", "");
            strk = strk.Replace("\n", "");
            if (strk != null)
            {
                SharedVariables.Direction = strk;
                PluginLine.Enqueue("direction¨U@¨U" + SharedVariables.Direction);
            }
        }
        else if (temp == "005")//objÄÚµÄ°´Å¥Ôö¼Ó
        {
            var strs = str.Split("$zj#");
            for (int i = 0; i < strs.Length; i++)
            {
                var dirs = strs[i].Split(':');
                if (dirs.Length != 2) continue;
                SharedVariables.Obj += ClearFormat(dirs[0]) + ":" + dirs[1] + "%SV%";//Ôö¼Ó¹²Ïí±äÁ¿ÖÐµÄObj
            }

            PluginLine.Enqueue("obj¨U@¨U" + SharedVariables.Obj);
        }
        else if (temp == "905")//É¾È¥objÄÚ°´Å¥
        {
            if (!SharedVariables.Obj.Contains(str))
            {
                str = str.Replace("\n", "").Replace("\r", "");
            }
            var s = SharedVariables.Obj.Split("%SV%");//É¾³ý¹²Ïí±äÁ¿ÖÐµÄObj
            string res = "";
            if (s.Length <= 1)
                return;
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i].Contains(str))
                    continue;
                res += s[i] + "%SV%";
            }
            SharedVariables.Obj = res;
            PluginLine.Enqueue("obj¨U@¨U" + SharedVariables.Obj);
        }
        else if (temp == "007")//»¥¶¯ÎÄ±¾
        {
            SharedVariables.Hudong = str;
            PluginLine.Enqueue("hudong¨U@¨U" + SharedVariables.Hudong);
        }
        else if (temp == "008" || temp == "009")//±íÇé¶¯×÷ÎÞÓÃ
        {
            SharedVariables.Hudong_Buttons = str;
            PluginLine.Enqueue("hudong_Buttons¨U@¨U" + SharedVariables.Hudong_Buttons);
        }
        else if (temp == "012")//×´Ì¬²Û
        {
            SharedVariables.State = str;
            PluginLine.Enqueue("state¨U@¨U" + SharedVariables.State);
            var strs = str.Split("¨U");
            if (strs.Length < 8)
                return;

            string[] oneStep = new string[8];

            for(int i=1;i<8;i++)
            {
                var res = strs[i].Split(":");
                if (res.Length < 2)
                    continue;
                oneStep[i] = res[1];
            }

            var blood = oneStep[1].Split("/");
            if (blood.Length < 3)
                return;
            long.TryParse(blood[0], out SharedVariables.HP);
            long.TryParse(blood[2], out SharedVariables.HP_MAX);

            var force = oneStep[2].Split("/");
            if (force.Length < 2)
                return;
            long.TryParse(force[0], out SharedVariables.Force);
            long.TryParse(force[1], out long force_full);
            SharedVariables.Force_MAX = force_full *2;

            var will = oneStep[3].Split("/")[0];
            long.TryParse(will, out SharedVariables.Mind);

            var EXP = oneStep[5].Split("/")[0];
            long.TryParse(EXP, out SharedVariables.EXP);

            var potential = oneStep[6].Split("/")[0];
            long.TryParse(potential, out SharedVariables.Potential);

            var energy = oneStep[7].Split("/")[0];
            long.TryParse(energy, out SharedVariables.Energy);
            if (strs[0].Contains("<busy"))
            {
                if (strs[0].Contains("<busy>"))
                {
                    SharedVariables.Busy = -1;
                }
                else
                {
                    string a = Regex.Match(strs[0], "<busy .*?s>").Value;
                    string b = Regex.Replace(a, @"[^0-9]+", "");
                    int.TryParse(b, out SharedVariables.Busy);
                    if (SharedVariables.Busy == 1)
                    {
                        SharedVariables.Busy = 0;
                    }
                }
            }
            else
            {
                SharedVariables.Busy = 0;
            }
        }
        else if (temp == "013")//ÏàÍ¬µÄ»¥¶¯ÎÄ±¾
        {
            SharedVariables.Hudong = str;
            PluginLine.Enqueue("hudong¨U@¨U" + SharedVariables.Hudong);
        }
        else 
        {
            SharedVariables.Out += ClearFormat(data) + "%SV%";
            if (SharedVariables.Out.Length > 2000)
            {
                SharedVariables.Out = SharedVariables.Out.Substring(500);
            }
            PluginLine.Enqueue("out¨U@¨U" + SharedVariables.Out);
        }
    }

    public void ReceiveMessage()
    {

        List<byte> temp = new List<byte>();
        while (true)
        {
            byte[] buffer = new byte[4096];
            int size = networkStream.Read(buffer,0,buffer.Length);
            temp.AddRange(buffer.Take(size));
            if(buffer.Length>0 && buffer[size-1] == '\n')
            {
                byte[] buf = temp.ToArray();
                temp.Clear();
                string res;
                if (!UTF8)
                    res = Encoding.GetEncoding("GB2312").GetString(buf);
                else
                    res = Encoding.Default.GetString(buf);
                MessageProcessor(res);
            }
            else
            {
                continue;
            }
        }

    }
    public void DownLoad()
    {
        StartCoroutine(DownloadFile());
        GameObject.Find("downloadButton").GetComponent<Button>().interactable = false;
        GameObject.Find("NoticeButton").GetComponent<Button>().interactable = false;
        updateSlider.GetComponent<Slider>().value = 0;
        updateRate.GetComponent<TMP_Text>().text = "0 %";
    }

    IEnumerator VersionCheck()
    {
        TokenJson token = new TokenJson();
        token.exp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 30;
        token.iss = "BieXiang";
        
        string token_str = JsonUtility.ToJson(token);
        
        string token_str_s = HmacSHA256(token_str,token.GetKey());
        
        string token_str_b = Convert.ToBase64String(Encoding.Default.GetBytes(token_str));
        string token_str_s_b = Convert.ToBase64String(Encoding.Default.GetBytes(token_str_s));
        string temp = token_str_b + "." + token_str_s_b;
        Debug.Log(temp);
        WWW version = new WWW("http://" + SharedVariables.Server_socket + "/update.php?token=" + temp);
        yield return version;
        if(version.error !=null)
        {
            updateInf.GetComponent<TMP_Text>().text = "ÍøÂçÁ¬½ÓÊ§°Ü...";
            yield break;
        }
        string eText = version.text.TrimStart('\uFEFF');
        Dictionary<string, object> jsonDict = Json.Deserialize(eText) as Dictionary<string, object>;
        TMP_Text login_inf = GameObject.Find("NoticeText").GetComponent<TMP_Text>();
        login_inf.text = jsonDict["updateInf"].ToString();
        HashCode = jsonDict["Hash"].ToString();
        yield return new WaitForFixedUpdate();
        login_inf.text = login_inf.text + "\n";
        if (float.Parse(jsonDict["paramVer"].ToString()) > PlayerPrefs.GetFloat("paramVer", 0f))
        {
            updateSlider.GetComponent<Slider>().value = 75;
            updateRate.GetComponent<TMP_Text>().text = "75 %";
            updateInf.GetComponent<TMP_Text>().text = "ÕýÔÚ¸üÐÂ²ÎÊý...";
            WWW paramG = new WWW("http://" + SharedVariables.Server_socket + "/param.php?token=" + temp);
            yield return paramG;
            if (paramG.error != null)
            {
                updateInf.GetComponent<TMP_Text>().text = "²ÎÊý¸üÐÂÊ§°Ü";
                yield break;
            }
            string GText = paramG.text.TrimStart('\uFEFF');
            PlayerPrefs.SetString("params", GText);
            PlayerPrefs.SetFloat("paramVer", float.Parse(jsonDict["paramVer"].ToString()));
        }
        if (float.Parse(Application.version) < float.Parse(jsonDict["version"].ToString()))
        {
            updateInf.GetComponent<TMP_Text>().text = "ÓÐÄÚÈÝÐèÒª¸üÐÂ£¡";
            GameObject.Find("downloadButton").GetComponent<Button>().interactable = true;
            updateSlider.GetComponent<Slider>().value = 100;
            updateRate.GetComponent<TMP_Text>().text = "100 %";
        }
        else
        {
            updateInf.GetComponent<TMP_Text>().text = "¸üÐÂ¼ì²éÍê±Ï£¡";
            updateSlider.GetComponent<Slider>().value = 100;
            updateRate.GetComponent<TMP_Text>().text = "100 %";
            yield return new WaitForSeconds(0.5f);
            Notice.SetActive(false);
        }
    }

    IEnumerator DownloadFile()
    {
        updateInf.GetComponent<TMP_Text>().text = "¼´½«ÎªÄú¿ªÊ¼ÏÂÔØ...";
        string filePath = Path.Combine(Application.persistentDataPath, "download");
        if (File.Exists(filePath + "/wuxia.apk"))
            File.Delete(filePath + "/wuxia.apk");
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        UnityWebRequest.ClearCookieCache();
        updateInf.GetComponent<TMP_Text>().text = "ÕýÔÚÏÂÔØÐÂappÖÐ...";
        UnityWebRequest webRequest = UnityWebRequest.Get("http://" + SharedVariables.Server_socket + "/wuxia.apk");
        StartCoroutine(ShowRate(webRequest));
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError)
        {
            updateInf.GetComponent<TMP_Text>().text = "ÏÂÔØÊ§°Ü£¬Çëµ½Ê¦ÃÅÈºÏÂÔØ¡£";
        }
        else
        {
            byte[] File = webRequest.downloadHandler.data;
            FileStream nFile = new FileStream(filePath + "/wuxia.apk", FileMode.Create);
            nFile.Write(File, 0, File.Length);
            nFile.Close();
            
        }
        
    }

    private bool VerifyFileSHA1(string filePath, string expectedHash)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha1.ComputeHash(fileStream);

                // ½«¹þÏ£×Ö½ÚÊý×é×ª»»ÎªÊ®Áù½øÖÆ×Ö·û´®
                string actualHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // ÓëÔ¤ÆÚµÄ¹þÏ£Öµ½øÐÐ±È½Ï
                Debug.Log(actualHash);
                return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    IEnumerator ShowRate(UnityWebRequest webRequest)
    {
        while (true)
        {
            string size = webRequest.GetResponseHeader("Content-Length");
            string path = Path.Combine(Application.persistentDataPath, "download/wuxia.apk");
            if(size != null)
            {
                double temp;
                double.TryParse(size, out temp);
                size = Math.Round(temp / 1048576, 1).ToString();
            }
            double downloadProgress = Math.Round(webRequest.downloadProgress * 90, 1);
            double fileSize = Math.Round((double)webRequest.downloadedBytes / 1048576, 1);
            updateRate.GetComponent<TMP_Text>().text = fileSize.ToString("0.0") + "MB/" + size + "MB  " + downloadProgress.ToString("0.0") + " %";
            updateSlider.GetComponent<Slider>().value = (float)downloadProgress;
            updateInf.GetComponent<TMP_Text>().text = "ÕýÔÚÏÂÔØÐÂ°æappÖÐ...";
            if (webRequest.isDone)
            {
                updateInf.GetComponent<TMP_Text>().text = "ÏÂÔØÍê³É£¬ÕýÔÚÑéÖ¤...";
                if (VerifyFileSHA1(path, HashCode))
                {
                    updateSlider.GetComponent<Slider>().value = 100;
                    updateRate.GetComponent<TMP_Text>().text = "100 %";
                    updateInf.GetComponent<TMP_Text>().text = "ÑéÖ¤³É¹¦£¬¼´½«¿ªÊ¼°²×°...";
                    yield return new WaitForSeconds(0.5f);
                    AndroidJavaClass javaClass = new AndroidJavaClass("com.unity.install.Install");
                    javaClass.CallStatic<bool>("InstallApk", path);
                }
                else
                {
                    updateInf.GetComponent<TMP_Text>().text = "ÑéÖ¤Ê§°Ü£¬ÇëÖØÐÂÏÂÔØ¡£";
                }

                break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }


    public void CloseNotice()
    {
        Notice.SetActive(false);
    }

    public void ChangeRegion()
    {
        if (region.GetComponent<TMP_Dropdown>().value == 8)
        {
            IpInput.SetActive(true);
        }
        else
            IpInput.SetActive(false);
    }

    public async void SendMsg(string msg)
    {
        try
        {

            byte[] bytes;
            if(!UTF8)
                bytes = Encoding.GetEncoding("GB2312").GetBytes(msg + "\n");
            else
                bytes = Encoding.Default.GetBytes(msg + "\n");
            await networkStream.WriteAsync(bytes, 0, bytes.Length);
        }
        catch (Exception)
        {
        }
    }

    IEnumerator LoginCheck()
    {
        Window.SetActive(true);
        rotate = StartCoroutine(RotateLoad(GameObject.Find("Loading")));
        if (region.GetComponent<TMP_Dropdown>().value == 8)
        {
            GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "Ìø¹ýÑéÖ¤£¬ÕýÔÚµÇÈëÓÎÏ·..";
            UTF8 = GameObject.Find("utf8Toggle").GetComponent<Toggle>().isOn;
            Connect();
            SharedVariables.ID = id;
            SendMsg(id + "¨U" + pass + "¨UAbcd1234Zwy¨U3213@qq.com");
            yield break;
        }
        WWW login = new WWW("http://124.220.17.206/mud_login/loginto_zf.php?id=" + id + "&pass=" + pass + "&page=1");
        yield return login;
        if (login.error != null)
        {
            GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "ÍøÂç²»Õý³££¬Çë¼ì²éÍøÂç.";
            Invoke("StopRotate", 2);
        }
        else
        {
            if (string.IsNullOrEmpty(login.text))
            {
                GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "·þÎñÆ÷ÒÑÍÑ»ú.";
                Invoke("StopRotate", 2);
            }
            else
            {
                if(login.text != "ÃÜÂë´íÎó" && login.text != "ÓÃ»§²»´æÔÚ")
                {
                    GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "ÑéÖ¤³É¹¦£¬ÕýÔÚµÇÈëÓÎÏ·..";
                    Connect();
                    SharedVariables.ID = id;
                    SendMsg(id + "¨U" + pass + "¨UAbcd1234Zwy¨U3213@qq.com");
                }
                else
                {
                    GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "ÕËºÅ»òÃÜÂë´íÎó£¬Çë¼ì²é£¡";
                    Invoke("StopRotate", 2);
                }

            }

        }

    }


    public void StopRotate()
    {
        GameObject temp = GameObject.Find("LoadText");
        if (temp == null)
            return;
        temp.GetComponent<TMP_Text>().text = "ÕýÔÚÑéÖ¤ÕËºÅÃÜÂëÖÐ..";
        StopCoroutine(rotate);
        Window.SetActive(false);
    }

    private IEnumerator RotateLoad(GameObject rotate)
    {
        int x = 0;
        while(true)
        {
            x -= 10;
            rotate.transform.Rotate(new Vector3(0, 0, x), 3f);
            yield return new WaitForFixedUpdate();
        }
    }


    private string HmacSHA256(string secret, string signKey)
    {
        string signRet = string.Empty;
        using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
        {
            byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
            signRet = Convert.ToBase64String(hash);
            signRet = ToHexString(hash); 
        }
        
        return signRet;
    }

    private static string ToHexString(byte[] bytes)
    {
        string hexString = string.Empty;
        if (bytes != null)
        {
            StringBuilder strB = new StringBuilder();
            foreach (byte b in bytes)
            {
                strB.AppendFormat("{0:x2}", b);
            }
            hexString = strB.ToString();
        }
        return hexString;

    }

    private void OnApplicationPause(bool focus)
    {
        if (!focus)   
        {
            if (SharedVariables.ID == "")
                return;
            if (PlayerPrefs.GetInt("isBackData", 0) == 1)
            {
                uicontroller.line.Clear();
                SendMsg("look");
            }
            else
            {
                while (uicontroller.line.Count > 100)
                    uicontroller.line.TryDequeue(out _);
                SendMsg("look");

            }
        }
        
    }

    public void AccountSelected()
    {
        string content = PlayerPrefs.GetString("Accounts", "");
        if (content == "")
        {
            AGUIMisc.ShowToast("Ã»ÓÐ±£´æµÄÕËºÅ£¡");
            return;
        }
        Window.SetActive(true);
        AccPanel.SetActive(true);
        GameObject temp = Resources.Load("AccountUnit") as GameObject;
        GameObject parent_ = GameObject.Find("AccountsContent");
        var lines = content.Split("<|>");
        for (int i=0;i<lines.Length;i++)
        {
            var res = lines[i].Split("$zj#");
            if (res.Length < 6)
                continue;
            GameObject a = Instantiate(temp);
            a.transform.SetParent(parent_.transform, true);
            a.GetComponent<AccountUnit>().InitUnit(res[0], res[1], res[2], res[3], res[4], res[5]);
        }
        
        

    }

    public void CloseAccount()
    {
        GameObject AccContent = GameObject.Find("AccountsContent");
        foreach (Transform child in AccContent.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject.Find("AccountView").SetActive(false);
        GameObject.Find("Window").SetActive(false);
    }

}


