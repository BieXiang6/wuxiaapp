using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PluginClick : MonoBehaviour, IPointerClickHandler
{
    public string pluginName;

    public void OnPointerClick(PointerEventData eventData)
    {
            SharedVariables.PC.OpenPlugin(pluginName);
    }
}
