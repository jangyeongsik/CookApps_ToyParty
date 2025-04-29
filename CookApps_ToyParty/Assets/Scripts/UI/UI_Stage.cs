using UnityEngine;
using System.Collections.Generic;

public class UI_Stage : UIBase
{
    List<UI_StageItem> _stageItemList = null;
    protected override void Init()
    {
        base.Init();

        _stageItemList = new List<UI_StageItem>();
        gameObject.GetComponentsInChildren<UI_StageItem>(_stageItemList);

        foreach (UI_StageItem item in _stageItemList)
        {
            item.Init(this);
        }
    }

    public override void Refresh()
    {
        base.Refresh();

        foreach(UI_StageItem item in _stageItemList)
        {
            item.Refresh();
        }
    }
}
