using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    static eTileDirection _createDirection = eTileDirection.Top;
    float _length = 0.86f;
    StageManager _stageManager;

    private void OnEnable()
    {
        _stageManager = Object.FindFirstObjectByType<StageManager>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        Tile currentTile = target as Tile;

        _length = EditorGUILayout.FloatField("Length", _length);
        _createDirection = (eTileDirection)EditorGUILayout.EnumPopup("Create Direction", _createDirection);
        if (GUILayout.Button("Create") == true)
        {
            Vector3 dir = GetDirection(_createDirection);
            Tile findEndTile = GetEndTile(_createDirection, currentTile);
            GameObject loadObj = Resources.Load<GameObject>("Tile");
            GameObject instObject = Instantiate(loadObj, findEndTile.transform.parent);
            ConvertToPrefabInstanceSettings setting = new ConvertToPrefabInstanceSettings();
            PrefabUtility.ConvertToPrefabInstance(instObject, loadObj, setting, InteractionMode.AutomatedAction);

            instObject.transform.localPosition = findEndTile.transform.localPosition + dir;
            instObject.name = $"Tile{instObject.GetHashCode()}";
            Tile newTile = instObject.GetComponent<Tile>();
            AddTile(_createDirection, findEndTile, newTile);

            List<Tile> tileList = new List<Tile>();
            _stageManager.GetComponentsInChildren<Tile>(tileList);

            List<Tile> findList = new List<Tile>();
            foreach (Tile item in tileList)
            {
                if (item == findEndTile)
                    continue;
                else if (item == newTile)
                    continue;

                if (item.x == newTile.x && Mathf.Abs(item.y - newTile.y) == 2)
                {
                    findList.Add(item);
                    continue;
                }

                if (Mathf.Abs(item.x - newTile.x) > 1)
                    continue;
                if (Mathf.Abs(item.y - newTile.y) > 1)
                    continue;

                findList.Add(item);
            }

            foreach (Tile t in findList)
            {
                int e = 0;
                if (t.x > newTile.x)
                    e += (int)eTileDirection.Right;
                if (t.x < newTile.x)
                    e += (int)eTileDirection.Left;
                if (t.y > newTile.y)
                    e += (int)eTileDirection.Top;
                if (t.y < newTile.y)
                    e += (int)eTileDirection.Bottom;

                eTileDirection findDir = (eTileDirection)e;
                AddTile(findDir, newTile, t);
            }

        }

        if(GUILayout.Button("Load Block Sprite") == true)
        {

        }

        if(GUILayout.Button("Test") == true)
        {
            currentTile.MoveDown();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private Vector3 GetDirection(eTileDirection direction)
    {
        Vector3 dir = Vector3.zero;

        switch (direction)
        {
            case eTileDirection.Top:
                dir.y = 1f;
                break;
            case eTileDirection.TopLeft:
                dir.x = Mathf.Sin(Mathf.Deg2Rad * 60f) * -1;
                dir.y = Mathf.Cos(Mathf.Deg2Rad * 60f);
                break;
            case eTileDirection.TopRight:
                dir.x = Mathf.Sin(Mathf.Deg2Rad * 60f);
                dir.y = Mathf.Cos(Mathf.Deg2Rad * 60f);
                break;
            case eTileDirection.Bottom:
                dir.y = -1f;
                break;
            case eTileDirection.BottomLeft:
                dir.x = Mathf.Sin(Mathf.Deg2Rad * 60f) * -1;
                dir.y = Mathf.Cos(Mathf.Deg2Rad * 60f) * -1;
                break;
            case eTileDirection.BottomRight:
                dir.x = Mathf.Sin(Mathf.Deg2Rad * 60f);
                dir.y = Mathf.Cos(Mathf.Deg2Rad * 60f) * -1;
                break;
            default:
                break;
        }

        return dir * _length;
    }

    public bool AddTile(eTileDirection direction,Tile target, Tile tile)
    {
        if (direction >= eTileDirection.End)
            return false;

        Tile find = target;
        if (find[direction] != null)
        {
            find = GetEndTile(direction, find[direction]);
        }

        SerializedObject sFind = new SerializedObject(find);
        SerializedObject sTile = new SerializedObject(tile);

        find[direction] = tile;
        sFind.FindProperty($"{direction}Tile").objectReferenceValue = tile;

        eTileDirection dir = direction.GetReflect();
        tile[dir] = find;
        sTile.FindProperty($"{dir}Tile").objectReferenceValue = find;

        tile.x = find.x;
        tile.y = find.y;
        eTileDirection compare;
        bool isLeftRight = false;

        compare = direction & eTileDirection.Left;
        if (compare == eTileDirection.Left)
        {
            tile.x--;
            isLeftRight = true;
        }
        compare = direction & eTileDirection.Right;
        if (compare == eTileDirection.Right)
        {
            tile.x++;
            isLeftRight = true;
        }

        compare = direction & eTileDirection.Top;
        if (compare == eTileDirection.Top)
            tile.y += (isLeftRight == false) ? 2 : 1;
        compare = direction & eTileDirection.Bottom;
        if (compare == eTileDirection.Bottom)
            tile.y -= (isLeftRight == false) ? 2 : 1;

        sTile.FindProperty("x").intValue = tile.x;
        sTile.FindProperty("y").intValue = tile.y;

        sFind.ApplyModifiedProperties();
        sTile.ApplyModifiedProperties();

        return true;
    }

    public Tile GetEndTile(eTileDirection direction, Tile target)
    {
        if (target[direction] == null)
            return target;

        return GetEndTile(direction, target[direction]);
    }
}
