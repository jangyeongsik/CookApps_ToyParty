using UnityEngine;
using UnityEngine.UI;

public class UI_QuitGame : UIBase
{
    public Button btnQuit;
    public Button btnClose;

    protected override void Init()
    {
        base.Init();

        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnClickQuit);
        if (btnClose != null)
            btnClose.onClick.AddListener(Close);
    }

    private void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
