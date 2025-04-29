using UnityEngine;
using UnityEngine.UI;

public class UI_StageEnterPopup : UIBase
{
    public TMPro.TextMeshProUGUI txtTitle;
    public Image imgTarget;
    public TMPro.TextMeshProUGUI txtTargetCount;
    public Button btnCancel;
    public Button btnStart;

    StageData _data;

    protected override void Init()
    {
        base.Init();

        if (btnCancel != null)
            btnCancel.onClick.AddListener(Close);
        if (btnStart != null)
            btnStart.onClick.AddListener(OnClickStart);
    }

    public void SetData(StageData data)
    {
        _data = data;
        if (_data == null)
            return;

        if (txtTitle != null)
            txtTitle.text = $"·¹º§ {_data.index}";
        if(imgTarget != null)
            imgTarget.sprite = Resources.Load<Sprite>(_data.targetType.ToDescription());
        if (txtTargetCount != null)
            txtTargetCount.text = _data.targetCount.ToString();
    }

    public void OnClickStart()
    {
        _closeCB = null;
        UIManager.Instance.ClearUI();

        UIManager.Instance.CreateUI<UI_Loading>(eUI_Type.Loading, eUILayer.Loading, false, () =>
        {
            GameManager.Instance.StartStage(_data);
        });
    }
}
