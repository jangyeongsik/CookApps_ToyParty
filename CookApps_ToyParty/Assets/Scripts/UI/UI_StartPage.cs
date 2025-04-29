using UnityEngine;
using UnityEngine.UI;

public class UI_StartPage : UIBase
{
    public const string prefabName = "UI_StartPage";
    public Button btnStart = null;

    protected override void Init()
    {
        base.Init();

        btnStart.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        Close();

        UIManager.Instance.CreateUI<UI_Loading>(eUI_Type.Loading, eUILayer.Loading, false, () =>
        {
            GameManager.Instance.LoadLobby();
            GameManager.Instance.isStartGame = true;
        });
    }
}
