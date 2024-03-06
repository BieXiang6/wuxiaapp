using UnityEngine;


public class ForegroundServiceController : MonoBehaviour
{
    private const string PluginClassName = "com.wuxia.ForegroundService.UnityForegroundServicePlugin";

    // 修改为你希望接收回调的Unity游戏对象的名称
    public string gameObjectName = "ForegroundServiceGameObject";

    public void StartForegroundService()
    {
        string imagePath = "icon_png"; // 替换为你的图片在Asset文件夹中的路径

        Texture2D image = Resources.Load(imagePath) as Texture2D;
        byte[] imageData = image.EncodeToPNG();

        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("startForegroundService", imageData);
    }

    public void StopForegroundService()
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("stopForegroundService");
    }

    // 在服务启动后，Java插件将调用此方法通知Unity
    public void OnForegroundServiceStarted(string message)
    {
        GameObject.Find("Global controller").GetComponent<UIcontroller>().ShowInf(message);
    }
}
