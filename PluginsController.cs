using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using DeadMosquito.AndroidGoodies;
using DeadMosquito.AndroidGoodies.Internal;
using ICSharpCode.SharpZipLib.Zip;
using MiniJSON;
using UnityEngine.Android;

public class PluginsController : MonoBehaviour
{
    private List<Plugin> plugins = new List<Plugin>();
    private Plugin plugin_running = null;
    public GameObject PluginsList;


    public void LoadFromData()
    {
        plugins.Clear();
        SharedVariables.uIcontroller.DestroyAllObjectsInChildren(PluginsList);
        string folderPath = Path.Combine(Application.persistentDataPath, "MudPlugins");
        if (Directory.Exists(folderPath))
        {
            string[] folderPaths = Directory.GetDirectories(folderPath);

            foreach (string path in folderPaths)
            {
                string folderName = Path.GetFileName(path);
                Plugin newPlugin = new Plugin(folderName);
                if (newPlugin.GetName() == null)
                    continue;
                plugins.Add(newPlugin);
                GameObject tempButton = SharedVariables.RL.GetObject(SharedVariables.RL.PluginButton);
                tempButton.GetComponent<PluginClick>().pluginName = folderName;
                Texture2D texture = newPlugin.GetIcon();
                tempButton.GetComponentInChildren<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                tempButton.transform.SetParent(PluginsList.transform, false);
            }
        }
        else
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public string GetPluginsList()
    {
        string res = "";
        for(int i=0;i<plugins.Count;i++)
        {
            string jsonString = Json.Serialize(plugins[i].GetInf());
            res += jsonString + "#";
        }

        if (res == "")
            return "";
        if(res[res.Length-1]=='#')
        {
            res = res.Substring(0, res.Length - 1);
        }
        return res;
    }
    
    public void OpenPlugin(string name,bool canReload = false)
    {
        if(plugin_running != null)
        {
            if(!canReload && plugin_running.GetName() == name)
            {
                SharedVariables.uIcontroller.OpenPlugin();
                return;
            }
            plugin_running.ClosePlugin();
            plugin_running = null;
        }
        for(int i=0;i<plugins.Count;i++)
        {
            if(plugins[i].GetName() == name)
            {
                plugin_running = plugins[i];
                break;
            }
        }
        if (plugin_running == null)
            return;
        SharedVariables.uIcontroller.OpenPlugin();
        plugin_running.OpenPlugin();
        
    }

    public Plugin GetRunningPlugin()
    {
        return plugin_running;
    }


    public void AddPlugin()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.MANAGE_EXTERNAL_STORAGE"))
        {
            Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
        }
        const string mimeType = "*/*";
        string filePath = "";
        AGFilePicker.PickFile(file =>
        {
            filePath = file.OriginalPath;
            ReleaseZip(filePath);
        }, error => { SharedVariables.uIcontroller.ShowInf("·¢ÉúÁË´íÎó£º" + error); }, mimeType);

    }

    private void ReleaseZip(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        string extractionFolderPath = Path.Combine(Application.persistentDataPath, "MudCache");
        if (!Directory.Exists(extractionFolderPath))
            Directory.CreateDirectory(extractionFolderPath);

        using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(filePath)))
        {
            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                string entryName = entry.Name;
                string fullEntryPath = Path.Combine(extractionFolderPath, entryName);

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(fullEntryPath);
                }
                else
                {
                    using (FileStream fileStream = File.Create(fullEntryPath))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = zipInputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        if (!File.Exists(extractionFolderPath + "/config.txt"))
        {
            Directory.Delete(extractionFolderPath);
            SharedVariables.uIcontroller.ShowInf("[1;36m²å¼þ°²×°Ê§°Ü£¬·Ç±ê×¼µÄ²å¼þ£¡[0;0m");
            return;
        }
        string jsonText = File.ReadAllText(extractionFolderPath + "/config.txt");

        Dictionary<string, object> jsonData = Json.Deserialize(jsonText) as Dictionary<string, object>;

        string PluginName = jsonData["name"] as string;

        string newFolderPath = Path.Combine(Application.persistentDataPath, "MudPlugins/" + PluginName);

        if (Directory.Exists(newFolderPath))
            Directory.Delete(newFolderPath, true);

        Directory.Move(extractionFolderPath, newFolderPath);

        SharedVariables.uIcontroller.ShowInf("[1;36m²å¼þ£º " + PluginName + " °²×°Íê³É£¡[0;0m");

        LoadFromData();
    }

    public void DelPlugin(string pluginName)
    {
        string path = Path.Combine(Application.persistentDataPath, "MudPlugins/" + pluginName);
        if (!Directory.Exists(path))
            return;
        Directory.Delete(path, true);
        SharedVariables.uIcontroller.ShowInf("[1;36m²å¼þ£º " + pluginName + " ÒÑ¾­´ÓÄúµÄÉè±¸ÖÐÒÆ³ý£¡[0;0m");

        LoadFromData();
    }
    
    public void OpenSetting()
    {
        if (plugin_running != null)
        {
            if (plugin_running.GetName() == "PluginDriver")
            {
                SharedVariables.uIcontroller.OpenPlugin();
                return;
            }
            plugin_running.ClosePlugin();
            plugin_running = null;
        }
        Plugin settingPlugin = new Plugin();
        plugin_running = settingPlugin;
        SharedVariables.uIcontroller.OpenPlugin();
    }

}
