using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_StageInfo : UIBase
{
    public Image imgTarget;
    public Coffee.UIExtensions.UIParticleAttractor uiEffect;
    public TMPro.TextMeshProUGUI txtTargetCount;
    public TMPro.TextMeshProUGUI txtSwitchCount;
    public TMPro.TextMeshProUGUI txtScore;
    public Image imgScore;
    public Button btnPause;

    private StageData _data;

    List<GameObject> _particleList = new List<GameObject>();
    int _attatchCount = 0;
    int _targetCount = 0;

    protected override void Init()
    {
        base.Init();
    }

    public void SetData(StageData data)
    {
        _data = data;

        _targetCount = _data.targetCount;

        if (imgTarget != null)
            imgTarget.sprite = Resources.Load<Sprite>(data.targetType.ToDescription());

        if (btnPause != null)
            btnPause.onClick.AddListener(OnClickPause);
     
        UpdateTargetCount(_targetCount);
        UpdateSwitchCount(_data.switchCount);
        UpdateScore(0, 0);
    }

    public void UpdateTargetCount(int currentTarget)
    {
        if (_data == null)
            return;

        if (currentTarget < 0)
            currentTarget = 0;
        if (txtTargetCount != null)
            txtTargetCount.text = currentTarget.ToString();
    }

    public void UpdateSwitchCount(int currentCount)
    {
        if (currentCount < 0)
            currentCount = 0;
        if (txtSwitchCount != null)
            txtSwitchCount.text = currentCount.ToString();
    }

    public void UpdateScore(int score, int grade)
    {
        if (imgScore != null)
            imgScore.sprite = UIManager.Instance.GetSocreImg(grade);
        if (txtScore != null)
            txtScore.text = score.ToString();
    }

    public void OnClickPause()
    {
        if (StageManager.Instance == null)
            return;
        if (StageManager.Instance.isMoveBlock == true)
            return;

        UIManager.Instance.CreateUI<UI_StagePausePopup>(eUI_Type.StagePausePopup, eUILayer.Popup);
    }

    public void MakeEffect(Block block)
    {
        MakeEffect(block.transform.position, block.blockType);
    }

    public void MakeEffect(Vector3 pos, eBlockType type)
    {
        if (uiEffect == null)
            return;

        _attatchCount = 0;
        string path = string.Empty;
        if (Const.BlockTargetEffect.TryGetValue(type, out path) == false)
            return;

        if (string.IsNullOrEmpty(path) == true)
            return;
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
            return;
        GameObject instObj = Instantiate(obj, transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        screenPos.z = 0;
        instObj.transform.position = screenPos;

        ParticleSystem particle = instObj.GetComponentInChildren<ParticleSystem>();
        if (particle == null)
            return;

        uiEffect.AddParticleSystem(particle);

        particle.Play();
        _particleList.Add(instObj);
    }

    public void EffectEnd()
    {
        ++_attatchCount;
        if(_attatchCount >= _particleList.Count)
        {
            for(int i = _particleList.Count - 1; i >= 0; --i)
            {
                Destroy(_particleList[i]);
            }
            _particleList.Clear();
        }

        UpdateTargetCount(--_targetCount);
    }
}
