using System;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public const int heartCountMax = 5;
    public long heartFillMinute => TimeSpan.TicksPerSecond * 15;
    public int heartCount = 5;
    public long nextHeartFillTick = 0;
    public bool isMaxHeart { get { return heartCount >= heartCountMax; } }

    public const int goldMax = 10000;
    public int gold = 100;

    public int stageIdx = 0;
    public Dictionary<int, int> stageClearDic = null;

    public void Init()
    {
        heartCount = heartCountMax;
        gold = 100;
        stageClearDic = new Dictionary<int, int>();
        stageIdx = 0;
    }
    
    public void Update()
    {
        if (DateTime.Now.Ticks >= nextHeartFillTick)
            AddHeart();
    }

    public bool UseHeart()
    {
        if (heartCount <= 0)
            return false;

        heartCount -= 1;
        SetHeartFillTime();

        UI_MainInfo ui = UIManager.Instance.GetUI<UI_MainInfo>();
        if(ui != null)
        {
            ui.Refresh();
        }

        return true;
    }

    public bool AddHeart()
    {
        if (heartCount >= heartCountMax)
            return false;

        heartCount += 1;
        if (heartCount < heartCountMax)
            SetHeartFillTime();

        UI_MainInfo ui = UIManager.Instance.GetUI<UI_MainInfo>();
        if (ui != null)
        {
            ui.Refresh();
        }

        return true;
    }

    private void SetHeartFillTime()
    {
        if (nextHeartFillTick > DateTime.Now.Ticks)
            return;

        nextHeartFillTick = DateTime.Now.Ticks + heartFillMinute;
    }

    public bool AddGold() => AddGold(100);
    public bool AddGold(int value)
    {
        if (gold >= goldMax)
            return false;

        gold += value;
        if (gold > goldMax)
            gold = goldMax;
        return true;
    }

    public bool IsClearStage(int index)
    {
        return stageClearDic.ContainsKey(index) == true;
    }

    public int GetStageClearGrade(int index)
    {
        if (stageClearDic.ContainsKey(index) == true)
            return stageClearDic[index];
        return 0;
    }

    public void StageClear(int index, int grade)
    {
        stageIdx = Math.Max(stageIdx, index);

        if (stageClearDic.ContainsKey(index) == false)
        {
            stageClearDic.Add(index, grade);
            return;
        }

        stageClearDic[index] = Math.Max(grade, stageClearDic[index]);
    }
}
