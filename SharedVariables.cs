using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;
using System.Threading;

public class SharedVariables : MonoBehaviour
{
    public static string Out;
    public static string Obj;
    public static string Direction;
    public static string Chat;
    public static string Here;
    public static string Hudong_Buttons;
    public static string Hudong;
    public static string Long;
    public static string State;
    public static string Exits;
    
    public static long HP;//血量
    public static long HP_MAX;
    public static long Force;//内力
    public static long Force_MAX;
    public static long Mind;//精神
    public static long EXP;//经验
    public static long Potential;//潜能
    public static long Energy;//精力
    public static int Busy;//忙碌
    public static long Ping;

    public static string ID = "";

    public static Color pressedColor;
    public static Color originalColor;
    public static string AccountInf;

    public static main JS = null;
    public static UIcontroller uIcontroller = null;
    public static ModScript Mod = null;
    public static ResourceLoader RL = null;
    public static GameObject CmdTip;
    public static PushNotification PN = null;
    public static CanvasWebViewPrefab WV = null;
    public static PluginsController PC = null;
    public static Canvas WVC = null;
    public static string Server_socket = "47.101.42.39:5500";


}
