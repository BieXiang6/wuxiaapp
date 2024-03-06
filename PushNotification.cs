using UnityEngine;
using DeadMosquito.AndroidGoodies;

public class PushNotification
{

    public PushNotification()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        var channel = new NotificationChannel("Mud_Channel", "Mud_Notice", AGNotificationManager.Importance.Low);
        AGNotificationManager.CreateNotificationChannel(channel);
    }

    public void SendNotice(string Title, string Text)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        var builder = new Notification.Builder("Mud_Channel")
        .SetContentTitle(Title)
        .SetContentText(Text)
        .SetSmallIcon("notify_icon_small");

        AGNotificationManager.Notify(12123, builder.Build());

    }

    public void DelNotice()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        AGNotificationManager.CancelAll();
    }

}
