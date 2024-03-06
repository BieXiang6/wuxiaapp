using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TMPObjectPool:MonoBehaviour
{
    private readonly Queue<GameObject> _pool;
    private readonly GameObject Pool_GameObject;
    private GameObject exm;
    private ResourceLoader RL;

    public TMPObjectPool()
    {
        _pool = new Queue<GameObject>();
        Pool_GameObject = GameObject.Find("TMPPool");
        if (SharedVariables.RL == null)
        {
            RL = GameObject.Find("Global controller").GetComponent<ResourceLoader>();
            SharedVariables.RL = RL;
        }
        RL = SharedVariables.RL;
        exm = RL.GetObject(RL.OutText);
    }

    public GameObject GetObject(Transform parent,string text,float fontsize,Vector2 sizeDelta)
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.transform.SetParent(parent,false);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.SetActive(true);
            obj.GetComponent<TMP_Text>().text = text;
            return obj;
        }

        var gameObject = Instantiate(exm);
        gameObject.transform.SetParent(parent,false);
        gameObject.GetComponent<TMP_Text>().text = text;
        gameObject.GetComponent<TMP_Text>().fontSize = fontsize;
        gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        return gameObject;
    }

    public void ReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(Pool_GameObject.transform);
        _pool.Enqueue(obj);
    }
}


public class ButtonObjectPool:MonoBehaviour
{
    private readonly Queue<GameObject> _pool;
    private readonly GameObject Pool_GameObject;
    private GameObject exm;
    private ResourceLoader RL;

    public ButtonObjectPool()
    {
        _pool = new Queue<GameObject>();
        Pool_GameObject = GameObject.Find("ButtonPool");
        if (SharedVariables.RL == null)
		{
			RL = GameObject.Find("Global controller").GetComponent<ResourceLoader>();
			SharedVariables.RL = RL;
		}
		RL = SharedVariables.RL;
        exm = RL.GetObject(RL.NormalButton);
    }
    public GameObject GetObject(Transform parent, string text, string name, Vector2 sizeDelta,float fontsize)
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.transform.SetParent(parent, false);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.SetActive(true);
            obj.GetComponentInChildren<TMP_Text>().text = text;
            obj.GetComponentInChildren<TMP_Text>().fontSize = fontsize;
            obj.name = name;
            obj.GetComponent<RectTransform>().sizeDelta = sizeDelta;
            return obj;
        }

        var gameObject = Instantiate(exm);
        gameObject.transform.SetParent(parent, false);
        gameObject.GetComponentInChildren<TMP_Text>().text = text;
        gameObject.GetComponentInChildren<TMP_Text>().fontSize = fontsize;
        gameObject.name = name;
        gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        return gameObject;
    }
    public GameObject GetObject(Transform parent, string text, string name, float fontsize)
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.transform.SetParent(parent, false);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.SetActive(true);
            obj.GetComponentInChildren<TMP_Text>().text = text;
            obj.GetComponentInChildren<TMP_Text>().fontSize = fontsize;
            obj.name = name;
            return obj;
        }

        var gameObject = Instantiate(exm);
        gameObject.transform.SetParent(parent, false);
        gameObject.GetComponentInChildren<TMP_Text>().text = text;
        gameObject.GetComponentInChildren<TMP_Text>().fontSize = fontsize;
        gameObject.name = name;
        return gameObject;
    }

    public void ReleaseObject(GameObject obj)
    {
        if (obj == null)
            return;
        obj.SetActive(false);
        obj.transform.SetParent(Pool_GameObject.transform,false);
        _pool.Enqueue(obj);
    }

}
