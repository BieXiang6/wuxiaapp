using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;
using MiniJSON;
using System.IO;

public class Plugin
{
    private string name, author, description, version, icon, index;
    public bool isReady = false;

    public Plugin(string name_input)
    {
        SetPlugin(name_input);
    }

    public Plugin()
    {

        OpenSetting();
    }

    private async void OpenSetting()
    {
        string path = "streaming-assets://PluginDriver/index.html";
        isReady = false;
        if (SharedVariables.WV != null)
        {
            SharedVariables.WV.WebView.MessageEmitted -= (sender, eventArgs) => {
                MessageHandler(eventArgs.Value);
            };

            CanvasWebViewPrefab a = SharedVariables.WV;
            SharedVariables.WV = null;
            Object.Destroy(a);
        }
        CanvasWebViewPrefab tempWebview = CanvasWebViewPrefab.Instantiate();
        tempWebview.transform.parent = SharedVariables.WVC.transform;
        var rectTransform = tempWebview.transform as RectTransform;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        tempWebview.transform.localScale = Vector3.one;
        await tempWebview.WaitUntilInitialized();
        tempWebview.WebView.LoadUrl(path);
        SharedVariables.WV = tempWebview;
        SharedVariables.WV.WebView.MessageEmitted += (sender, eventArgs) => {
            MessageHandler(eventArgs.Value);
        };
        await SharedVariables.WV.WebView.WaitForNextPageLoadToFinish();
        
        isReady = true;
    }


    public void SetPlugin(string name_input)
    {
        string path = "MudPlugins/" + name_input + "/config.txt";
        string fullPath = Path.Combine(Application.persistentDataPath, path);
        if (File.Exists(fullPath))
        {
            string jsonText = File.ReadAllText(fullPath);

            Dictionary<string, object> jsonData = Json.Deserialize(jsonText) as Dictionary<string, object>;

            name = jsonData["name"] as string;
            author = jsonData["author"] as string;
            description = jsonData["description"] as string;
            version = jsonData["version"] as string;
            icon = jsonData["icon"] as string;
            index = jsonData["index"] as string;

        }
        else
        {
            name = null;
        }

    }

    public Dictionary<string, object> GetInf()
    {
        Dictionary<string, object> jsonData = new Dictionary<string, object>
        {
            {"name",name },
            {"author",author },
            {"description",description },
            {"version",version },
            {"icon",icon },
            {"index",index }

        };

        return jsonData;
    }

    public string GetIndexPath()
    {
        string path = "MudPlugins/" + name + "/" + index;
        string fullPath = Path.Combine(Application.persistentDataPath, path);
        return fullPath;
    }

    public Texture2D GetIcon()
    {
        string path = "MudPlugins/" + name + "/" + icon;
        Texture2D texture = new Texture2D(2, 2);
        string fullPath = Path.Combine(Application.persistentDataPath, path);
        if (!File.Exists(fullPath))
            return texture;
        byte[] imageData = File.ReadAllBytes(fullPath);
        texture.LoadImage(imageData);
        return texture;

    }
    public string GetName()
    {
        return name;
    }

    public async void OpenPlugin()
    {
        isReady = false;
        if (SharedVariables.WV != null)
        {
            SharedVariables.WV.WebView.MessageEmitted -= (sender, eventArgs) => {
                MessageHandler(eventArgs.Value);
            };
            
            CanvasWebViewPrefab a = SharedVariables.WV;
            SharedVariables.WV = null;
            Object.Destroy(a);
            
        }
        CanvasWebViewPrefab tempWebview = CanvasWebViewPrefab.Instantiate();
        tempWebview.transform.parent = SharedVariables.WVC.transform;
        var rectTransform = tempWebview.transform as RectTransform;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        tempWebview.transform.localScale = Vector3.one;
        await tempWebview.WaitUntilInitialized();
        string path = GetIndexPath();
        if (!File.Exists(path))
            return;
        tempWebview.WebView.LoadUrl("file://" + path);
        SharedVariables.WV = tempWebview;
        SharedVariables.WV.WebView.MessageEmitted += (sender, eventArgs) => {
            MessageHandler(eventArgs.Value);
        };
        await SharedVariables.WV.WebView.WaitForNextPageLoadToFinish();
        
        isReady = true;

    }

    public void ClosePlugin()
    {
        isReady = false;

    }

    private void MessageHandler(string msg)
    {
        var str = msg.Split("¨U@¨U");
        if (str.Length != 2)
            return;
        if(str[0] == "cmds")
        {
            SharedVariables.JS.SendMsg(str[1]);
        }
        else if(str[0] == "clear")
        {
            if (str[1] == "outText")
            {
                SharedVariables.Out = "";
                SharedVariables.WV.WebView.PostMessage("out¨U@¨U");
            }
            else if (str[1] == "chatText")
            {
                SharedVariables.Chat = "";
                SharedVariables.WV.WebView.PostMessage("chat¨U@¨U");
            }
        }
        else if(str[0] == "del")
        {
            SharedVariables.PC.DelPlugin(str[1]);
        }
        else if(str[0] == "add")
        {
            SharedVariables.PC.AddPlugin();
        }
        else if(str[0] == "reload")
        {
            SharedVariables.PC.OpenPlugin(str[1],true);
        }
        else if(str[0] == "get")
        {
            string tempstr = SharedVariables.PC.GetPluginsList();
            SharedVariables.WV.WebView.PostMessage("pluginsList¨U@¨U" + tempstr);
            
        }
        else if(str[0] == "showInf")
        {
            SharedVariables.uIcontroller.ShowInf(str[1]);
        }
    }


}
