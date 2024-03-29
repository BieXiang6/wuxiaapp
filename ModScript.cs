using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Threading;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.Text;
using DeadMosquito.AndroidGoodies;
using System.IO;


public class ModScript : MonoBehaviour
{

    private main JS;
    public GameObject isSkill;
    public GameObject isCmd;
    public GameObject CheckBut;
    public GameObject StartButton;
    public GameObject ShiMenInf;
    public GameObject LianGongContent;
    public GameObject LianGongListItem;

    private UIcontroller uIcontroller;
    private PushNotification PN;
    private bool isSkill_bool = false;
    private bool isCmd_bool = false;
    private bool ShiMen_isFight = false;
    private bool ShiMen_AutoApply = false;
    private int ShiMen_RTT = 300;
    private bool ShiMen_isOn = false;
    private bool BoDong_isOn = false;
    private bool BoDong_isGetSecond = false;
    private bool LianGong_isOn = false;
    private bool FuBen_isOn = false;
    private bool FuBen_isEnd = true;
    private float LianGong_Speed = 70;

    private string SkillName;
    private string Cmds;
    private int Cmds_num;
    

    private string BoDong_Skill;
    private int BoDong_Num;
    private int BoDong1_Potential;
    private int BoDong2_Speed = 0;

    private string TenGodID;

    private List<Skill> LianGongList = new List<Skill>();
    private Dictionary<string, object> ShanHePosition = new Dictionary<string, object>();

    private string ShiMen_north = "金水桥 天山山路 刀店 大将军府 西街 易溪部 青龙街 碎石路 乌蒙村落 东大街 西大街 西镇街 东城 土地庙 打铁铺";
    private string ShiMen_south = "崇圣门 西村口 夺宝口 西大街 乌蒙村落 大竹楼 国子监 西街 白河 渔村小屋 西大街 碎石路 北内大街 一品堂大门 小院子 大将军府 楼梯";
    private string ShiMen_west = "东内大街 南大街 青石大道 昆仑山下 碎石路 苍山 永泰大道 南安大道 北大街 步雄部 忠烈祠 觅香楼";
    private string ShiMen_east = "北街 青石大道 南大街 沿湖大道 镇雄 赌场 刘庄 富家侧门";

    private List<string> TuiChe_way = new List<string>
    {
        "w","w","w","n","w","wu","wu","wu","w","wu","wu","nd","nd","nd","nd","nd","nd",
        "sw","nw","n","w","n","nw","w","w","w","w","w","w","w","w","w","w","w","w","w"
        ,"w","w","w","w","w","w","s","s","w","w","s","e","n"
    };

    private int ShiMen_Number_left;
    private int ShiMen_delay;
    private int ShiMen_Number;
    private long ShiMen_StartTime;

    private StringBuilder errorLogStringBuilder = new StringBuilder();

    Thread newBodong1;
    Thread LianGongThread;
    Thread FuBen;

    private Queue<string> TaskQueue = new Queue<string>();


    public struct Skill
    {
        public string id;
        public string type;
        public string weapon;
    }

    private void Start()
    {
        if (SharedVariables.JS == null)
        {
            JS = gameObject.GetComponent<main>();
            SharedVariables.JS = JS;
        }
        JS = SharedVariables.JS;
        if (SharedVariables.uIcontroller == null)
        {
            uIcontroller = gameObject.GetComponent<UIcontroller>();
            SharedVariables.uIcontroller = uIcontroller;
        }
        uIcontroller = SharedVariables.uIcontroller;

        Button but = CheckBut.GetComponent<Button>();
        but.onClick.AddListener(ShiMen_Check_main);

        string NoticeID = PlayerPrefs.GetString("NoticeID", "");
        if(NoticeID != "")
        {
            var temp = NoticeID.Split(";");
            for(int i=0;i<temp.Length;i++)
            {
                NoticeController.idList.Add(int.Parse(temp[i]));
            }
        }

        Application.logMessageReceived += HandleLog;
    }

    private void Update()
    {
        if(TaskQueue.Count != 0)
        {
            string line = TaskQueue.Dequeue();

            if(line.Substring(0,5) == "<001>")
            {
                ShiMenInf.GetComponent<TMP_Text>().text = "师门已经全部完成！";
                ShiMen_isOn = false;
                StartButton.GetComponent<Button>().interactable = false;
                CheckBut.GetComponent<Button>().interactable = true;
                StartButton.GetComponentInChildren<TMP_Text>().text = "<--------";
            }
            if(line.Substring(0,5) == "<002>")
            {
                ShiMenInf.GetComponent<TMP_Text>().text = line.Substring(5);
                    
            }
        }
    }

    private void ShiMen_Check_main()
    {
        StartCoroutine(ShiMen_Check());
    }

    public void IsSkill()
    {
        isSkill_bool = isSkill.GetComponent<Toggle>().isOn;
    }

