using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eStageState
{
    Play,
    CheckTileMoveDone,
    CheckMatch,
    GameOver,
    GameClear,
}

public class StageManager : MonoBehaviour
{
    static StageManager _instance;
    public static StageManager Instance
    {
        get { return _instance; }
    }

    public GameObject tileParent;
    public GameObject blockParent;
    public Tile[] blockCreateTiles;
    public StageData data => _data;
    public int targetCount => _targetCount;
    public int switchCount => _switchCount;
    public int score => _score;

    private List<Tile> tileList;
    private StageData _data;
    private UI_StageInfo _uiStage;

    private Queue<Block> _blockPool;
    private List<Block> _useBlockList;

    public bool isBlockSwitch = false;
    public bool isMoveBlock => isBlockSwitch == true || _state != eStageState.Play;

    private eStageState _state;
    private int _targetCount = 0;
    private int _switchCount = 0;
    private int _score = 0;
    private int _combo = 0;

    public List<eTileDirection> directionList = new List<eTileDirection>() { 
        eTileDirection.Top, eTileDirection.TopLeft, eTileDirection.TopRight, 
        eTileDirection.Bottom, eTileDirection.BottomLeft, eTileDirection.BottomRight };

    public void Init(StageData data)
    {
        _instance = this;
        _data = data;

        _uiStage = UIManager.Instance.CreateUI<UI_StageInfo>(eUI_Type.StageInfo, eUILayer.Layer_1, false);
        _uiStage.SetData(_data);

        _blockPool = new Queue<Block>();
        _useBlockList = new List<Block>();

        tileList = new List<Tile>();
        if (tileParent != null)
            tileParent.GetComponentsInChildren<Tile>(tileList);

        foreach (Tile tile in tileList)
            tile.Init();

        isBlockSwitch = false;
        _targetCount = _data.targetCount;
        _switchCount = _data.switchCount;
        _score = 0;
        _state = eStageState.Play;
    }

    private void Update()
    {
        if (_state != eStageState.CheckTileMoveDone)
            return;

        bool isMove = false;
        foreach(Block block in _useBlockList)
        {
            if (block.isMove == true)
                isMove = true;
        }

        if (isMove == false)
        {
            CheckMatchAllTiles();
        }
    }

    public void Test()
    {
        _instance = this;
        _blockPool = new Queue<Block>();
        _useBlockList = new List<Block>();
        tileList = new List<Tile>();
        if (tileParent != null)
            tileParent.GetComponentsInChildren<Tile>(tileList);

        foreach (Tile tile in tileList)
            tile.Init();

        isBlockSwitch = false;
    }

    public void Test2()
    {
        
    }

    private Block CreateBlock()
    {
        GameObject obj = Resources.Load<GameObject>("Block");
        GameObject instObj = Instantiate(obj, blockParent.transform);
        Block block = instObj.GetComponent<Block>();
        block.InitCreate();
        block.isActive = false;
        return block;
    }
    private void RemoveBlock(Block block)
    {
        block.gameObject.SetActive(false);
        block.Clear();
        block.isActive = false;
        _useBlockList.Remove(block);
        _blockPool.Enqueue(block);
    }

    public Block CreateBlock(Tile tile)
    {
        if (blockParent == null)
            return null;

        Block block;
        if (_blockPool.Count > 0)
            block = _blockPool.Dequeue();
        else
            block = CreateBlock();

        block.gameObject.SetActive(true);
        block.transform.position = tile.transform.position;
        block.Init(tile);
        block.isActive = true;

        _useBlockList.Add(block);

        return block;
    }

    public void CheckMatchAllTiles()
    {
        List<Block> totalMatchList = new List<Block>();
        foreach (Block block in _useBlockList)
        {
            List<Block> matchList = block.CheckMatch();
            if(matchList.Count >= Const.MinMatchCount)
            {
                totalMatchList.AddRange(matchList);
            }
        }

        if (totalMatchList.Count > 0)
        {
            _combo++;
            ProcessMatch(totalMatchList);
        }
        else
        {
            _state = eStageState.Play;
        }
    }

    public void ProcessMatch(List<Block> matchList)
    {
        _state = eStageState.CheckMatch;
        StartCoroutine(C_ProcessMatch(matchList));
    }

    IEnumerator C_ProcessMatch(List<Block> matchList)
    {
        WaitForSeconds wf = GameManager.Instance.GetWaitForSeconds(0.3f);
        yield return wf;

        List<Tile> emptyList = new List<Tile>();
        HashSet<Block> checkList = new HashSet<Block>();
        HashSet<Tile> checkTileList = new HashSet<Tile>();
        _combo++;

        foreach (Block block in matchList)
        {
            if (block.isActive == false)
                continue;

            foreach(eTileDirection dir in directionList)
            {
                Block dBlock = block[dir];
                if (dBlock == null || checkList.Contains(dBlock) == true)
                    continue;
                checkList.Add(dBlock);

                if (dBlock.blockType == _data.targetType)
                {
                    _score += 10 * _combo;
                    if(dBlock.CheckTarget() == true)
                    {
                        _targetCount--;
                        dBlock.SetBlockOutLineAll(false);
                        emptyList.Add(dBlock.currentTile);
                        if(_uiStage != null)
                            _uiStage.MakeEffect(dBlock);

                        if(dBlock.isRemoveable == true)
                            RemoveBlock(dBlock);
                    }
                }
            }

            if (block.currentTile != null && checkTileList.Contains(block.currentTile) == false)
            {
                checkTileList.Add(block.currentTile);

                if (block.currentTile.CheckTile() == true)
                {
                    if (block.currentTile.tileType == _data.targetType)
                        _targetCount--;

                    if (_uiStage != null)
                        _uiStage.MakeEffect(block.transform.position, block.currentTile.tileType);

                    block.currentTile.ResetTile();
                }
            }

            block.SetBlockOutLineAll(false);
            emptyList.Add(block.currentTile);
            RemoveBlock(block);
        }


        if(_uiStage != null)
        {
            _uiStage.UpdateSwitchCount(_switchCount);
            _uiStage.UpdateScore(_score, _data.GetGrade(_score));
        }

        yield return wf;


        foreach (Tile tile in emptyList)
        {
            if (tile == null)
                continue;
            tile.MoveDown();
        }
        _state = eStageState.CheckTileMoveDone;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (_targetCount <= 0)
        {
            GameClear();
        }
        else if (_switchCount <= 0)
            GameOver();
    }
   
    public void OnSwitch()
    {
        --_switchCount;
    }

    public void GameClear()
    {
        _state = eStageState.GameClear;
        GameManager.Instance.StageClear(_data.index, _data.GetGrade(_score));
        GameManager.Instance.userdata.AddGold(_data.index);
        UIManager.Instance.CreateUI<UI_Loading>(eUI_Type.Loading, eUILayer.Loading, false, () =>
        {
            GameManager.Instance.LoadLobby();
        });
    }

    public void GameOver()
    {
        _state = eStageState.GameOver;

        Clear();

        GameManager.Instance.userdata.UseHeart();

        UIManager.Instance.CreateUI<UI_Loading>(eUI_Type.Loading, eUILayer.Loading, false, () =>
        {
            GameManager.Instance.LoadLobby();
        });
    }

    public void Clear()
    {
        if (_uiStage != null)
            _uiStage.Close();

        StopAllCoroutines();
        _instance = null;
        Destroy(gameObject);
    }
}
