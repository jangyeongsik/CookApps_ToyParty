using UnityEngine;
using UnityEngine.Events;

public abstract class UIBase : MonoBehaviour
{
    protected bool _isStackUI = true;
    protected System.Action _closeCB = null;

    public void AddCloseEvent(System.Action cb)
    {
        _closeCB = cb;
    }
    protected virtual void OnClose()
    {

    }

    public void Init(bool isStackUI)
    {
        _isStackUI = isStackUI;
        Init();
    }
    protected virtual void Init()
    {

    }

    public virtual void Refresh()
    {

    }

    public void Close()
    {
        _closeCB?.Invoke();

        OnClose();
        Destroy(gameObject);
        UIManager.Instance.RemoveUI(this);
    }
}
