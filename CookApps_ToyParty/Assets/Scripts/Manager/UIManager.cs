using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject();
                _instance = obj.AddComponent<UIManager>();
                obj.name = nameof(UIManager);
            }
            return _instance;
        }
    }

    public int uiCount
    {
        get
        {
            if (_uiStack == null)
                return 0;
            return _uiStack.Count;
        }
    }

    GameObject _rootUI;
    Stack<UIBase> _uiStack;
    List<UIBase> _uiList;
    
    public void Init()
    {
        GameObject rootUI = Resources.Load<GameObject>("UI/RootUI");

        if(rootUI == null)
        {
            return;
        }
        _rootUI = Instantiate(rootUI, this.transform);
        _uiStack = new Stack<UIBase>();
        _uiList = new List<UIBase>();

        for (int i = 0; i < (int)eUILayer.Count; ++i)
        {
            eUILayer layer = (eUILayer)i;
            GameObject child = new GameObject(layer.ToString());
            RectTransform rt = child.AddComponent<RectTransform>();
            child.transform.SetParent(_rootUI.transform);
            child.transform.SetAsLastSibling();
            child.transform.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
        }
    }

    public T CreateUI<T>(eUI_Type type, eUILayer layer = eUILayer.Layer_1, bool isStackUI = true, System.Action closeCB = null) where T : UIBase
    {
        T item = GetUI<T>();
        if (item == null)
        {
            string fileName = type.ToDescription();
            if (string.IsNullOrEmpty(fileName) == true)
                return null;

            GameObject newUI = Resources.Load<GameObject>(fileName);
            if (newUI == null)
                return null;

            GameObject instUI = Instantiate(newUI, _rootUI.transform.GetChild((int)layer));
            if (instUI == null)
                return null;

            instUI.transform.SetAsLastSibling();

            item = instUI.GetComponent<T>();

            item.Init(isStackUI);

            if (isStackUI == true)
                _uiStack.Push(item);
            _uiList.Add(item);
        }

        item.Refresh();

        item.AddCloseEvent(closeCB);

        return item;
    }

    public T GetUI<T>() where T : UIBase
    {
        foreach(UIBase ui in _uiList)
        {
            if(ui is T)
            {
                return ui as T;
            }
        }
        return null;
    }

    public void ClearUI()
    {
        while(_uiList.Count > 0)
        {
            if (_uiList[0] != null)
                _uiList[0].Close();
        }
    }

    public void CloseAll()
    {
        while(_uiStack.Count > 0)
        {
            if (_uiStack.TryPop(out UIBase ui) == false)
                continue;
            if (ui == null)
                CloseUI();

            ui.Close();
        }
    }

    public void CloseUI()
    {
        if (_uiStack.TryPop(out UIBase ui) == false)
            return;
        if (ui == null)
            CloseUI();

        ui.Close();
    }

    public void RemoveUI(UIBase item)
    {
        _uiList.Remove(item);
        Stack<UIBase> stack = new Stack<UIBase>();
        UIBase peek = null;
        while(_uiStack.Count > 0)
        {
            peek = _uiStack.Pop();
            if (peek == item)
                break;

            stack.Push(peek);
            peek = null;
        }

        while (stack.Count > 0)
            _uiStack.Push(stack.Pop());

    }

    public Sprite GetSocreImg(int grade)
    {
        return Resources.Load<Sprite>($"Score_{grade}");
    }
}
