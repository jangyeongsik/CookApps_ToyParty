using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;

public class UI_StageItem : MonoBehaviour
{
    public StageData stageData;

    public Button btn;
    public UIEffect grayScaleSlot;
    public TMPro.TextMeshProUGUI txtStage;
    public Image imgClearGrade;

    UI_Stage _parent = null;

    private int _index = 0;

    public void Init(UI_Stage parent)
    {
        _parent = parent;
        if (stageData != null)
            _index = stageData.index;

        if (btn != null)
            btn.onClick.AddListener(OnClickStage);

        if (txtStage != null)
            txtStage.text = _index.ToString();
    }

    public void Refresh()
    {
        bool isClear = GameManager.Instance.userdata.IsClearStage(_index);
        bool isCurrentStage = _index == GameManager.Instance.userdata.stageIdx + 1;
        int clearGrade = GameManager.Instance.userdata.GetStageClearGrade(_index);
        if(grayScaleSlot != null)
        {
            grayScaleSlot.toneIntensity = (isClear == true || isCurrentStage == true) ? 0 : 1;
        }

        if(imgClearGrade != null)
        {
            imgClearGrade.gameObject.SetActive(isClear == true);
            if(isClear == true)
            {
                if(_parent != null)
                {
                    imgClearGrade.sprite = UIManager.Instance.GetSocreImg(clearGrade);
                }
            }
        }
    }

    private void OnClickStage()
    {
        bool isCurrentStage = _index == GameManager.Instance.userdata.stageIdx + 1;
        bool isClear = GameManager.Instance.userdata.IsClearStage(_index);
        if (isClear == false && isCurrentStage == false)
            return;
        UI_StageEnterPopup ui = UIManager.Instance.CreateUI<UI_StageEnterPopup>(eUI_Type.StageEnterPopup, eUILayer.Popup);
        if(ui != null)
        {
            ui.SetData(stageData);
        }
    }
}
