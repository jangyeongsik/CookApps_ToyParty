using System.ComponentModel;

public enum eUILayer
{
    Layer_1,
    Layer_2,
    Layer_3,
    Popup,
    Loading,

    Count
}

public enum eUI_Type
{
    None,
    [Description("UI/UI_StartPage")] StartPage,
    [Description("UI/UI_Loading")] Loading,
    [Description("UI/UI_MainInfo")] MainInfo,
    [Description("UI/UI_Stage")] Stage,
    [Description("UI/UI_Setting")] Setting,
    [Description("UI/UI_QuitGame")] QuitGame,
    [Description("UI/UI_StageInfo")] StageInfo,
    [Description("UI/UI_StagePausePopup")] StagePausePopup,
    [Description("UI/UI_StageEnterPopup")] StageEnterPopup,
    
}