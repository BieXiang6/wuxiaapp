using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Concurrent;
using DeadMosquito.AndroidGoodies;

public class UIcontroller : MonoBehaviour
{

    public GameObject Obj;
	public GameObject Chat;
	public GameObject Title;
	
	public GameObject DirGrid;
	public GameObject DirVertical;
	public GameObject Out;
	public GameObject Hudong;
	public GameObject Mycmds;
	public GameObject Script;
	public GameObject Enemy, LongText, FightContent, LongContent, LongView;
	public GameObject Long;


	public GameObject ShiMen;
	public GameObject BoDong;
	public GameObject Setting;
	public GameObject LianGong;
	public GameObject FuBen;
	public GameObject CmdTip;
	public GameObject Plugin;


	public Canvas Main;
	public Canvas Game;
	public Canvas WVC;
	public GameObject HudongView;

	public ConcurrentQueue<string> line = new ConcurrentQueue<string>();


	private GameObject dire, dirw, dirn, dirso, dirsw, dirse, dirnw, dirne, dircen;

	private Slider enemyHP_s, enemyJP_s, enemyPP_s, enemyEP_s, HP_B, JP_B, PP_B;


	private TMP_Text HP_Text, BT_Text;

	private string enemyName_Last;

	string _PUT_CMD_ = "";
	List<string[]> _PUT_FONTSTYLE_;
	string _PUT_FONTSIZE_ = "";


	TMPObjectPool TMP_Pool;
	public ButtonObjectPool Button_Pool;


	private float width_HD;
	private float height_HD;
	private float DirHeight;
	public int EmojiPage = 0;
	private bool EmojiChangeBlock = false;
	private bool isMoveCloseHudong = false;
	private bool isShowLong = false;
	private bool isForegroundServiceOpen = false;
	private int freshSpeed_global = 4;

	private bool isMap;
	private Coroutine tempSmall;

	private Dictionary<string, string[]> fontStyle;
	private ModScript Mod;
	private main JS;
	private ResourceLoader RL;

	private Dictionary<string, Action<string>> UIMap;


	private void Start()
    {
		Enemy.SetActive(true);
		dirnw = DirGrid.transform.GetChild(0).gameObject;
		dirn = DirGrid.transform.GetChild(1).gameObject;
		dirne = DirGrid.transform.GetChild(2).gameObject;
		dirw = DirGrid.transform.GetChild(3).gameObject;
		dircen = DirGrid.transform.GetChild(4).gameObject;
		dire = DirGrid.transform.GetChild(5).gameObject;
		dirsw = DirGrid.transform.GetChild(6).gameObject;
		dirso = DirGrid.transform.GetChild(7).gameObject;
		dirse = DirGrid.transform.GetChild(8).gameObject;
		SharedVariables.CmdTip = CmdTip;

		enemyHP_s = GameObject.Find("enemyHP").GetComponent<Slider>();
		enemyJP_s = GameObject.Find("enemyJP").GetComponent<Slider>();
		enemyEP_s = GameObject.Find("enemyEP").GetComponent<Slider>();
		enemyPP_s = GameObject.Find("enemyPP").GetComponent<Slider>();

		HP_Text = GameObject.Find("HP Text").GetComponent<TMP_Text>();
		BT_Text = GameObject.Find("Busy Text").GetComponent<TMP_Text>();
		

		HP_B = GameObject.Find("HP Bar").GetComponent<Slider>();
		PP_B = GameObject.Find("PP Bar").GetComponent<Slider>();
		JP_B = GameObject.Find("JP Bar").GetComponent<Slider>();

		width_HD = Hudong.GetComponent<RectTransform>().rect.width;
		height_HD = HudongView.GetComponent<RectTransform>().rect.height;

		DirHeight = DirVertical.GetComponent<RectTransform>().rect.height;
		Enemy.SetActive(false);

		
		RL = gameObject.GetComponent<ResourceLoader>();
		if(SharedVariables.JS == null)
        {
			JS = gameObject.GetComponent<main>();
			SharedVariables.JS = JS;
		}
		JS = SharedVariables.JS;

		if (SharedVariables.Mod == null)
		{
			Mod = gameObject.GetComponent<ModScript>();
			SharedVariables.Mod = Mod;
		}
		Mod = SharedVariables.Mod;

		if (SharedVariables.RL == null)
		{
			RL = gameObject.GetComponent<ResourceLoader>();
			SharedVariables.RL = RL;
		}
		RL = SharedVariables.RL;

		SharedVariables.WVC = WVC;
		SharedVariables.PC = gameObject.GetComponent<PluginsController>();

		TMP_Pool = new TMPObjectPool();
		Button_Pool = new ButtonObjectPool();
		_PUT_FONTSTYLE_ = new List<string[]>();

		freshSpeed_global = PlayerPrefs.GetInt("freshSpeed", 4);

		int iSL = PlayerPrefs.GetInt("isShowLong", 1);
		if (iSL == 1)
			isShowLong = true;
		else
			isShowLong = false;


		//所有色彩都经过柔和化处理以适应浅色app
		fontStyle = new Dictionary<string, string[]>
		{
			["30"] = new string[2] { "<color=#000000>", "</color>" },
			["31"] = new string[2] { "<color=#CC0000>", "</color>" },
			["32"] = new string[2] { "<color=#00CC00>", "</color>" },
			["33"] = new string[2] { "<color=#FF9900>", "</color>" },
			["34"] = new string[2] { "<color=#0000CC>", "</color>" },
			["35"] = new string[2] { "<color=#CC00CC>", "</color>" },
			["36"] = new string[2] { "<color=#00B3B3>", "</color>" },
			["37"] = new string[2] { "<color=#0088FA>", "</color>" },
			["1"] = new string[2] { "<b>", "</b>" },
			["3"] = new string[2] { "<i>", "</i>" },
			["4"] = new string[2] { "<u>", "</u>" },
			["5"] = new string[2] { "<color=#000000>", "</color>" },
			["7"] = new string[2] { "<color=#000000>", "</color>" },
			["8"] = new string[2] { "<alpha=0>", "</alpha>" },
			["40"] = new string[2] { "<mark=#00000088>", "</mark>" },
			["41"] = new string[2] { "<mark=#BB000088>", "</mark>" },
			["42"] = new string[2] { "<mark=#00BB0088>", "</mark>" },
			["43"] = new string[2] { "<mark=#FFFF0088>", "</mark>" },
			["44"] = new string[2] { "<mark=#0000BB88>", "</mark>" },
			["45"] = new string[2] { "<mark=#BB00BB88>", "</mark>" },
			["46"] = new string[2] { "<mark=#00BBBB88>", "</mark>" },
			["47"] = new string[2] { "<mark=#00000088>", "</mark>" },
		};

		UIMap = new Dictionary<string, Action<string>>()
		{
			{ "100", WriteToChat },
			{ "001", WriteInput },
			{ "002", ChangeHere },
			{ "003", WriteToEX },
			{ "903", RemoveEX },
			{ "006", WriteToMU },
			{ "004", WriteToLong },
			{ "005", WriteToObj },
			{ "905", RemoveObj },
			{ "007", WriteToHD },
			{ "008", WriteToAct },
			{ "009", WriteToAct },
			{ "011", WriteToMap },
			{ "012", WriteToHP },
			{ "013", WriteToHD },
			{ "014", JS.SendMsg },
			{ "015", WriteToSmall },
			{ "020", WriteToPop },
			{ "021", WriteToBar },
			{ "022", Debug.Log },
		};
		SharedVariables.pressedColor = new Color(200f / 255f, 200f / 255f, 200f / 255f);
		SharedVariables.originalColor = new Color(1, 1, 1);

		StartCoroutine(TaskList());

	}

