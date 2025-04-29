using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_MainInfo : UIBase
{
    public const string prefabName = "UI_MainInfo";

    public TMPro.TextMeshProUGUI txtHeartCount = null;
    public TMPro.TextMeshProUGUI txtHeartFillTime = null;
    public TMPro.TextMeshProUGUI txtGoldCount = null;

    public Button btnAddHeart = null;
    public Button btnAddGold = null;
    public Button btnSetting = null;

    protected override void Init()
    {
        base.Init();

        if (btnAddHeart != null)
            btnAddHeart.onClick.AddListener(OnClickAddHeart);
        if (btnAddGold != null)
            btnAddGold.onClick.AddListener(OnClickAddGold);
        if (btnSetting != null)
            btnSetting.onClick.AddListener(OnClickSetting);

        StartCoroutine(C_RefreshTime());
    }

    public override void Refresh()
    {
        base.Refresh();

        RefreshHeartCount();
        RefreshHeartFillTime();
        RefreshGoldCount();
    }

    private void RefreshHeartCount()
    {
        if (txtHeartCount != null)
            txtHeartCount.text = GameManager.Instance.userdata.heartCount.ToString();
    }

    private void RefreshHeartFillTime()
    {
        if (GameManager.Instance.userdata.isMaxHeart == true)
        {
            if (txtHeartFillTime != null)
                txtHeartFillTime.text = "°¡µæÂü";
        }
        else
        {
            if (txtHeartFillTime != null)
            {
                long remainTick = GameManager.Instance.userdata.nextHeartFillTick - System.DateTime.Now.Ticks;
                System.TimeSpan remainSpan = System.TimeSpan.FromTicks(remainTick);
                txtHeartFillTime.text = remainSpan.ToString(@"mm\:ss");
            }
        }
    }

    private void RefreshGoldCount()
    {
        if (txtGoldCount != null)
            txtGoldCount.text = GameManager.Instance.userdata.gold.ToString();
    }

    private void OnClickAddHeart()
    {
        GameManager.Instance.userdata.AddHeart();
        RefreshHeartCount();
    }

    private void OnClickAddGold()
    {
        GameManager.Instance.userdata.AddGold();
        RefreshGoldCount();
    }

    private void OnClickSetting()
    {
        UIManager.Instance.CreateUI<UI_Setting>(eUI_Type.Setting, eUILayer.Popup);
    }

    IEnumerator C_RefreshTime()
    {
        WaitForSecondsRealtime wf = GameManager.Instance.GetWaitForSecondRealTime(1f);
        while(true)
        {
            RefreshHeartFillTime();
            yield return wf;
        }
    }
}