    public void IsCmd()
    {
        if(isCmd.GetComponent<Toggle>().isOn)
        {
            isCmd_bool = true;
        }
        else
        {
            isCmd_bool = false;
        }
    }


    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            // 将错误消息附加到字符串中，可以使用换行符分隔不同的错误
            errorLogStringBuilder.AppendLine(logString);
            errorLogStringBuilder.AppendLine(stackTrace);
        }
    }

    public void ShiMen()
    {
        
        if(StartButton.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            StartButton.GetComponentInChildren<TMP_Text>().text = "结束";
            ShiMen_isOn = true;
            ShiMen_StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            ShiMen_Number = 0;
            int.TryParse(GameObject.Find("DelayInput").GetComponent<TMP_InputField>().text,out ShiMen_delay);
            if (ShiMen_delay < 300 && ShiMen_delay != 0)
                ShiMen_delay = 300;
            if (ShiMen_delay > 3500)
                ShiMen_delay = 3500;
            Thread ShiMen_Task = new Thread(ShiMen_Main);
            ShiMen_Task.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            ShiMen_isOn = false;
            StartButton.GetComponent<Button>().interactable = false;
            GameObject.Find("InputReady").GetComponent<CanvasGroup>().interactable = true;
            CheckBut.GetComponent<Button>().interactable = true;
            StartButton.GetComponentInChildren<TMP_Text>().text = "<--------";
        }
    }

    

    IEnumerator ShiMen_Check()
    {
        string res = "";
        GameObject.Find("InputReady").GetComponent<CanvasGroup>().interactable = false;
        CheckBut.GetComponentInChildren<TMP_Text>().text = "正在检测..";
        CheckBut.GetComponent<Button>().interactable = false;
        Cmds = GameObject.Find("CmdInput").GetComponent<TMP_InputField>().text;
        ShiMen_RTT = int.Parse(GameObject.Find("RTTInput").GetComponent<TMP_InputField>().text);
        if (ShiMen_RTT < 150)
            ShiMen_RTT = 150;
        else if (ShiMen_RTT > 800)
            ShiMen_RTT = 800;
        Cmds_num = int.Parse(GameObject.Find("ShiMenNum").GetComponent<TMP_InputField>().text);
        SkillName = GameObject.Find("SkillInput").GetComponent<TMP_InputField>().text;
        SharedVariables.Out = "";
        JS.SendMsg("bei\nquest\ni");
        yield return new WaitForSeconds(0.5f);
        Debug.Log(SharedVariables.Out);
        if(!int.TryParse(TextGainCenter("本周还可领取师门任务次数：", "次", SharedVariables.Out),out ShiMen_Number_left))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "请不要在领任务后点击本按钮！";
            CheckBut.GetComponentInChildren<TMP_Text>().text = "状态检测";
            CheckBut.GetComponent<Button>().interactable = true;
            GameObject.Find("InputReady").GetComponent<CanvasGroup>().interactable = true;
            yield break;
        }
        if(ShiMen_Number_left == 0)
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "请不要在任务全部完成后后点击本按钮！";
            CheckBut.GetComponentInChildren<TMP_Text>().text = "状态检测";
            CheckBut.GetComponent<Button>().interactable = true;
            GameObject.Find("InputReady").GetComponent<CanvasGroup>().interactable = true;
            yield break;
        }
        var beiSkill = Regex.Match(SharedVariables.Out, "以下是你目前组合中的特殊拳术技能。(.*?)%SV%师长",RegexOptions.Singleline);
        if (beiSkill.Success)
        {
            string bei = beiSkill.Groups[1].Value.Replace("%SV%", "");
            bei = bei.Trim();
            res = "准备技能: " + bei;
        }
        else
            res = "你还没有准备技能。";
        //扫描背包
        string id_ = TextGainCenter("inventory 0 ", "$zj#下一页", SharedVariables.Hudong_Buttons);
        int inventoryPages = 1;
        string allText = "";
        if(id_ == "")
            allText = SharedVariables.Hudong_Buttons;
        else
        {
            while (true)
            {
                JS.SendMsg("inventory " + inventoryPages + " " + id_);
                yield return new WaitForSeconds(0.5f);
                int pages = int.Parse(TextGainCenter("下一页:inventory ", " " + id_, SharedVariables.Hudong_Buttons));
                if (inventoryPages == pages)
                    break;
                inventoryPages++;
                allText += SharedVariables.Hudong_Buttons;
            }
        }
        var pans = Regex.Matches(allText, "(\\d*?)个\\[1;37m超级八卦盘");
        var fus = Regex.Matches(allText, "(\\d*?)个\\[1;32m师门扫荡符");
        int numPans = 0;
        int numfus = 0;
        if(pans.Count > 0)
        {
            for (int i = 0; i < pans.Count; i++)
                numPans += int.Parse(pans[i].Groups[1].Value);
        }
        if (fus.Count > 0)
        {
            for (int i = 0; i < fus.Count; i++)
                numfus += int.Parse(fus[i].Groups[1].Value);
        }
        if (numPans == 0)
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "您的背包里没有 超级八卦盘 ！";
            CheckBut.GetComponentInChildren<TMP_Text>().text = "状态检测";
            CheckBut.GetComponent<Button>().interactable = true;
            GameObject.Find("InputReady").GetComponent<CanvasGroup>().interactable = true;
            yield break;
        }
        res += "\n你的八卦盘还有: " + numPans + " 个。\n";
        if (numfus == 0)
            res += "你没有扫荡符可能无法跳过任务。\n";
        else
            res += "你的扫荡符还剩 " + numfus + " 个。\n";
        res += "您已经符合要求了！请<color=red>领取师门任务</color>以后再点击开始！";
        ShiMenInf.GetComponent<TMP_Text>().text = res;
        StartButton.GetComponent<Button>().interactable = true;
        CheckBut.GetComponentInChildren<TMP_Text>().text = "状态检测";
        StartButton.GetComponentInChildren<TMP_Text>().text = "开始";

    }


    public void ShiMen_Main()
    {
        if (ShiMen_isOn == false)
            return;
        SharedVariables.Out = "";
        string ShiMen_Name, ShiMen_ID;
        JS.SendMsg("exert recover\nexert regenerate\naccept quest\nquest\nuse ba gua");
        Thread.Sleep(ShiMen_RTT);
        string str = SharedVariables.Out;
        if (str.Contains("无法占卜到目标人物"))
        {
            if (ShiMen_AutoApply)
            {
                JS.SendMsg("use saodangfu\nfly yz\ns\ns\nw\nbuy 1 枸杞茶\njingcha\nshimen_ask");
                Thread.Sleep(1000);
                ShiMen_Main();
                return;
            }
            else
            {
                uIcontroller.ShowInf("[1;36m你的师门怪被别人杀死了，任务停止。[0;0m");
                return;
                
            }
        }
        if (!str.Contains("使用八卦盘占得在户外"))
        {
            var match = Regex.Match(str, "之前割下(.*?)\\((.*?)\\)的人头");
            if(match.Success && match.Groups[2].Value != "")
            {
                ShiMen_Name = match.Groups[1].Value;
                var match_in = Regex.Matches(SharedVariables.Obj, ShiMen_Name + ":look (.*?)%SV%");
                if (match_in.Count > 0)
                {
                    ShiMen_ID = match_in[0].Groups[1].Value;
                    if (match_in.Count > 1)
                    {
                        for(int i=0;i<match_in.Count;i++)
                        {
                            if(match_in[i].Groups[1].Value.Contains("/kungfu/class/generate/killed"))
                            {
                                ShiMen_ID = match_in[i].Groups[1].Value;
                                break;
                            }
                        }
                    }
                    if (ShiMen_delay != 0)
                        Thread.Sleep(ShiMen_delay);
                    JS.SendMsg("kill " + ShiMen_ID);
                    ShiMen_isFight = true;
                    if (isSkill_bool)
                    {
                        Thread doSkill = new Thread(DoSkill);
                        doSkill.Start();
                    }
                    Thread.Sleep(150);
                    ObjScanner(ShiMen_Name);
                }
                else
                {
                    Thread.Sleep(300);
                    ShiMen_Main();
                }
            }
            else
            {
                Thread.Sleep(300);
                ShiMen_Main();
            }
        }
        else
            GetOut();
    }


    public void DoSkill()
    {
        while(ShiMen_isFight)
        {
            Thread.Sleep(500);
            JS.SendMsg(SkillName);
        }
    }

   

    public void GetOut()
    {
        string here = SharedVariables.Here;
        if(ShiMen_north.Contains(here))
            JS.SendMsg("north");
        else if(ShiMen_south.Contains(here))
            JS.SendMsg("south");
        else if (ShiMen_west.Contains(here))
            JS.SendMsg("west");
        else if (ShiMen_east.Contains(here))
            JS.SendMsg("east");
        Thread.Sleep(300);
        if (SharedVariables.Here != here)
        {
            ShiMen_Main();
            return;
        }
        if (here == "青羊宫")
            JS.SendMsg("northeast");
        else if (here == "玄坛庙")
            JS.SendMsg("southwest");
        else if (here == "中岳大殿")
            JS.SendMsg("northup");
        else if (here == "兵器铺")
            JS.SendMsg("southeast\nwest");
        else if (here == "民房")
            JS.SendMsg("northeast");
        else if (here == "竹楼")
            JS.SendMsg("down");
        else if (here == "宏圣寺塔" || here == "塔基")
            JS.SendMsg("d\nd\nout\nw");
        else if (here == "戈壁滩")
            JS.SendMsg("n\nw");
        else if (here == "祭祀大屋" || here == "大厅" || here == "山神庙")
            JS.SendMsg("out");
        else if (here == "祭祀屋")
            JS.SendMsg("d\nd\nout");
        else if (here == "议事堂")
            JS.SendMsg("down");
        Thread.Sleep(300);
        if(SharedVariables.Here != here)
        {
            ShiMen_Main();
        }
        else
        {
            AutoWayFinding();
            
        }

    }


    public void AutoWayFinding()
    {
        JS.SendMsg("look");
        Thread.Sleep(300);
        var temp = SharedVariables.Direction.Split("  ");
        System.Random r = new System.Random();
        int i = r.Next(0,temp.Length);
        JS.SendMsg(temp[i]);
        Thread.Sleep(300);
        ShiMen_Main();
        
    }

    
    public void ObjScanner(string ShiMen_Name)
    {
        while(true)
        {
            if(SharedVariables.Obj.IndexOf(ShiMen_Name) == -1)
            {
                ShiMen_isFight = false;
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                ShiMen_Main();
                break;
            }
            if(SharedVariables.Obj.IndexOf(ShiMen_Name+"的尸体") != -1)
            {
                ShiMen_isFight = false;
                int i = 0;
                ShiMen_Counter();
                while (i < 40)
                {
                    if (SharedVariables.Out.Contains("确认或者拒绝"))
                        break;
                    i++;
                    Thread.Sleep(100);
                }
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                ShiMen_Main();
                break;
            }
            Thread.Sleep(100);
        }
    }

    public void ShiMen_Counter()
    {
        ShiMen_Number++;
        ShiMen_Number_left--;
        long nowTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        if( isCmd_bool && ShiMen_Number % Cmds_num == 0)
        {
            JS.SendMsg(Cmds);
        }
        float speed = (float)ShiMen_Number / (float)(nowTime - ShiMen_StartTime) * 60;
        float time_left = (float)ShiMen_Number_left / speed;
        
        if(ShiMen_Number_left == 0)
        {
            TaskQueue.Enqueue("<001>");
            return;
        }
        
        string temp = "已完成 " + ShiMen_Number+ "  个师门\n还剩 " + ShiMen_Number_left
            + "个师门\n师门速度 " + Math.Round(speed, 1)
            + " 个/分钟\n全部完成还需 " + Math.Round(time_left) + " 分钟";
        TaskQueue.Enqueue("<002>" + temp);
        
    }

    public void SavePotential()
    {
        JS.SendMsg("qncunru " + SharedVariables.Potential.ToString() + " qn");
    }

    public void BoDong1()
    {
        GameObject e = GameObject.Find("BoDongStart1");
        if(e.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            if (BoDong_isOn)
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            BoDong1_Potential = int.Parse(GameObject.Find("potential1").GetComponent<TMP_InputField>().text);
            newBodong1 = new Thread(BoDong1_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "结束";
            newBodong1.Start();
            uIcontroller.ShowInf("[1;36m开始模式一提升技能！！![0;0m");
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "开始";
            BoDong_isOn = false;
            newBodong1.Abort();
        }
        
    }

    public void BoDong1_Main()
    {
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            int left = BoDong1_Potential - (int)SharedVariables.Potential + 1;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            while(SharedVariables.Potential<BoDong1_Potential)
            {
                Thread.Sleep(300);
                overTime += 0.3f;
                if (overTime > 10)
                    break;
            }
            if (overTime > 10) continue;
            while (true)
            {
                string n = "research " + BoDong_Skill + " 1\n";
                SharedVariables.Out = "";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                string temp = SharedVariables.Out;
                if(temp.Contains("恭喜："))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m成功提升" + (i + 1).ToString() + "级技能！！！[0;0m");
            Thread.Sleep(1000);
        }
        BoDong_isOn = false;
    }

    public void BoDong2()
    {
        
        GameObject e = GameObject.Find("BoDongStart2");
        if (e.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            if (BoDong_isOn)
                return;
            BoDong_isGetSecond = GameObject.Find("IsGetSecond").GetComponent<Toggle>().isOn;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            BoDong2_Speed = GameObject.Find("BoDongSpeedDropdown").GetComponent<TMP_Dropdown>().value;
            Thread newBoDong2 = new Thread(BoDong2_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "结束";
            newBoDong2.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "开始";
            BoDong_isOn = false;
        }
        
    }

    public void BoDong2_Main()
    {
        uIcontroller.ShowInf("[1;36m开始模式二提升技能！！![0;0m");
        int timeCount = 5 - BoDong2_Speed;
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            List<long> record = new List<long>();
            uIcontroller.ShowInf("[1;36m正在进行多次寻找。。。[0;0m");
            JS.SendMsg("qntiqu 1 qn");
            Thread.Sleep(300);
            for (int j=0;j<timeCount;j++)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                MatchCollection a = Regex.Matches(SharedVariables.Out, "要潜能.*?点。");
                for(int k=0;k<a.Count;k++)
                {
                    long temp = long.Parse(TextGainCenter("要潜能", "点。", a[k].Value));
                    record.Add(temp);
                }

            }
            if (record.Count < 2)
                continue;
            record.Sort();
            long potential;
            if (BoDong_isGetSecond)
                potential = record[1];
            else
                potential = record[0];
            long left = potential - SharedVariables.Potential;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            uIcontroller.ShowInf("[1;36m正在尝试以最小潜能研究。。。[0;0m");
            while (SharedVariables.Potential < potential)
            {
                Thread.Sleep(300);
                overTime += 0.3f;
                if (overTime > 10)
                    break;
            }
            if (overTime > 10) continue;
            while (true)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                string temp = SharedVariables.Out;
                if (temp.Contains("恭喜："))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m成功提升" + (i + 1).ToString() + "级技能！！！[0;0m");
            record.Clear();
            Thread.Sleep(1000);
        }
        BoDong_isOn = false;
    }

    public void LianGong_AddSkill()
    {
        GameObject SkillInput = GameObject.Find("LianGongSkillInput");
        GameObject WeaponInput = GameObject.Find("LianGongWeaponInput");
        GameObject TypeDropdown = GameObject.Find("LianGongTypeDropdown");
        
        Skill newSkill;
        newSkill.id = SkillInput.GetComponent<TMP_InputField>().text;
        string temp_text = TypeDropdown.GetComponent<TMP_Dropdown>().captionText.text;
        newSkill.type = temp_text.Substring(4);
        newSkill.weapon = WeaponInput.GetComponent<TMP_InputField>().text;
        if (newSkill.id == "" || newSkill.weapon == "")
            return;
        LianGongList.Add(newSkill);
        string inf = newSkill.id + "--" + newSkill.type + "--" + newSkill.weapon;
        GameObject tempObj = Instantiate(LianGongListItem, LianGongContent.transform);
        tempObj.GetComponent<Toggle>().group = LianGongContent.GetComponent<ToggleGroup>();
        tempObj.GetComponentInChildren<TMP_Text>().text = inf;
    }

    public void LianGong_DelSkill()
    {
        Toggle temp = LianGongContent.GetComponent<ToggleGroup>().GetFirstActiveToggle();
        if (temp == null)
            return;
        string selectedText = temp.gameObject.GetComponentInChildren<TMP_Text>().text;
        var dir = selectedText.Split("--");
        if (dir.Length < 3) return;
        Skill newSkill;
        newSkill.id = dir[0];
        newSkill.type = dir[1];
        newSkill.weapon = dir[2];
        LianGongList.Remove(newSkill);
        Destroy(temp.gameObject);
    }

    public void LianGong_ChangeType()
    {
        GameObject WeaponInput = GameObject.Find("LianGongWeaponInput");
        GameObject TypeDropdown = GameObject.Find("LianGongTypeDropdown");
        if (TypeDropdown.GetComponent<TMP_Dropdown>().value > 8)
        {
            WeaponInput.GetComponent<TMP_InputField>().text = "null";
            WeaponInput.GetComponent<TMP_InputField>().interactable = false;
        }
        else if(TypeDropdown.GetComponent<TMP_Dropdown>().value < 7)
        {
            WeaponInput.GetComponent<TMP_InputField>().interactable = true;
            WeaponInput.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            WeaponInput.GetComponent<TMP_InputField>().interactable = true;
            WeaponInput.GetComponent<TMP_InputField>().text = "null";
        }
    }

    public void LianGong_Save()
    {
        GameObject SettingDropdown = GameObject.Find("LianGongSettingDropdown");
        int index = SettingDropdown.GetComponent<TMP_Dropdown>().value;
        string res = "";
        for(int i=0;i<LianGongList.Count;i++)
        {
            Skill temp = LianGongList[i];
            res += temp.id + "--" + temp.type + "--" + temp.weapon + "\n";
        }
        PlayerPrefs.SetString("LianGongSetting" + index.ToString(), res);

    }

    public void LianGong_Load()
    {
        uIcontroller.DestroyAllObjectsInChildren(LianGongContent);
        LianGongList.Clear();
        GameObject SettingDropdown = GameObject.Find("LianGongSettingDropdown");
        int index = SettingDropdown.GetComponent<TMP_Dropdown>().value;
        string temp = PlayerPrefs.GetString("LianGongSetting" + index.ToString(), "");
        var dir = temp.Split("\n");
        for(int i=0;i<dir.Length;i++)
        {
            var cache = dir[i].Split("--");
            if (cache.Length < 3) continue;
            Skill newSkill;
            newSkill.id = cache[0];
            newSkill.type = cache[1];
            newSkill.weapon = cache[2];
            LianGongList.Add(newSkill);
            string inf = dir[i];
            GameObject tempObj = Instantiate(LianGongListItem, LianGongContent.transform);
            tempObj.GetComponent<Toggle>().group = LianGongContent.GetComponent<ToggleGroup>();
            tempObj.GetComponentInChildren<TMP_Text>().text = inf;

        }
    }

    public void LianGong1()
    {
        GameObject Mode1StartButton = GameObject.Find("LianGongMode1StartButton");
        GameObject SettingArea = GameObject.Find("LianGongSettingArea");
        if (Mode1StartButton.GetComponentInChildren<TMP_Text>().text == "开始练功")
        {
            if (LianGong_isOn)
                return;
            TenGodID = GameObject.Find("LianGongTenGodInput").GetComponent<TMP_InputField>().text;
            if (TenGodID == "")
                return;
            if (LianGongList.Count == 0)
                return;
            
            SettingArea.GetComponent<CanvasGroup>().interactable = false;
            Mode1StartButton.GetComponentInChildren<TMP_Text>().text = "结束练功";
            LianGong_isOn = true;
            LianGongThread = new Thread(LianGong1_Main);
            LianGongThread.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            SettingArea.GetComponent<CanvasGroup>().interactable = true;
            Mode1StartButton.GetComponentInChildren<TMP_Text>().text = "开始练功";
            LianGong_isOn = false;
        }
    }

    private void LianGong1_Main()
    {
        int skillNum = LianGongList.Count;
        string weaponNow = "";
        uIcontroller.ShowInf("[1;36m开始模式一自动练习技能！！![0;0m");
        List<string> weaponList = new List<string>();
        for(int i=0;i<skillNum;i++)
        {
            string item_temp = LianGongList[i].weapon;
            if (item_temp != "null")
            {
                if (!weaponList.Contains(item_temp))
                    weaponList.Add(item_temp);
            }
        }
        string cmds = "";
        if(weaponList.Count != 0)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                cmds += "drop " + weaponList[i] + "\n";
            }
            JS.SendMsg(cmds.Trim('\n'));
        }

        for (int i=0;i<skillNum;i++)
        {
            string id = LianGongList[i].id;
            string weapon = LianGongList[i].weapon;
            string type = LianGongList[i].type;
            if(weapon != "null")
            {
                JS.SendMsg("summon " + weapon);
                weaponNow = weapon;
            }
            else if(weaponNow != "null")
            {
                JS.SendMsg("drop " + weaponNow);
                weaponNow = "null";
            }
            Thread.Sleep(2000);
            JS.SendMsg("jifa " + type + " " + id);
            string cmd1 = "lian " + type + " 9999";
            string cmd2 = "touch " + TenGodID;
            uIcontroller.ShowInf("[1;36m开始练习技能 " + id + " [0;0m");
            while (LianGong_isOn)
            {
                SharedVariables.Out = "";
                JS.SendMsg(cmd1);
                Thread.Sleep(500);
                JS.SendMsg(cmd2);
                Thread.Sleep(500);
                JS.SendMsg("exert recover");
                Thread.Sleep(500);
                if(SharedVariables.Out.Contains("火候不够，难以继续提升你"))
                {
                    uIcontroller.ShowInf("[1;36m技能 " + id + " 已经练习完毕，准备练习下一技能...[0;0m");
                    break;
                }
            }
            if (!LianGong_isOn)
                break;

        }
        uIcontroller.ShowInf("[1;36m所有技能全部完成，脚本关闭！[0;0m");
        LianGong_isOn = false;
    }

    public void LianGong_SpeedChange()
    {
        GameObject SpeedSlider = GameObject.Find("LianGongSpeedSlider");
        LianGong_Speed = SpeedSlider.GetComponent<Slider>().value;
        SpeedSlider.GetComponentInChildren<TMP_Text>().text = LianGong_Speed.ToString();
    }

    public void LianGong2()
    {
        GameObject Mode2StartButton = GameObject.Find("LianGongMode2StartButton");
        GameObject SettingArea = GameObject.Find("LianGongSettingArea");
        if (Mode2StartButton.GetComponentInChildren<TMP_Text>().text == "开始练功")
        {
            if (LianGong_isOn)
                return;
            if (LianGongList.Count == 0)
                return;

            SettingArea.GetComponent<CanvasGroup>().interactable = false;
            Mode2StartButton.GetComponentInChildren<TMP_Text>().text = "结束练功";
            LianGong_isOn = true;
            LianGongThread = new Thread(LianGong2_Main);
            LianGongThread.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            SettingArea.GetComponent<CanvasGroup>().interactable = true;
            Mode2StartButton.GetComponentInChildren<TMP_Text>().text = "开始练功";
            LianGong_isOn = false;
        }
    }

    private void LianGong2_Main()
    {
        int skillNum = LianGongList.Count;
        string weaponNow = "";
        float a = SharedVariables.HP_MAX * LianGong_Speed / 100;
        string blood = ((int)a).ToString();
        uIcontroller.ShowInf("[1;36m开始模式二自动练习技能！！![0;0m");

        List<string> weaponList = new List<string>();
        for (int i = 0; i < skillNum; i++)
        {
            string item_temp = LianGongList[i].weapon;
            if (item_temp != "null")
            {
                if (!weaponList.Contains(item_temp))
                    weaponList.Add(item_temp);
            }
        }
        string cmd = "";
        if (weaponList.Count != 0)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                cmd += "drop " + weaponList[i] + "\n";
            }
            JS.SendMsg(cmd.Trim('\n'));
        }


        for (int i = 0; i < skillNum; i++)
        {
            string id = LianGongList[i].id;
            string weapon = LianGongList[i].weapon;
            string type = LianGongList[i].type;
            if (weapon != "null")
            {
                JS.SendMsg("summon " + weapon);
                weaponNow = weapon;
            }
            else if (weaponNow != "null")
            {
                JS.SendMsg("drop " + weaponNow);
                weaponNow = "null";
            }
            Thread.Sleep(2000);
            string cmds = "\nalias 自练功 exert recover;lian " + type + " 9999;lian " + type + " 9999;dazuo " + blood;
            JS.SendMsg("jifa " + type + " " + id + cmds);
            Thread.Sleep(500);
            JS.SendMsg("set sign5 自练功\nset sign1 1");
            uIcontroller.ShowInf("[1;36m开始练习技能 " + id + " [0;0m");
            int c_flag = 0;
            SharedVariables.Out = "";
            while (LianGong_isOn)
            {
                c_flag++;
                Thread.Sleep(1000);
                string tempText = SharedVariables.Out;
                SharedVariables.Out = "";
                var matches = Regex.Match(tempText, "你试着把(.*?)练了(.*?)趟");
                if (matches.Success)
                {
                    if (matches.Groups[2].Value == "零")
                    {
                        if (c_flag > 10)
                        {
                            uIcontroller.ShowInf("[1;35m技能 " + matches.Groups[1].Value + " 已经练习完毕，准备练习下一技能...[0;0m");
                            break;
                        }
                    }
                    else
                    {
                        uIcontroller.ShowInf("[1;35m技能 " + matches.Groups[1].Value + " 正在机器人练习中...[0;0m");
                        c_flag = 0;
                    }

                }
                else
                {
                    if (c_flag > 10)
                    {
                        uIcontroller.ShowInf("[1;35m技能 " + id + " 已经练习完毕，准备练习下一技能...[0;0m");
                        break;
                    }
                }
            }
            if (!LianGong_isOn)
                break;

        }
        JS.SendMsg("halt\nhalt");
        uIcontroller.ShowInf("[1;35m所有技能全部完成，脚本关闭！[0;0m");
        LianGong_isOn = false;
    }
    //private string[] GetAllIDInRoom()
    //{
    //    string temp = SharedVariables.Obj;
    //    var temp1 = temp.Split("%SV%");
    //    string[] res = new string[temp1.Length-1];
    //    for(int i=0;i<res.Length;i++)
    //    {
    //        var a = temp1[i].Split(":");
    //        res[i] = a[1].Substring(5);
            
    //    }
    //    return res;
    //}

    //private void killAll()
    //{
    //    string[] enemy = GetAllIDInRoom();
    //    string msg = "";
    //    for (int i = 0; i < enemy.Length; i++)
    //    {
    //        if(enemy[i] != "" && enemy[i] != " ")
    //            msg += "kill " + enemy[i] + "\n";
    //    }
    //    msg = msg.Substring(0, msg.Length - 1);
    //    JS.SendMsg(msg);
    //}



    public void YuLuoSha()
    {
        TMP_Text temp = GameObject.Find("YuLuoShaButton").GetComponentInChildren<TMP_Text>();
        if(temp.text == "玉罗刹开")
        {
            temp.text = "玉罗刹关";
            FuBen = null;
            FuBen = new Thread(YuLuoSha_main);
            FuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "玉罗刹开";
            FuBen.Abort();
            uIcontroller.CloseScript();
        }
    }

    private void YuLuoSha_main()
    {
        while(true)
        {
            JS.SendMsg("fly guanwai\nn\nask xiu cai about 玉罗刹");
            Thread.Sleep(500);
            if (!OpenFuBen())
                continue;
            Thread.Sleep(500);
            while (!CheckDeathNumber(1))
                Thread.Sleep(500);
            JS.SendMsg("out\ndown\nw");
            Thread.Sleep(500);
        }
    }


    public void DuoBao()
    {
        TMP_Text temp = GameObject.Find("DuoBaoButton").GetComponentInChildren<TMP_Text>();
        if (temp.text == "准备夺宝")
        {
            temp.text = "结束夺宝";
            FuBen = null;
            FuBen = new Thread(DuoBao_wait);
            FuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "准备夺宝";
            uIcontroller.ShowInf("[1;36m终止夺宝[0;0m");
            FuBen.Abort();
        }
    }

    private void DuoBao_wait()
    {
        JS.SendMsg("fly gw\nn\nn");
        Thread.Sleep(500);
        uIcontroller.ShowInf("[1;36m正在等待夺宝开始，移动自动取消夺宝[0;0m");
        while (true)
        {
            if(!SharedVariables.Here.Contains("夺宝口"))
            {
                if(SharedVariables.Here.Contains("房间"))
                {
                    uIcontroller.ShowInf("[1;36m开始夺宝[0;0m");
                    DuoBao_main();
                    break;
                }
                else
                {
                    uIcontroller.ShowInf("[1;36m终止夺宝[0;0m");
                    break;
                }
            }
            Thread.Sleep(1000);
        }
    }

    //private void DuoBao_main()
    //{
    //    while(SharedVariables.Here.Contains("房间"))
    //    {
    //        var temp = SharedVariables.Direction.Split("  ");
    //        System.Random r = new System.Random();
    //        int i = r.Next(0, temp.Length);
    //        JS.SendMsg(temp[i] + "\nlook");
    //        Thread.Sleep(100);
    //        JS.SendMsg("wa");
    //        Thread.Sleep(500);
    //    }
    //}

    private void DuoBao_main()
    {
        JS.SendMsg("w\nw\nw\nw\nw\nw\nw\nw\nw\nw");
        Thread.Sleep(2000);
        JS.SendMsg("n\nn\nn\nn\nn\nn\nn\nn\nn\nn");


        int currentRow = 0;
        int currentCol = 0;
        int direction = 1; // 1: move right, -1: move left
        string cmd = "wa\ne\nwa";
        // Function to check if the coordinates are valid
        bool IsValidPosition(int col)
        {
            return col >= 0 && col < 10;
        }

        while(SharedVariables.Here.Contains("房间"))
        {

            currentCol += direction;

            if (!IsValidPosition(currentCol))
            {
                direction *= -1; // Change direction (move left if moving right, move right if moving left)

                if (direction == 1)
                    cmd = "wa\ne\nwa";
                else
                    cmd = "wa\nw\nwa";

                currentRow++; // Move to the next row
                if (currentRow == 10)
                    break;

                SharedVariables.Long = "";
                while (true)
                {
                    JS.SendMsg("s\nwa");
                    Thread.Sleep(300);
                    if (SharedVariables.Long != "")
                        break;
                }

                currentCol += direction; // Move to the first room of the next row
            }
            else
            {
                SharedVariables.Long = "";
                while (true)
                {
                    JS.SendMsg(cmd);
                    Thread.Sleep(300);
                    if (SharedVariables.Long != "")
                        break;
                }
            }

            if (SharedVariables.Out.Contains("留点汤"))
                break;
        }
        uIcontroller.ShowInf("[1;36m夺宝完成！[0;0m");
    }


    public void TuiChe()
    {
        TMP_Text temp = GameObject.Find("TuiCheButton").GetComponentInChildren<TMP_Text>();
        TMP_InputField input = GameObject.Find("TuiCheSpeedInput").GetComponent<TMP_InputField>();
        int.TryParse(input.text, out int delay);
        if (temp.text == "推车开")
        {
            if (!FuBen_isEnd)
                return;
            uIcontroller.ShowInf("[1;36m即将启动推车副本...[0;0m");
            temp.text = "推车关";
            FuBen = null;
            FuBen = new Thread(new ParameterizedThreadStart(TuiChe_main));
            FuBen_isOn = true;
            FuBen_isEnd = false;
            FuBen.Start(delay);
            uIcontroller.CloseScript();
        }
        else
        {
            temp.text = "推车开";
            FuBen_isOn = false;
            uIcontroller.ShowInf("[1;36m将在完成次轮推车后关闭推车脚本...[0;0m");

        }
    }
    
    private void TuiChe_main(object delay)
    {
        int a = (int)delay;
        uIcontroller.ShowInf("[1;36m正在进行推车脚本...[0;0m");
        JS.SendMsg("fly hangzhou\ns\ns\ns\nsw\nsw\nsw\nwestup\nwestup\nwestdown\nwestdown\nw\ns\ne\ne\ne\nask jian gong about 运货");
        Thread.Sleep(1000);
        int index = 0;
        while (true)
        {
            SharedVariables.Out = "";
            string cmd = "drive cart " + TuiChe_way[index];
            JS.SendMsg(cmd);
            Thread.Sleep(100);
            int times = 0;
            while(true)
            {
                if (SharedVariables.Out.Contains("你手忙脚乱的折腾了半天"))
                    break;
                if (SharedVariables.Out.Contains("你上一个动作还没有完成"))
                    break;
                if (SharedVariables.Out.Contains("你赶着大车到了"))
                {
                    index++;
                    break;
                }
                if (SharedVariables.Out.Contains("乱动什么"))
                    break;
                if (SharedVariables.Out.Contains("你现在正在和人家动手"))
                    break;
                if (times > 50)
                {
                    SharedVariables.Here = "";
                    while(SharedVariables.Here == "")
                    {
                        JS.SendMsg("look");
                        Thread.Sleep(1000);
                    }
                    if (SharedVariables.Out.Contains("你赶着大车到了"))
                        index++;
                    break;
                }
                times++;
                Thread.Sleep(100);
            }
            if(index == 49)
            {
                if (!FuBen_isOn)
                    break;
                uIcontroller.ShowInf("[1;36m完成一轮推车，即将开始下一轮推车...[0;0m");
                while (true)
                {
                    Thread.Sleep(2000);
                    if (SharedVariables.Out.Contains("你领到了一些工钱"))
                        break;
                    JS.SendMsg("s\nn");
                }
                JS.SendMsg("s\nw\nfly hangzhou\ns\ns\ns\nsw\nsw\nsw\nwestup\nwestup\nwestdown\nwestdown\nw\ns\ne\ne\ne\nask jian gong about 运货");
                Thread.Sleep(1000);
                index = 0;
            }
            Thread.Sleep(a);
        }
        FuBen_isEnd = true;
        uIcontroller.ShowInf("[1;36m推车脚本执行完毕！！！[0;0m");
    }


    private bool OpenFuBen()
    {
        string id = SharedVariables.ID;
        if (id == "")
            return false;
        JS.SendMsg("fuben");
        Thread.Sleep(1000);
        Match temp = Regex.Match(SharedVariables.Hudong_Buttons, "fuben " + id + "\\w*");
        if (!temp.Success)
            return false;
        JS.SendMsg(temp.Value);
        return true;


    }

    public void OpenNotice()
    {
        NoticeController NC = new NoticeController();
        bool a = GameObject.Find("ForeignerToggle").GetComponent<Toggle>().isOn;
        bool b = GameObject.Find("JiWuMingToggle").GetComponent<Toggle>().isOn;
        bool c = GameObject.Find("ZhangWuJiToggle").GetComponent<Toggle>().isOn;
        NC.OpenNotice(a, b, c);
        uIcontroller.ShowInf("[1;36m成功启动计划通知！[0;0m");
        uIcontroller.CloseScript();
    }

    public void CloseNotice()
    {
        NoticeController.CloseAllNotice();
        uIcontroller.ShowInf("[1;36m计划通知已全部关闭！[0;0m");
        uIcontroller.CloseScript();
    }

    public void ShareBug()
    {
        string error = errorLogStringBuilder.ToString();
        if (error != "")
        {
            GUIUtility.systemCopyBuffer = error;
            AGUIMisc.ShowToast("BUG已经复制到剪辑板中，请联系管理员！");
        }
        else
        {
            AGUIMisc.ShowToast("未出现BUG！");
        }
    }

    /// <summary>
    /// 判断当前场景的尸体个数
    /// </summary>
    /// <param name="name">目标名字</param>
    /// <param name="number">目标数量</param>
    /// <returns>是否死亡</returns>
    private bool CheckDeathNumber(int number)
    {
        if(Regex.Matches(SharedVariables.Obj,"的尸体").Count == number)
        {
            return true;
        }
        return false;
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

    public void FanPai()
    {
        GameObject temp = GameObject.Find("FanPaiButton");
        if (!FuBen_isEnd)
            return;
        uIcontroller.ShowInf("[1;36m即将启动翻牌副本...[0;0m");
        FuBen = null;
        FuBen = new Thread(FanPai_main);
        FuBen_isEnd = false;
        FuBen.Start();
        uIcontroller.CloseScript();
    }

    private void FanPai_main()
    {
        if(SharedVariables.Here != "东厅")
        {
            JS.SendMsg("fly yz\nw\nn\nn\ne");
        }
        JS.SendMsg("fan");
        SharedVariables.Hudong_Buttons = "";
        while (true)
        {
            if (SharedVariables.Hudong_Buttons != "")
                break;
            Thread.Sleep(100);
        }
        Dictionary<string, List<int>> tempPai = new Dictionary<string, List<int>>();
        for (int i=1;i<=19;i+=2)
        {
            SharedVariables.Hudong_Buttons = "";
            JS.SendMsg("fan " + i.ToString() + "\nfan " + (i + 1).ToString());
            Thread.Sleep(300);
            string str = SharedVariables.Hudong_Buttons;
            str = str.Substring(str.IndexOf('#') + 1);
            var step = str.Split("$zj#");
            if (!tempPai.ContainsKey(step[i - 1].Split(":fan")[0]))
            {
                tempPai[step[i - 1].Split(":fan")[0]] = new List<int>();
            }
            if (!tempPai.ContainsKey(step[i].Split(":fan")[0]))
            {
                tempPai[step[i].Split(":fan")[0]] = new List<int>();
            }
            tempPai[step[i - 1].Split(":fan")[0]].Add(i);
            tempPai[step[i].Split(":fan")[0]].Add(i + 1);
            SharedVariables.Hudong_Buttons = "";
            while (true)
            {
                if (SharedVariables.Hudong_Buttons != "")
                    break;
                Thread.Sleep(100);
            }
        }

        ICollection<string> keys = tempPai.Keys;
        int stage = 0;
        foreach(string key in keys)
        {
            string res;
            stage++;
            if (tempPai[key].Count != 2)
                continue;
            if (tempPai[key][0] % 2 == 1 && tempPai[key][1] == tempPai[key][0] + 1)
                continue;
            res = "fan " + tempPai[key][0] + "\nfan " + tempPai[key][1];
            JS.SendMsg(res);
            SharedVariables.Hudong_Buttons = "";
            int times = 0;
            while (stage != keys.Count())
            {
                times++;
                if (SharedVariables.Hudong_Buttons != "")
                {
                    string str = SharedVariables.Hudong_Buttons;
                    str = str.Substring(str.IndexOf('#') + 1);
                    var step = str.Split("$zj#");
                    if (step[tempPai[key][1] - 1].StartsWith('×'))
                        break;
                    else
                        SharedVariables.Hudong_Buttons = "";
                }
                Thread.Sleep(100);
                if (times > 40)
                    break;
            }
        }

        uIcontroller.ShowInf("[1;36m翻牌已完成！！！[0;0m");
        FuBen_isEnd = true;
    }

    public void DiaoYu()
    {
        TMP_Text temp = GameObject.Find("DiaoYuButton").GetComponentInChildren<TMP_Text>();
        if (temp.text == "钓鱼开")
        {
            if (!FuBen_isEnd)
                return;
            uIcontroller.ShowInf("[1;36m即将启动钓鱼脚本...[0;0m");
            temp.text = "钓鱼关";
            FuBen = null;
            FuBen = new Thread(DiaoYu_main);
            FuBen_isOn = true;
            FuBen_isEnd = false;
            FuBen.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            temp.text = "钓鱼开";
            FuBen_isOn = false;
            JS.SendMsg("halt");
            uIcontroller.ShowInf("[1;36m钓鱼脚本关闭完成...[0;0m");

        }
    }

    private void DiaoYu_main()
    {
        JS.SendMsg("fish");
        while (FuBen_isOn)
        {
            SharedVariables.Out = "";
            Thread.Sleep(1000);
            if (SharedVariables.Out.Contains("你可以现在拉杆"))
            {
                JS.SendMsg("draw gan\nfish");
            }
        }
        FuBen_isEnd = true;
    }


    public void ShanHe()
    {
        TMP_Text temp = GameObject.Find("ShanHeButton").GetComponentInChildren<TMP_Text>();
        if (temp.text == "挖山河图")
        {
            if (!FuBen_isEnd)
                return;
            Dictionary<string, object> jsonDict = MiniJSON.Json.Deserialize(PlayerPrefs.GetString("params")) as Dictionary<string, object>;
            if (jsonDict == null)
                return;
            ShanHePosition = jsonDict["ShanHe"] as Dictionary<string, object>;
            uIcontroller.ShowInf("[1;36m即将启动钓鱼脚本...[0;0m");
            temp.text = "结束";
            FuBen = null;
            FuBen = new Thread(ShanHe_main);
            FuBen_isOn = true;
            FuBen_isEnd = false;
            FuBen.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            temp.text = "挖山河图";
            FuBen_isOn = false;
            JS.SendMsg("halt");
            uIcontroller.ShowInf("[1;36m山河图脚本关闭完成...[0;0m");

        }
    }

    private void ShanHe_main()
    {
        int index = 1;
        while(FuBen_isOn)
        {
            SharedVariables.Out = "";
            SharedVariables.Hudong = "";
            JS.SendMsg("look shan he " + index.ToString());
            Thread.Sleep(500);
            if(SharedVariables.Out.Contains("你要看什么"))
            {
                uIcontroller.ShowInf("[1;36m山河图已经全部挖完了...[0;0m");
                FuBen_isEnd = true;
                break;
            }
            string position = TextGainCenter("作画之人曾在[1;33m", "[0;0m", SharedVariables.Hudong);
            if (position == "")
                continue;
            if (!ShanHePosition.ContainsKey(position))
            {
                index++;
                continue;
            }
            string move = ShanHePosition[position].ToString();
            var paths = move.Split("$zj#");
            bool success = false;
            for(int i=0;i<paths.Length;i++)
            {
                if (paths[i] == "")
                    continue;
                SharedVariables.Out = "";
                JS.SendMsg(paths[i] + "\nwabao");
                Thread.Sleep(500);
                if(SharedVariables.Out.Contains("在房间里不能乱跑"))
                {
                    GoOut();
                    Thread.Sleep(1000);
                    i--;
                    continue;
                }
                if (SharedVariables.Out.Contains("恭喜你，获得物品"))
                {
                    success = true;
                    break;
                }
                Thread.Sleep(1000);
            }
            if (!success)
                index++;
            Thread.Sleep(1000);
        }
    }

    public void GoOut()
    {
        JS.SendMsg("look");
        Thread.Sleep(300);
        var temp = SharedVariables.Direction.Split("  ");
        System.Random r = new System.Random();
        int i = r.Next(0, temp.Length);
        JS.SendMsg(temp[i]);
    }

    public void IsAutoApply()
    {
        ShiMen_AutoApply = GameObject.Find("isAutoApply").GetComponent<Toggle>().isOn;
    }



}