    private IEnumerator TaskList()
    {
		while(true)
        {
			int count = line.Count;
			if (count > 0)
			{
				int num = count + 1;
				for (int i = 1; i < num; i++)
				{
					string cell;
					if (line.TryDequeue(out cell))
					{
						WriteToUI(cell);
					}
					else continue;
					
					if (i % freshSpeed_global == 0)
						yield return new WaitForEndOfFrame();
				}
			}
			else
				yield return new WaitForEndOfFrame();

			if (line == null)
				line = new ConcurrentQueue<string>();
			
		}
	}


	
	

    public void StrSelector(string str)
    {
		line.Enqueue(str);
		
	}


	public string Substr(string text,int start,int length)
    {
		if (text.Length < length)
			return "error";
		return text.Substring(start, length);
    }

	private void WriteToUI(string param)
    {
		string str = param;
		if (str.Length < 4)
			return;

		if (str.StartsWith("[0;0m"))
			str = str.Replace("[0;0m", "");

		if (Substr(str, 0, 8) == "0000007")
		{
			JS.StopRotate();
			Main.GetComponent<CanvasGroup>().alpha = 0;
			Main.GetComponent<Canvas>().sortingOrder = 0;
			Game.GetComponent<CanvasGroup>().alpha = 1;
			Game.GetComponent<Canvas>().sortingOrder = 1;

			Main.enabled = false;
			if (PlayerPrefs.GetInt("isOpenNotice", 0) == 1)
            {
				//SharedVariables.PN = new PushNotification();
				//SharedVariables.PN.SendNotice("这是江湖辅助", "登陆成功！");
				SetForegroundService(true);
			}

			SharedVariables.PC.LoadFromData();
			return;
		}

		if (Substr(str, 0, 8) == "0000008")
		{
			GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "所选区服没有人物,请重启app切换区服。";
			Invoke("CallStopRotate", 5);
			return;
		}

		string temp = Substr(str, 0, 4);
		if (UIMap.ContainsKey(temp))
			UIMap[temp](str.Substring(4));
		else
        {
			WriteToOut(str);
		}
	}

	private void CallStopRotate()
    {
		JS.StopRotate();
		Application.Quit();
	}

	private void WriteToSmall(string str)
    {
		if(tempSmall != null)
        {
			StopCoroutine(tempSmall);
			tempSmall = null;
        }
		tempSmall = StartCoroutine(ShowInfInSmall(str));

	}

