using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CustomEditor(typeof(StageManager))]
public class StageManagerEditor : Editor
{
    eBlockType _blockType = eBlockType.Empty;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StageManager stageManager = target as StageManager;

        if(GUILayout.Button("Test") == true)
        {
            stageManager.Test();
        }
        if (GUILayout.Button("Test2") == true)
        {
            stageManager.Test2();
        }

        _blockType = (eBlockType)EditorGUILayout.EnumPopup("Find Type", _blockType);
        if (GUILayout.Button("Find All") == true)
        {
            var tileList = stageManager.GetComponentsInChildren<Tile>();
            if (tileList == null)
                return;
            int cnt = 0;
            foreach(Tile tile in tileList)
            {
                if (tile.tileType == _blockType)
                    cnt++;
            }
            foreach (Tile tile in tileList)
            {
                if (tile.blockType == _blockType)
                    cnt++;
            }

            Debug.Log($"{_blockType} Count : {cnt}");
        }

    }
}
