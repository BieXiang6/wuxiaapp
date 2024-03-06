using UnityEngine;


public class ForegroundServiceController : MonoBehaviour
{
    private const string PluginClassName = "com.wuxia.ForegroundService.UnityForegroundServicePlugin";

    // �޸�Ϊ��ϣ�����ջص���Unity��Ϸ���������
    public string gameObjectName = "ForegroundServiceGameObject";

    public void StartForegroundService()
    {
        string imagePath = "icon_png"; // �滻Ϊ���ͼƬ��Asset�ļ����е�·��

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

    // �ڷ���������Java��������ô˷���֪ͨUnity
    public void OnForegroundServiceStarted(string message)
    {
        GameObject.Find("Global controller").GetComponent<UIcontroller>().ShowInf(message);
    }
}