	private IEnumerator ShowInfInSmall(string str)
    {
		CmdTip.SetActive(true);
		GameObject temp = CmdTip.transform.GetChild(0).gameObject;
		if (temp != null)
		{
			temp.GetComponent<TMP_Text>().text = ColorPut(str);
			yield return new WaitForEndOfFrame();
			float width = temp.GetComponent<RectTransform>().rect.width;
			CmdTip.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 50, 100);
		}
		yield return new WaitForSeconds(1.5f);
		CmdTip.SetActive(false);
    }

	


	private void WriteToLong(string str)
    {
		if (!isShowLong)
			return;

		if (str.IndexOf("气血.") != -1)
		{
			Enemy.SetActive(true);
			FightContent.GetComponent<CanvasGroup>().alpha = 1;
			LongContent.GetComponent<CanvasGroup>().alpha = 0;
			string temp = ClearFormat(str);
			string enemyName = temp.Substring(0, temp.IndexOf("|"));
			string enemyHP, enemyPP, enemyJP, enemyEP;
			string busyTime = "";
			if (!temp.Contains("<busy"))
			{
				enemyHP = TextGainCenter("气血.", "　内力.", temp);
				enemyPP = TextGainCenter("内力.", "　精神.", temp);
				enemyJP = TextGainCenter("　精神.", "　精力.", temp);
				enemyEP = temp.Substring(temp.IndexOf("精力.") + 3);
			}
			else
			{
				enemyHP = TextGainCenter("气血.", "　内力.", temp);
				enemyPP = TextGainCenter("内力.", "　精神.", temp);
				enemyJP = TextGainCenter("精神.", "<busy", temp);
				busyTime = TextGainCenter("<busy ", ">", temp);
				if (busyTime == "")
				{
					busyTime = "0";
				}
				enemyEP = "0";
			}
			int enemyHP_i, enemyPP_i, enemyJP_i, enemyEP_i;
			int.TryParse(enemyHP, out enemyHP_i);
			int.TryParse(enemyPP, out enemyPP_i);
			int.TryParse(enemyEP, out enemyEP_i);
			int.TryParse(enemyJP, out enemyJP_i);
			if (enemyName != enemyName_Last)
			{
				enemyName_Last = enemyName;


				enemyHP_s.maxValue = enemyHP_i;
				enemyJP_s.maxValue = enemyJP_i;
				enemyPP_s.maxValue = enemyPP_i;
				enemyEP_s.maxValue = enemyEP_i;
				enemyHP_s.value = enemyHP_i;
				enemyJP_s.value = enemyJP_i;
				enemyPP_s.value = enemyPP_i;
				enemyEP_s.value = enemyEP_i;
			}
			else
			{
				enemyHP_s.value = enemyHP_i;
				enemyJP_s.value = enemyJP_i;
				enemyPP_s.value = enemyPP_i;
				enemyEP_s.value = enemyEP_i;
			}

			if (busyTime != "")
			{
				GameObject.Find("enemyName").GetComponent<TMP_Text>().text = "敌人:" + enemyName + "\n忙碌：" + busyTime;
			}
			else
			{
				GameObject.Find("enemyName").GetComponent<TMP_Text>().text = "敌人:" + enemyName;
			}
			GameObject.Find("enemyText").GetComponent<TMP_Text>().text = "气血:" + enemyHP + "\n内力:" + enemyPP + "\n精神:" + enemyJP + "\n精力:" + enemyEP;

		}
		else
        {
			Enemy.SetActive(true);
			FightContent.GetComponent<CanvasGroup>().alpha = 0;
			LongContent.GetComponent<CanvasGroup>().alpha = 1;
			LongText.GetComponent<TMP_Text>().text = ColorPut(str);
			LayoutRebuilder.ForceRebuildLayoutImmediate(LongView.GetComponent<RectTransform>());
		}
	}


	private void WriteToEX(string str)
    {
		var strs = str.Split("$zj#");
		string idss = "";

		for (int i = 0;i<strs.Length;i++)
        {
			
			if (strs[i].Length < 2)
				continue;

			var dirs = strs[i].Split(":");
			if(dirs.Length==3)
            {
				idss =  dirs[2];
			}
            else 
			{
				idss = dirs[0] ;
			}
			dirs[1] = ColorPut(dirs[1]);
			if (dirs[0] == "east" || dirs[0] == "eastup" || dirs[0] == "eastdown")
			{
				dire.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dire.name = idss;
				dire.GetComponent<CanvasGroup>().alpha = 1;
				dire.GetComponent<CanvasGroup>().blocksRaycasts = true;

			}
			else if (dirs[0] == "west" || dirs[0] == "westup" || dirs[0] == "westdown")
			{

				dirw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirw.name = idss;
				dirw.GetComponent<CanvasGroup>().alpha = 1;
				dirw.GetComponent<CanvasGroup>().blocksRaycasts = true;

			}
			else if (dirs[0] == "north" || dirs[0] == "northup" || dirs[0] == "northdown")
			{

				dirn.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirn.name = idss;
				dirn.GetComponent<CanvasGroup>().alpha = 1;
				dirn.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else if (dirs[0] == "south" || dirs[0] == "southup" || dirs[0] == "southdown")
			{

				dirso.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirso.name = idss;
				dirso.GetComponent<CanvasGroup>().alpha = 1;
				dirso.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else if (dirs[0] == "southwest")
			{

				dirsw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirsw.name = idss;
				dirsw.GetComponent<CanvasGroup>().alpha = 1;
				dirsw.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else if (dirs[0] == "southeast")
			{

				dirse.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirse.name = idss;
				dirse.GetComponent<CanvasGroup>().alpha = 1;
				dirse.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else if (dirs[0] == "northwest")
			{

				dirnw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirnw.name = idss;
				dirnw.GetComponent<CanvasGroup>().alpha = 1;
				dirnw.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else if (dirs[0] == "northeast")
			{

				dirne.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirne.name = idss;
				dirne.GetComponent<CanvasGroup>().alpha = 1;
				dirne.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
			else
			{
				Button_Pool.GetObject(DirVertical.transform, dirs[1], idss, new Vector2(0 ,DirHeight / 5),30);
				if(DirVertical.transform.childCount>5)
                {
					DirVertical.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                }
				else
                {
					DirVertical.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
				}
			}

		}


	}


	private void RemoveEX(string str)
    {
		str = str.Replace("\r", "").Replace("\n", "");
		Transform temp = DirGrid.transform.Find(str);
		if (temp != null)
		{
			temp.GetComponent<Button>().interactable = false;
			temp.GetComponent<CanvasGroup>().alpha = 0;
		}
		else
        {
			temp = DirVertical.transform.Find(str);
			if (temp != null)
				Button_Pool.ReleaseObject(temp.gameObject);
		}
		
    }


	private void ChangeHere(string str)
    {
		ReleaseButtons(DirVertical);
		ReleaseButtons(Obj);
		Title.GetComponent<TMP_Text>().text = ColorPut(str);


		dire.GetComponent<CanvasGroup>().alpha = 0;
		dire.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirso.GetComponent<CanvasGroup>().alpha = 0;
		dirso.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirsw.GetComponent<CanvasGroup>().alpha = 0;
		dirsw.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirse.GetComponent<CanvasGroup>().alpha = 0;
		dirse.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirw.GetComponent<CanvasGroup>().alpha = 0;
		dirw.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirn.GetComponent<CanvasGroup>().alpha = 0;
		dirn.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirnw.GetComponent<CanvasGroup>().alpha = 0;
		dirnw.GetComponent<CanvasGroup>().blocksRaycasts = false;
		dirne.GetComponent<CanvasGroup>().alpha = 0;
		dirne.GetComponent<CanvasGroup>().blocksRaycasts = false;

		Mycmds.SetActive(false);
		CloseHudong();
		dircen.GetComponentInChildren<TMP_Text>().text = ColorPut(str);

		
	}


	private void WriteToObj(string str)
    {
		var strs = str.Split("$zj#");
		for (int i=0;i<strs.Length;i++)
        {
			var dirs = strs[i].Split(':');
			Button_Pool.GetObject(Obj.transform, ColorPut(dirs[0]), dirs[1], new Vector2(0, height_HD / 12),35);

		}

	}


	private void WriteToChat(string str)
    {
		string temp = ColorPut(str);
		temp = Regex.Replace(temp, "<e:", "<sprite=");

		TMP_Pool.GetObject(Chat.transform, temp, 35, new Vector2(0, 0));
		if (Chat.transform.childCount > 40)
		{
			TMP_Pool.ReleaseObject(Chat.transform.GetChild(0).gameObject);
		}

	}


	private void RemoveObj(string str)
    {
		GameObject a = GameObject.Find(str);
		if(!a)
        {
			str = str.Replace("\n", "");
			str = str.Replace("\r", "");
			a = GameObject.Find(str);
		}
		Button_Pool.ReleaseObject(a);

	}


	private void WriteToOut(string str)
    {
		
		str = str.Replace("$br#", "\n");
		TMP_Pool.GetObject(Out.transform,ColorPut(str),35, new Vector2(width_HD, 0));
		if(Out.transform.childCount>70)
        {
			TMP_Pool.ReleaseObject(Out.transform.GetChild(0).gameObject);
		}
		StartCoroutine(InsSrollBar());

	}

	IEnumerator InsSrollBar()
	{
		yield return new WaitForEndOfFrame();
		GameObject.Find("OutView").GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
	}


	private void WriteToHP(string str)
    {
		var strs = str.Split("║");
		if (strs.Length < 8)
			return;

		string[] oneStep = new string[8];

		for (int i = 1; i < 8; i++)
		{
			var res = strs[i].Split(":");
			if (res.Length < 2)
				continue;
			oneStep[i] = res[1];
		}

		var blood = oneStep[1].Split("/");
		if (blood.Length < 3)
			return;
		long.TryParse(blood[0], out long HP);
		long.TryParse(blood[2], out long HP_Max);
		long.TryParse(blood[1], out long HP_Real);
		HP_B.value = (float)HP / HP_Max;
		GameObject HP_bar = GameObject.Find("HP Bar");
		if (HP_bar == null)
			return;
		HP_bar.transform.GetChild(0).GetComponent<Slider>().value = (float)HP_Real / HP_Max;


		var force = oneStep[2].Split("/");
		if (force.Length < 2)
			return;
		long.TryParse(force[0], out long Force_now);
		long.TryParse(force[1], out long Force_Max);

		PP_B.value = (float)Force_now / Force_Max / 2;

		var will = oneStep[3].Split("/");
		long.TryParse(will[0], out long Will_Now);
		long.TryParse(will[2], out long Will_Max);
		long.TryParse(will[1], out long Will_Real);

		JP_B.value = (float)Will_Now / Will_Max;
		GameObject Will_bar = GameObject.Find("JP Bar");
		if (HP_bar == null)
			return;
		Will_bar.transform.GetChild(0).GetComponent<Slider>().value = (float)Will_Real / Will_Max;



		var potential = oneStep[6].Split("/")[0];


		HP_Text.text = "气：" + blood[0] + "\n内：" + force[0] + "\n精：" + will[0] + "\n潜：" + potential;


		if(strs[0].Contains("<busy"))
        {
			if(strs[0].Contains("<busy>"))
            {
				BT_Text.text = "忙碌中";
			}
			else
            {
				string a = Regex.Match(strs[0], "<busy .*?s>").Value;
				string temp = Regex.Replace(a, @"[^0-9]+","");
				int busyTime = int.Parse(temp);
				BT_Text.text = "忙碌 " + busyTime + "s";
				if (busyTime == 1)
                {
					BT_Text.text = "空闲中";
				}
            }
        }
		else
        {
			BT_Text.text = "空闲中";

		}

		
		

	}


	private void WriteToMap(string str)
    {
		GameObject dirce = RL.GetObject(RL.HudongText);
		dirce.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, height_HD / 20);
		dirce.GetComponent<TMP_Text>().fontSize = 30;
		dirce.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		HudongView.SetActive(true);
		Hudong.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		Hudong.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
		HudongView.GetComponent<ScrollRect>().horizontal = true;
		HudongView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Unrestricted;
		string strt = str.Replace("$br#", "\n");
		
		dirce.GetComponent<TMP_Text>().text = ColorPut(strt);
		dirce.transform.SetParent(Hudong.transform);
		isMap = true;
	}


	private void WriteToHD(string str)
    {
		isMoveCloseHudong = true;
		CloseHudong();
		HudongView.SetActive(true);
		if (isMap)
        {
			Hudong.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
			Hudong.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
			HudongView.GetComponent<ScrollRect>().horizontal = false;
			HudongView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
			isMap = false;
		}
		str = str.Replace("$br#", "\n");
		GameObject eeee = RL.GetObject(RL.HudongText);
		eeee.GetComponent<TMP_Text>().text = ColorPut(str);
		eeee.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, height_HD / 20);
		eeee.transform.SetParent(Hudong.transform, false);
	}


	private void WriteToAct(string str)
    {
		GameObject HudongButtons = RL.GetObject(RL.HudongButtons);
		HudongButtons.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, 0);
		HudongButtons.name = "HDButtons";
		int r = 3, w = 3, h = 10, s = 30;
		Regex strw = new Regex("\\$.*?\\#");
		string rs = strw.Match(str).Value;
		string[] ss = null;
		if(rs != "$zj#" && rs!="")
        {
			Regex strh = new Regex("[\\$,\\#]");
			ss = strh.Split(rs);
			
			if (ss.Length>4)
			{
				r = int.Parse(ss[1]);
				if (r == 1)
					r = 4;
				w = int.Parse(ss[2]);
				h = int.Parse(ss[3]);
				s = int.Parse(ss[4]);
			}
			str = str.Replace(rs, "");
		}
		var strs = str.Split("$zj#");
		HudongButtons.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width_HD / w-w*5, height_HD / h);
		HudongButtons.GetComponent<GridLayoutGroup>().spacing = new Vector2(5, 5);
		HudongButtons.transform.SetParent(Hudong.transform, false);
		for (var i = 0; i < (strs.Length / r + 1); i++)
		{
			for (var j = 0; j < r; j++)
			{
				if ((i * r + j) > (strs.Length - 1)) break;
				var dirs = strs[i * r + j].Split(':');
				if (dirs.Length < 2) continue;
				var hi = height_HD / h;
				var wi = width_HD / w;
				Button_Pool.GetObject(HudongButtons.transform, ColorPut(dirs[0].Replace("$br#", "\n")), dirs[1], new Vector2(wi, hi),35);
				
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(Hudong.GetComponent<RectTransform>());

	}


	private void WriteToBar(string str)
    {
		
		GameObject a = GameObject.Find("TitleBar");
		ReleaseButtons(a);

		float height = a.GetComponent<RectTransform>().rect.height;
		float weight = a.GetComponent<RectTransform>().rect.width;
		var strs = str.Split("$zj#");
		for (var i = 0; i < strs.Length; i++)
		{
			var dirs = strs[i].Split(':');
			if (dirs.Length < 2)
				continue;
			Button_Pool.GetObject(a.transform, ColorPut(dirs[0]), dirs[1], new Vector2(weight * 0.22f, height),30);
			
		}
	}


	private void WriteToMU(string str)
    {
		if (str == "")
		{
			ShowInf("空的快捷指令.");
			return;
		}
		str = Regex.Replace(str, "\\$br#", "\n");
		Mycmds.SetActive(true);
		int num = Mycmds.transform.childCount;
		int j = 0;
		for (int i = 0; i < num; i++)
		{
			var child = Mycmds.transform.GetChild(j);
			if (child.gameObject.name != "CloseCmds")
			{
				Button_Pool.ReleaseObject(child.gameObject);
			}
			else
			{
				Destroy(child.gameObject);
				j++;
			}
		}
		var strs = str.Split("$zj#");
		float width = Mycmds.GetComponent<RectTransform>().rect.width;
		float height = Mycmds.GetComponent<RectTransform>().rect.height;
		Mycmds.GetComponent<GridLayoutGroup>().cellSize = new Vector2((width - 25) / 6, height / 2);
		Mycmds.GetComponent<GridLayoutGroup>().spacing = new Vector2(2, 2);
		for (var i = 0; i < 11; i++)
		{
			if(i < strs.Length)
            {
				if (strs[i].Length < 2) continue;
				var dirs = strs[i].Split(':');
				Button_Pool.GetObject(Mycmds.transform, ColorPut(dirs[1]), dirs[2], 35);
			}
			else
            {
				Button_Pool.GetObject(Mycmds.transform, "空", "look", 35);
			}
			
			if (i == 4)
			{
				GameObject dddd = RL.GetObject(RL.ObjButton);
				dddd.GetComponent<Button>().onClick.AddListener(CloseMycmds);
				dddd.GetComponentInChildren<TMP_Text>().text = "关闭";
				dddd.name = "CloseCmds";
				dddd.transform.SetParent(Mycmds.transform, false);

			}
		}

	}


	public void WriteToPop(string str)
    {
		HudongView.SetActive(true);
		
		GameObject HudongButtons = RL.GetObject(RL.HudongButtons);
		HudongButtons.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, 0);
		HudongButtons.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width_HD / 3, height_HD / 10);
		HudongButtons.transform.SetParent(Hudong.transform,false);
		
		var strs = str.Split("$z2#");
		
		for (var i = 0; i < strs.Length; i++)
		{

			var dirs = strs[i].Split('|');
			if (dirs.Length < 2) continue;
			var hi = height_HD / 3;
			var wi = width_HD / 20;
			
			dirs[0] = dirs[0].Replace("$br#", "");
			GameObject eeee = RL.GetObject(RL.ActButton);
			eeee.name = dirs[1];
			eeee.GetComponentInChildren<TMP_Text>().text = ColorPut(dirs[0]);
			eeee.GetComponent<RectTransform>().sizeDelta = new Vector2(wi, hi);
			eeee.transform.SetParent(HudongButtons.transform,false);
			
		}
	}


	private void WriteInput(string str)
    {
		var ss = str.Split("$zj#");
		ss[0] = ss[0].Replace("$br#", "\n");
		WriteToHD(ss[0]);
		GameObject eeee = RL.GetObject(RL.Input);
		eeee.name = "HudongInput";
		eeee.transform.SetParent(Hudong.transform,false);
		eeee.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(width_HD * 0.8), height_HD/10);
		GameObject dddd = RL.GetObject(RL.ObjButton);
		dddd.GetComponent<Button>().onClick.AddListener(delegate {Says(ss[1],eeee);}) ;
		dddd.transform.SetParent(Hudong.transform,false);
		dddd.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD/3, height_HD/10);
		dddd.GetComponentInChildren<TMP_Text>().text = "确认";
		


	}


	

	public void Says(string str,GameObject a)
    {
		string strs = a.GetComponent<TMP_InputField>().text;
		a.GetComponent<TMP_InputField>().text = "";
		strs = Regex.Replace(strs, "<sprite=", "<e:");
		if (strs == "")
			return;
		JS.SendMsg(str.Replace("$txt#",strs));
		CloseHudong();
    }


	public void CloseMycmds()
    {
		int num = Mycmds.transform.childCount;
		int j = 0;
		for (int i = 0; i < num; i++)
		{
			var child = Mycmds.transform.GetChild(j);
			if (child.gameObject.name != "CloseCmds")
			{
				Button_Pool.ReleaseObject(child.gameObject);
			}
			else
			{
				Destroy(child.gameObject);
				j++;
			}
		}
		Mycmds.SetActive(false);

	}

	public void CloseHudongByUser()
    {
		isMoveCloseHudong = true;
		CloseHudong();
		
	}

	public void CloseHudong()
    {
		if (!HudongView.active)
			return;
		if (!isMoveCloseHudong)
			return;
		EmojiChangeBlock = false;
		var temp = Hudong.transform.Find("HDButtons");
		if(temp != null)
        {
			int a = temp.childCount;
			Transform transform;
			for (int i = 0; i < a; i++)
			{
				transform = temp.GetChild(0);
				if(!transform.name.Contains("Emoji"))
                {
					Button_Pool.ReleaseObject(transform.gameObject);
				}

			}
		}

		DestroyAllObjectsInChildren(Hudong);
		HudongView.SetActive(false);
		
	}



    public string ColorPut(string str)
    {
        string strt = str;
        var raw_text_chunks = strt.Split("[");
        string res = "";
        for (int i = 1; i < raw_text_chunks.Length; i++)
        {
            string temp = Chunk_Process(raw_text_chunks[i]);
            if (temp != "")
                res += temp;
        }
		_PUT_FONTSTYLE_.Clear();
        _PUT_CMD_ = "";
        _PUT_FONTSIZE_ = "";
        if (res.Length > 5000)
            res = res.Substring(0, 5000);
        return raw_text_chunks[0] + res;
    }


    private string Chunk_Process(string text)
    {
        var matches = Regex.Match(text, "^(\\d{0,3}(;\\d{0,3})*)([mKJHsuf])(.*)", RegexOptions.Singleline);
        if (!matches.Success)
            return text;
		
        string orig_txt = matches.Groups[4].Value;
		string control_txt = matches.Groups[3].Value;
		if (control_txt == "m")
		{
			string source = matches.Groups[1].Value;
			var ss = source.Split(";");
			for (int i=0;i<ss.Length;i++)
            {
				if (fontStyle.ContainsKey(ss[i]))
					_PUT_FONTSTYLE_.Add(fontStyle[ss[i]]);
				else
					_PUT_FONTSTYLE_.Clear();
			}

			return SetFormat(orig_txt);
		}
		else if(control_txt == "u")
        {
			var ss = orig_txt.Split("]");
			_PUT_CMD_ = ss[0];

			return SetFormat(ss[1]);
		}
		else if (control_txt == "s")
        {
			var ss = orig_txt.Split("]");
			_PUT_FONTSIZE_ = ss[0].Substring(1);
			return SetFormat(ss[1]);
		}
		else if (control_txt == "f")
        {
			if (text.Length > 8)
				_PUT_FONTSTYLE_.Add(new string[2] { "<color=" + text.Substring(1, 7) + ">", "</color>" });
			else
				_PUT_FONTSTYLE_.Clear();

			if (text.Length > 9)
			{
				string rgb_text = text.Substring(9);
				return SetFormat(rgb_text);
			}
			else return "";
		}
		else if (control_txt == "J")
        {
			int num = Out.transform.GetChildCount();
			for (int i=0;i<num;i++)
            {
				TMP_Pool.ReleaseObject(Out.transform.GetChild(i).gameObject);
			}
		}
		return "";


    }


	private string SetFormat(string str)
    {
		if (str == "" || str == "\r" || str == "\t")
			return "";
		string res = str;
		if (_PUT_CMD_ != "")
			res = "<u><link=\"" + _PUT_CMD_ + "\">" + res + "</link></u>";
		if (_PUT_FONTSTYLE_.Count != 0)
        {
			for( int i=0;i<_PUT_FONTSTYLE_.Count;i++)
            {
				res = _PUT_FONTSTYLE_[i][0] + res + _PUT_FONTSTYLE_[i][1];

			}
        }
		if (_PUT_FONTSIZE_ != "")
		{
            if (!int.TryParse(_PUT_FONTSIZE_, out int size))
                size = 2;
            res = "<size=+" + size / 2 + ">" + res + "</size>";
		}
		_PUT_FONTSTYLE_.Clear();
		_PUT_CMD_ = "";
		_PUT_FONTSIZE_ = "";
		return res;
	}




    public string ClearFormat(string str)
    {
		string strs = str;
		string temp = Regex.Replace(strs, "\\[\\d.*?m", "");
		temp = Regex.Replace(temp, "\\[.*?]", "");
		return temp;
    }


	public void OpenSetting()
    {
		OpenScript();
		Setting.SetActive(true);
		ShiMen.SetActive(false);
		FuBen.SetActive(false);
		BoDong.SetActive(false);
		LianGong.SetActive(false);
		Plugin.SetActive(false);
		Slider slider = GameObject.Find("performanceSlider").GetComponent<Slider>();
		Slider freshSlider = GameObject.Find("freshSpeedSlider").GetComponent<Slider>();
		TMP_Text freshLable = GameObject.Find("freshSpeedLable").GetComponent<TMP_Text>();
		TMP_Text lable = GameObject.Find("performanceLable").GetComponent<TMP_Text>();
		int freshSpeed = PlayerPrefs.GetInt("freshSpeed", 4);
		int Performance = PlayerPrefs.GetInt("Performance", 1);
		int isBackData = PlayerPrefs.GetInt("isBackData", 0);
		int isOpenNotice = PlayerPrefs.GetInt("isOpenNotice", 0);
		int isShowLong = PlayerPrefs.GetInt("isShowLong", 1);
		int isMovable = PlayerPrefs.GetInt("isMovable", 0);
		if (isBackData == 1)
			GameObject.Find("isBackData").GetComponent<Toggle>().isOn = true;
		else
			GameObject.Find("isBackData").GetComponent<Toggle>().isOn = false;
		if (isOpenNotice == 1)
			GameObject.Find("isOpenNotice").GetComponent<Toggle>().isOn = true;
		else
			GameObject.Find("isOpenNotice").GetComponent<Toggle>().isOn = false;
		if (isShowLong == 1)
			GameObject.Find("isShowLong").GetComponent<Toggle>().isOn = false;
		else
			GameObject.Find("isShowLong").GetComponent<Toggle>().isOn = true;
		if (isMovable == 1)
			GameObject.Find("isMovable").GetComponent<Toggle>().isOn = true;
		else
			GameObject.Find("isMovable").GetComponent<Toggle>().isOn = false;

		freshSlider.value = freshSpeed;
		freshLable.text = freshSpeed.ToString();
		switch (Performance)
		{
			case 0:
				lable.text = "节能";
				break;
			case 1:
				lable.text = "60帧";
				break;
			case 2:
				lable.text = "高帧率";
				break;
			case 3:
				lable.text = "超高帧";
				break;
		}
		slider.value = Performance;

		QuickLoadList();
	}

	public void SetForegroundService(bool isOpen)
    {
		if (isOpen  && !isForegroundServiceOpen)
			GameObject.Find("ForegroundServiceGameObject").GetComponent<ForegroundServiceController>().StartForegroundService();
		else
			GameObject.Find("ForegroundServiceGameObject").GetComponent<ForegroundServiceController>().StopForegroundService();

		isForegroundServiceOpen = isOpen;
	}

	public void ChangePerformance()
    {
		float value = GameObject.Find("performanceSlider").GetComponent<Slider>().value;
		TMP_Text lable = GameObject.Find("performanceLable").GetComponent<TMP_Text>();
		int refreashRate = Screen.currentResolution.refreshRate;
		switch((int)value)
        {
            case 0:
                Application.targetFrameRate = 30;
				PlayerPrefs.SetInt("Performance", 0);
				lable.text = "节能";
				break;
            case 1:
				Application.targetFrameRate = 60;
				PlayerPrefs.SetInt("Performance", 1);
				lable.text = "60帧";
				break;
            case 2:
				Screen.SetResolution(0, 0, true, 0);
				Application.targetFrameRate = 90;
				PlayerPrefs.SetInt("Performance", 2);
				lable.text = "高帧率";
				if(refreashRate != 90)
                {
					ShowInf("[1;36m屏幕刷新率为 " + refreashRate + " Hz可能并不合适[0;0m");
				}
                else
                {
					ShowInf("[1;36m屏幕刷新率为 " + refreashRate + " Hz适合该帧率[0;0m");
				}
                break;
            case 3:
				Screen.SetResolution(0, 0, true, 0);
				Application.targetFrameRate = 120;
				PlayerPrefs.SetInt("Performance", 3);
				lable.text = "超高帧";
				if (refreashRate < 120)
				{
					ShowInf("[1;36m屏幕刷新率为 " + refreashRate + " Hz可能并不合适[0;0m");
				}
				else
				{
					ShowInf("[1;36m屏幕刷新率为 " + refreashRate + " Hz适合该帧率[0;0m");
				}
				break;
		}
    }

	public void ChangeFreshSpeed()
    {
		float value = GameObject.Find("freshSpeedSlider").GetComponent<Slider>().value;
		TMP_Text lable = GameObject.Find("freshSpeedLable").GetComponent<TMP_Text>();
		lable.text = ((int)value).ToString();
		freshSpeed_global = (int)value;
		PlayerPrefs.SetInt("freshSpeed", (int)value);

	}

	public void ChangeOpenNotice()
    {
		bool temp = GameObject.Find("isOpenNotice").GetComponent<Toggle>().isOn;
		if(temp)
        {
			PlayerPrefs.SetInt("isOpenNotice", 1);
			SetForegroundService(true);
		}
		else
		{
			PlayerPrefs.SetInt("isOpenNotice", 0);
			SetForegroundService(false);
		}
	}

	public void ChangeShowLong()
    {
		bool temp = GameObject.Find("isShowLong").GetComponent<Toggle>().isOn;
		if (temp)
		{
			PlayerPrefs.SetInt("isShowLong", 0);
			isShowLong = false;
			CloseEnemy();
		}
		else
		{
			PlayerPrefs.SetInt("isShowLong", 1);
			isShowLong = true;
		}
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



	public void ShowInf(string inf)
    {
		line.Enqueue(inf+"\n");
    }


	public void OpenCmd()
    {
		WriteInput("请输入指令：$zj#$txt#");
    }
	

	public void OpenScript()
    {
		Script.SetActive(true);
    }


	public void ChangeBackData()
    {
		bool temp = GameObject.Find("isBackData").GetComponent<Toggle>().isOn;
		if(temp)
        {
			PlayerPrefs.SetInt("isBackData", 1);
		}
		else
        {
			PlayerPrefs.SetInt("isBackData", 0);
		}
    }


	public void CloseScript()
    {
		Script.SetActive(false);
    }


	public void OpenShiMen()
    {
		ShiMen.SetActive(true);
		BoDong.SetActive(false);
		Setting.SetActive(false);
		FuBen.SetActive(false);
		LianGong.SetActive(false);
		Plugin.SetActive(false);
	}
	public void OpenBoDong()
	{
		ShiMen.SetActive(false);
		Setting.SetActive(false);
		BoDong.SetActive(true);
		FuBen.SetActive(false);
		LianGong.SetActive(false);
		Plugin.SetActive(false);
	}
	public void OpenFuBen()
    {
		Setting.SetActive(false);
		ShiMen.SetActive(false);
		FuBen.SetActive(true);
		BoDong.SetActive(false);
		LianGong.SetActive(false);
		Plugin.SetActive(false);
	}


	public void CloseEnemy()
    {
		Enemy.SetActive(false);
    }


	public void ChangeQuick()
    {
		int hashCode = SharedVariables.AccountInf.GetHashCode();
		string temp = PlayerPrefs.GetString("Quick:" + hashCode.ToString(), "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
			"\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
			"look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");
		var oneStep = temp.Split("$zj#");
		if (oneStep.Length != 11)
		{
			string changeText = temp;
			for (int i = oneStep.Length; i < 11; i++)
			{
				changeText += "$zj#b2:未设\n置" + (i + 1).ToString() + ":look";
			}
			PlayerPrefs.SetString("Quick:" + hashCode.ToString(), changeText);
			oneStep = changeText.Split("$zj#");
		}
		TMP_Dropdown obj = GameObject.Find("QuickDropdown").GetComponent<TMP_Dropdown>();
		string value = obj.options[obj.value].text;
		value = value.Replace("$br#", "\n");

		string str = GameObject.Find("QuickInput").GetComponent<TMP_InputField>().text;
		
		if(!str.Contains(":"))
        {
			ShowInf("[1;36m格式错误，请按照格式输入![0;0m");
			CloseScript();
			return;
        }

		string res = "";
		bool flag = true;
		for (int i = 0; i < oneStep.Length; i++)
		{
			if (oneStep[i].Contains(value) && flag)
			{
				res += "b2:" + str + "$zj#";
				flag = false;
			}
			else res += oneStep[i] + "$zj#";
		}
		res = res.TrimEnd("$zj#".ToCharArray());
		PlayerPrefs.SetString("Quick:" + hashCode.ToString(), res);
		ShowInf("[1;36m成功修改快捷键[0;0m");
		CloseScript();
		
	}

	private void QuickLoadList()
    {
		int hashCode = SharedVariables.AccountInf.GetHashCode();
		string temp = PlayerPrefs.GetString("Quick:" + hashCode.ToString(), "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
			"\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
			"look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");
		var oneStep = temp.Split("$zj#");
		if (oneStep.Length != 11)
		{
			string changeText = temp;
			for (int i = oneStep.Length; i < 11; i++)
			{
				changeText += "$zj#b2:未设\n置" + (i + 1).ToString() + ":look";
			}
			PlayerPrefs.SetString("Quick:" + hashCode.ToString(), changeText);
			oneStep = changeText.Split("$zj#");
		}
		TMP_Dropdown obj = GameObject.Find("QuickDropdown").GetComponent<TMP_Dropdown>();
		obj.options.Clear();
		for (int i=0;i<11;i++)
        {
			string res = TextGainCenter("b2:", ":", oneStep[i]);
			res = res.Replace("\n", "$br#");
			obj.options.Add(new TMP_Dropdown.OptionData(res));
		}
		obj.RefreshShownValue();

	}

	public void ChangeQuickList()
    {
		TMP_Dropdown obj = GameObject.Find("QuickDropdown").GetComponent<TMP_Dropdown>();
		string value = obj.options[obj.value].text;
		value = value.Replace("$br#", "\n");
		int hashCode = SharedVariables.AccountInf.GetHashCode();
		string temp = PlayerPrefs.GetString("Quick:" + hashCode.ToString(), "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
			"\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
			"look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");
		var oneStep = temp.Split("$zj#");

		for (int i = 0; i < oneStep.Length; i++)
		{
			if (oneStep[i].Contains(value))
			{
				GameObject.Find("QuickInput").GetComponent<TMP_InputField>().text = oneStep[i].Substring(3);
				break;
			}
		}
	}


	public void DestroyAllObjectsInChildren(GameObject temp)
    {
		Transform[] children = temp.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != temp.transform)
			{
                Destroy(child.gameObject);
			}
		}
	}


	public void ReleaseButtons(GameObject temp)
    {
		int a = temp.transform.childCount;
		Transform transform;
		for (int i = 0; i < a; i++)
		{
			transform = temp.transform.GetChild(0);
			Button_Pool.ReleaseObject(transform.gameObject);
			
		}
	}

	public void OpenQuick()
    {
		int hashCode = SharedVariables.AccountInf.GetHashCode();
		string temp = PlayerPrefs.GetString("Quick:" + hashCode.ToString(), "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
			"\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
			"look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");
		WriteToMU(temp);
		
    }


	public void OpenLianGong()
    {
		ShiMen.SetActive(false);
		Setting.SetActive(false);
		BoDong.SetActive(false);
		FuBen.SetActive(false);
		LianGong.SetActive(true);
		Plugin.SetActive(false);
		//int LianGongSetting = PlayerPrefs.GetInt("LianGongSetting", 0);
		//GameObject.Find("LianGongSetting").GetComponent<TMP_Dropdown>().value = LianGongSetting;
		//GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("LianGongSetting" + LianGongSetting, "");

	}

	public void OpenPlugin()
    {
		OpenScript();
		ShiMen.SetActive(false);
		Setting.SetActive(false);
		BoDong.SetActive(false);
		FuBen.SetActive(false);
		LianGong.SetActive(false);
		Plugin.SetActive(true);
	}



	public void LiaoTian()
    {
		if(!EmojiChangeBlock)
        {
			JS.SendMsg("liaotian");
			Invoke("LiaoTian_Main", 0.3f);
		}
		
	}

	private void LiaoTian_Main()
    {
		GameObject HudongButtons = GameObject.Find("HDButtons");
		if (HudongButtons == null)
			return;
		isMoveCloseHudong = false;
		HudongButtons.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width_HD / 6 - 10, height_HD / 9);
		GameObject exmaple = Resources.Load("EmojiEmptyButton") as GameObject;
		for (int i = 0; i < 3; i++)
		{
			GameObject dddd = Instantiate(exmaple);
			dddd.transform.SetParent(HudongButtons.transform, false);
			if (i == 0)
			{
				dddd.name = "EmojiRecently";
				dddd.GetComponentInChildren<TMP_Text>().text = "最近表情";
			}
			else if (i == 1)
			{
				dddd.name = "EmojiPrevious";
				dddd.GetComponentInChildren<TMP_Text>().text = "上一页";
			}
			else
			{
				dddd.name = "EmojiNext";
				dddd.GetComponentInChildren<TMP_Text>().text = "下一页";
			}
		}
		StartCoroutine(LiaoTian_Recently(HudongButtons));
	}

	public IEnumerator LiaoTian_Recently(GameObject HudongButtons)
    {
		if (EmojiChangeBlock)
			yield break;
		EmojiChangeBlock = true;
		Transform[] children = HudongButtons.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != HudongButtons.transform && child.name == "EmojiButton")
			{
				Destroy(child.gameObject);
			}
		}
		EmojiPage = 0;
		string RecentlyEmoji = PlayerPrefs.GetString("RecentlyEmoji", "0;1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;21;22;23;24;25;26;27;28;29");
		var dir = RecentlyEmoji.Split(";");
		for (int i = 0; i < dir.Length; i++)
		{
			GameObject eeee = RL.GetObject(RL.EmojiButton);
			eeee.GetComponentInChildren<TMP_Text>().text = "<sprite=" + dir[i] + ">";
			eeee.name = "EmojiButton";
			if (!EmojiChangeBlock)
				break;
			eeee.transform.SetParent(HudongButtons.transform, false);
			if(i % 2 == 0)
				yield return new WaitForEndOfFrame();
		}
		EmojiChangeBlock = false;
	}

	public IEnumerator LiaoTian_ChangePage(GameObject HudongButtons, bool isNext)
    {
		if (EmojiChangeBlock)
			yield break;
		EmojiChangeBlock = true;
		Transform[] children = HudongButtons.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != HudongButtons.transform && child.name == "EmojiButton")
			{
				Destroy(child.gameObject);
			}
		}
		if (isNext)
			EmojiPage++;
		else EmojiPage--;
		int endIndex = EmojiPage * 30;
		int startIndex = endIndex - 30;
		if (endIndex == 120)
			endIndex = 107;
		for(int i = startIndex;i<endIndex;i++)
        {
			GameObject eeee = RL.GetObject(RL.EmojiButton);
			eeee.GetComponentInChildren<TMP_Text>().text = "<sprite=" + i + ">";
			eeee.name = "EmojiButton";
			eeee.transform.SetParent(HudongButtons.transform, false);
			yield return new WaitForEndOfFrame();
		}
		EmojiChangeBlock = false;
	}

	public void ChangeIsMovable()
    {
		bool isOn = false;
		isOn = GameObject.Find("isMovable").GetComponent<Toggle>().isOn;
		if (isOn)
			PlayerPrefs.SetInt("isMovable", 1);
		else
			PlayerPrefs.SetInt("isMovable", 0);

	}

	public void ReadOldQuick()
    {
		int hashCode = SharedVariables.AccountInf.GetHashCode();
		string res = PlayerPrefs.GetString("Quick", "b2:未设\n置1:look$zj#b2:未设\n置2:look$zj#b2:未设" +
			"\n置3:look$zj#b2:未设\n置4:look$zj#b2:未设\n置5:look$zj#b2:未设\n置6:look$zj#b2:未设\n置7:" +
			"look$zj#b2:未设\n置8:look$zj#b2:未设\n置9:look$zj#b2:未设\n置10:look$zj#b2:未设\n置11:look");
		PlayerPrefs.SetString("Quick:" + hashCode.ToString(), res);
		AGUIMisc.ShowToast("载入完成，请重新打开设置");
    }

}
