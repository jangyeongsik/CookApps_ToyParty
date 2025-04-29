using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    UserData _userdata = null;
    public UserData userdata { get { return _userdata; } }

    public bool isStartGame { get; set; } = false;

    Dictionary<float, WaitForSeconds> _wfsDic = null;
    Dictionary<float, WaitForSecondsRealtime> _wfsRTDic = null;

    private void Awake()
    {
        _instance = this;

        Screen.SetResolution(1080, 1920, true);
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        UIManager.Instance.Init();
        SoundManager.Instance.Init();

        if (currentScene.name != "StageMakeScene") 
            UIManager.Instance.CreateUI<UI_StartPage>(eUI_Type.StartPage, eUILayer.Layer_1, false);

        _userdata = new UserData();
        _userdata.Init();

        _wfsDic = new Dictionary<float, WaitForSeconds>();
        _wfsRTDic = new Dictionary<float, WaitForSecondsRealtime>();
        isStartGame = false;
    }

    public void LoadLobby()
    {
        UIManager.Instance.CreateUI<UI_MainInfo>(eUI_Type.MainInfo, eUILayer.Layer_2, false);
        UIManager.Instance.CreateUI<UI_Stage>(eUI_Type.Stage, eUILayer.Layer_1, false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) == true)
        {
            SoundManager.Instance.PlaySFX(eSFX_Type.Click);
        }
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            userdata.UseHeart();
        }
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (UIManager.Instance.uiCount > 0)
                UIManager.Instance.CloseUI();
            else
                UIManager.Instance.CreateUI<UI_QuitGame>(eUI_Type.QuitGame, eUILayer.Popup);
        }

        if(isStartGame == true)
        {
            if(userdata != null)
            {
                userdata.Update();
            }
        }
    }

    public void StartStage(StageData data)
    {
        if(StageManager.Instance != null)
        {
            StageManager.Instance.Clear();
        }
        GameObject prefab = Resources.Load<GameObject>(data.prefabPath);
        GameObject instObj = Instantiate(prefab);
        instObj.transform.position = Vector3.zero;
        StageManager manager = instObj.GetComponent<StageManager>();
        manager.Init(data);
    }

    public void StageClear(int index, int grade)
    {
        if (_userdata != null)
            _userdata.StageClear(index, grade);
    }

    public WaitForSeconds GetWaitForSeconds(float time)
    {
        if(_wfsDic.TryGetValue(time, out WaitForSeconds res) == false)
        {
            res = new WaitForSeconds(time);
            _wfsDic.Add(time, res);
        }
        return res;
    }
    public WaitForSecondsRealtime GetWaitForSecondRealTime(float time)
    {
        if (_wfsRTDic.TryGetValue(time, out WaitForSecondsRealtime res) == false)
        {
            res = new WaitForSecondsRealtime(time);
            _wfsRTDic.Add(time, res);
        }
        return res;
    }
}
