using UnityEngine;
using UnityEngine.UI;

public class UI_StagePausePopup : UIBase
{
    public Button btnQuit;
    public Button btnRestart;
    public Button btnCancel;

    protected override void Init()
    {
        base.Init();

        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnClickQuit);
        if (btnRestart != null)
            btnRestart.onClick.AddListener(OnClickRestart);
        if (btnCancel != null)
            btnCancel.onClick.AddListener(Close);
    }

    private void OnClickQuit()
    {
        if (StageManager.Instance == null)
            return;

        Close();

        StageManager.Instance.GameOver();
    }

    private void OnClickRestart()
    {
        if (StageManager.Instance == null)
            return;

        UI_StageEnterPopup ui = UIManager.Instance.CreateUI<UI_StageEnterPopup>(eUI_Type.StageEnterPopup, eUILayer.Popup, true, StageManager.Instance.GameOver);
        if(ui != null)
            ui.SetData(StageManager.Instance.data);
    }
}
