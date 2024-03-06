using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using DeadMosquito.AndroidGoodies;
using System;

public class NoticeController
{

    NotificationChannel channel;
    private static int id = 754336;
    public static List<int> idList = new List<int>();

    public NoticeController()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.SCHEDULE_EXACT_ALARM"))
        {
            string[] permissions = { "android.permission.SCHEDULE_EXACT_ALARM" };
            Permission.RequestUserPermissions(permissions);
        }

        if (Application.platform != RuntimePlatform.Android)
            return;
        channel = new NotificationChannel("Mud_Channel", "Mud_Notice", AGNotificationManager.Importance.Default);
        AGNotificationManager.CreateNotificationChannel(channel);
    }

    private void ScheduleNotification(int hour,int min, string text)
    {
        var builder = new Notification.Builder("Mud_Channel")
        .SetContentTitle("定时提醒")
        .SetContentText(text)
        .SetSmallIcon("notify_icon_small");

        DateTime notificationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);

        id++;
        idList.Add(id);
        AGNotificationManager.Notify(id, builder.Build(), notificationTime);
    }

    private void OpenForeignerNotice()
    {
        int[] times = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        int hourNow = DateTime.Now.Hour;
        foreach(int hour in times)
        {
            if (hour <= hourNow)
                continue;
            ScheduleNotification(hour - 1, 58, "歪果仁挑战Boss即将出现，点击进入Mud。");
        }
    }

    private void OpenJiWuMingNotice()
    {
        int hourNow = DateTime.Now.Hour;
        int minNow = DateTime.Now.Minute;
        for(int i=8;i<23;i++)
        {
            if (minNow >= 30 && hourNow >= i)
                continue;
            if (minNow < 30 && hourNow > i)
                continue;
            ScheduleNotification(i, 28, "姬无命挑战Boss即将出现，点击进入Mud。");
        }
    }

    private void OpenZhangWuJiNotice()
    {
        int hourNow = DateTime.Now.Hour;
        int minNow = DateTime.Now.Minute;
        for (int i = 8; i < 23; i++)
        {
            if (minNow >= 50 && hourNow >= i)
                continue;
            if (minNow < 50 && hourNow > i)
                continue;
            ScheduleNotification(i, 48, "张无忌挑战Boss即将出现，点击进入Mud。");
        }
    }

    public static void CloseAllNotice()
    {
        for(int i=0;i<idList.Count;i++)
        {
            int id_temp = idList[i];
            AGNotificationManager.CancelScheduledNotification(id_temp);
        }
        idList.Clear();
        id = 754336;
        PlayerPrefs.SetString("NoticeID", "");
    }

    public void OpenNotice(bool Foreigner,bool JiWuMing,bool ZhangWuJi)
    {
        CloseAllNotice();

        if (Foreigner)
            OpenForeignerNotice();
        if (JiWuMing)
            OpenJiWuMingNotice();
        if (ZhangWuJi)
            OpenZhangWuJiNotice();
        string id_temp = "";
        for(int i=0;i<idList.Count;i++)
        {
            id_temp += idList[i].ToString() + ";";
        }
        if (id_temp.EndsWith(';'))
            id_temp = id_temp.Substring(0, id_temp.Length - 1);

        PlayerPrefs.SetString("NoticeID", id_temp);
    }

}
