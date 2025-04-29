using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public string prefabPath = "";
    public int index = 0;
    public int switchCount = 0;
    public eBlockType targetType = eBlockType.Empty;
    public int targetCount = 0;
    public int[] scoreList;

    public int GetGrade(int score)
    {
        if (scoreList == null)
            return 0;
        for(int i = 0; i < 3 && i < scoreList.Length; ++i)
        {
            if (score < scoreList[i])
                return i;
        }
        return 2;
    }
}
