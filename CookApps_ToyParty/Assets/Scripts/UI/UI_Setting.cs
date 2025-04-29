using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UIBase
{
    public Button btnClose;
    public Toggle tgBGMOn;
    public GameObject objBGMOn;
    public Toggle tgBGMOff;
    public GameObject objBGMOff;

    public Toggle tgSFXOn;
    public GameObject objSFXOn;
    public Toggle tgSFXOff;
    public GameObject objSFXOff;

    protected override void Init()
    {
        base.Init();

        if (btnClose != null)
            btnClose.onClick.AddListener(OnClickClose);
        if (tgBGMOff != null)
        {
            tgBGMOff.onValueChanged.AddListener(OnClickMuteBGM);
            tgBGMOff.SetIsOnWithoutNotify(SoundManager.Instance.isMuteBGM == true);
            SetTgIcon(SoundManager.Instance.isMuteBGM == true, objBGMOff);
        }
        if (tgBGMOn != null)
        {
            tgBGMOn.onValueChanged.AddListener(OnClickMuteBGM);
            tgBGMOn.SetIsOnWithoutNotify(SoundManager.Instance.isMuteBGM == false);
            SetTgIcon(SoundManager.Instance.isMuteBGM == false, objBGMOn);
        }

        if (tgSFXOff != null)
        {
            tgSFXOff.onValueChanged.AddListener(OnClickMuteSFX);
            tgSFXOff.SetIsOnWithoutNotify(SoundManager.Instance.isMuteSFX == true);
            SetTgIcon(SoundManager.Instance.isMuteSFX == true, objSFXOff);
        }
        if (tgSFXOn != null)
        {
            tgSFXOn.onValueChanged.AddListener(OnClickMuteSFX);
            tgSFXOn.SetIsOnWithoutNotify(SoundManager.Instance.isMuteSFX == false);
            SetTgIcon(SoundManager.Instance.isMuteSFX == false, objSFXOn);
        }

    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickMuteBGM(bool isOn)
    {
        if (isOn == false)
            return;
        SoundManager.Instance.isMuteBGM = !SoundManager.Instance.isMuteBGM;
        if (tgBGMOff != null)
        {
            tgBGMOff.SetIsOnWithoutNotify(SoundManager.Instance.isMuteBGM == true);
            SetTgIcon(SoundManager.Instance.isMuteBGM == true, objBGMOff);
        }
        if (tgBGMOn != null)
        {
            tgBGMOn.SetIsOnWithoutNotify(SoundManager.Instance.isMuteBGM == false);
            SetTgIcon(SoundManager.Instance.isMuteBGM == false, objBGMOn);
        }
    }

    private void OnClickMuteSFX(bool isOn)
    {
        if (isOn == false)
            return;
        SoundManager.Instance.isMuteSFX = !SoundManager.Instance.isMuteSFX;
        if (tgSFXOff != null)
        {
            tgSFXOff.SetIsOnWithoutNotify(SoundManager.Instance.isMuteSFX == true);
            SetTgIcon(SoundManager.Instance.isMuteSFX == true, objSFXOff);
        }
        if (tgSFXOn != null)
        {
            tgSFXOn.SetIsOnWithoutNotify(SoundManager.Instance.isMuteSFX == false);
            SetTgIcon(SoundManager.Instance.isMuteSFX == false, objSFXOn);
        }
      
    }
    private void SetTgIcon(bool isOn, GameObject obj)
    {
        if (obj != null)
            obj.SetActive(isOn);
    }
}
