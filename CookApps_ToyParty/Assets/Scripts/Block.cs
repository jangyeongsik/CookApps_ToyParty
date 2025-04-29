using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void BlockMoveCompleteCB(Block owner, eTileDirection dir);

    public bool isActive { get; set; } = false;
    public eBlockType blockType => _blockType;
    public SpriteRenderer blockIcon;
    public Tile currentTile => _currentTile;

    public bool isMove => _isMove;

    Tile _currentTile;

    Coroutine _cMoveBlock = null;
    Coroutine _cPointerCheck = null;

    private bool _isMove = false;
    private eBlockType _blockType;
    private Dictionary<eTileDirection, GameObject> _outLineDic;
    private int _targetHP = 0;

    public Block this[eTileDirection dir]
    {
        get
        {
            if (_currentTile == null)
                return null;
            if (_currentTile[dir] == null)
                return null;
            return _currentTile[dir].blockTest;
        }
    }

    public bool isMoveable
    {
        get
        {
            if (isActive == false)
                return false;
            if (_currentTile == null)
                return false;

            return _blockType.IsMoveableBlock() == true;
        }
    }

    public bool isRemoveable
    {
        get
        {
            return _blockType.IsRemoveableBlock() == true;
        }
    }

    public void InitCreate()
    {
        InitOutLine();
    }

    public void Clear()
    {
        if(_currentTile != null)
        {
            _currentTile.blockTest = null;
            _currentTile = null;
        }
        
        _blockType = eBlockType.Empty;
        StopMove();
        StopPointerCheck();
        name = "Block";
    }

    public void Init(Tile tile)
    {
        SetTile(tile);
    }

    public void SetRanomType()
    {
        SetBlockType((eBlockType)Random.Range((int)eBlockType.Blue, (int)eBlockType.Count));
    }

    public void SetBlockType(eBlockType type)
    {
        _blockType = type;
        SetBlockIcon();

        if(Const.BlockTargetHP.ContainsKey(_blockType) == true)
        {
            _targetHP = Const.BlockTargetHP[_blockType];
        }
    }

    public void SetTile(Tile tile, bool setNull = true)
    {
        if (_currentTile != null && setNull == true)
            _currentTile.blockTest = null;

        _currentTile = tile;
        _currentTile.blockTest = this;
        name = $"Block ({tile.name})";
    }

    public bool CheckTarget()
    {
        if (--_targetHP <= 0)
            return true;
        return false;
    }

    private void SetBlockIcon()
    {
        if (blockIcon == null)
            return;

        string path = _blockType.ToDescription();
        if (string.IsNullOrEmpty(path) == true)
        {
            HideBlockIcon();
        }
        if (blockIcon != null)
        {
            blockIcon.enabled = true;
            blockIcon.sprite = Resources.Load<Sprite>(path);
        }
    }

    public void HideBlockIcon()
    {
        if (blockIcon == null)
            return;

        if (blockIcon != null)
            blockIcon.enabled = false;
    }

    public void MoveDown(eTileDirection dir)
    {
        MoveBlock(dir, CompleteMoveDown);
    }

    private void CompleteMoveDown(Block owner, eTileDirection direction)
    {
        if (owner == null || owner.currentTile == null)
            return;

        owner.currentTile.MoveDown();
    }

    public List<Block> CheckMatch()
    {
        List<Block> matchList = new List<Block>();
        matchList.AddRange(this.CheckMatch(eTileDirection.Top));
        matchList.AddRange(this.CheckMatch(eTileDirection.TopRight));
        matchList.AddRange(this.CheckMatch(eTileDirection.TopLeft));

        foreach (Block block in matchList)
        {
            block.SetBlockOutLineAll(true);
        }
        foreach (Block block in matchList)
        {
            block.SetBlockOutLine(eTileDirection.Top, matchList);
            block.SetBlockOutLine(eTileDirection.TopRight, matchList);
            block.SetBlockOutLine(eTileDirection.TopLeft, matchList);
        }

        return matchList;
    }

    public List<Block> CheckMatch(eTileDirection checkDirection)
    {
        List<Block> result = new List<Block>() { this };
        result.AddRange(CheckMatch(checkDirection, checkDirection));
        result.AddRange(CheckMatch(checkDirection, checkDirection.GetReflect()));

        if (result.Count < Const.MinMatchCount)
        {
            result.Clear();
        }

        return result;
    }

    public List<Block> CheckMatch(eTileDirection checkDirection, eTileDirection tileDirection)
    {
        List<Block> result = new List<Block>();
        if (IsMatch(tileDirection) == true)
        {
            result = this[tileDirection].CheckMatch(checkDirection, tileDirection);
            result.Add(this[tileDirection]);
        }

        return result;
    }

    public bool IsMatch(eTileDirection direction)
    {
        Block target = this[direction];
        if (target == null || target.currentTile.isCreateTile == true)
            return false;
        if (_blockType >= eBlockType.Count)
            return false;

        return target._blockType == this._blockType;
    }

    public void SwitchBlock(eTileDirection direction)
    {
        Block target = this[direction];
        if (target == null)
            return;
        if (target.isMoveable == false)
            return;

        if (StageManager.Instance.isMoveBlock == true)
            return;
        StageManager.Instance.isBlockSwitch = true;

        Block block = target;
        MoveBlock(direction, CompleteSwitchBlock);
        block.MoveBlock(direction.GetReflect(), null);
    }
    
    public void CompleteSwitchBlock(Block owner, eTileDirection moveDir)
    {
        StageManager.Instance.isBlockSwitch = false;
        if (owner == null)
            return;

        Block target = owner[moveDir.GetReflect()];
        if (target == null)
            return;

        List<Block> matchList = new List<Block>();
        matchList.AddRange(owner.CheckMatch());
        matchList.AddRange(target.CheckMatch());

        if (matchList.Count == 0)
        {
            StageManager.Instance.isBlockSwitch = true;
            owner.MoveBlock(moveDir.GetReflect(), CompleteSwitchUndo, true, false, Const.MoveBlockUndoTime, Const.MoveBlockUndoDelayTime);
            target.MoveBlock(moveDir, null, true, false, Const.MoveBlockUndoTime, Const.MoveBlockUndoDelayTime);
        }
        else
        {
            StageManager.Instance.OnSwitch();
            StageManager.Instance.ProcessMatch(matchList);
        }
    }

    public void CompleteSwitchUndo(Block owner, eTileDirection moveDir)
    {
        StageManager.Instance.isBlockSwitch = false;
    }

    public void MoveBlock(eTileDirection moveDir, BlockMoveCompleteCB completeCB, bool forceMove = false, bool setNull = true, float moveTime = 0.2f, float delay = 0f)
    {
        if (forceMove == false && _isMove == true)
            return;
        if (isMoveable == false)
            return;

        StopMove();

        if (_currentTile == null)
            return;

        Tile target = _currentTile[moveDir];
        if (target == null)
            return;

        SetTile(target, setNull);

        _cMoveBlock = StartCoroutine(C_MoveBlock(moveDir, _blockType, completeCB, moveTime, delay));
    }

    public void StopMove()
    {
        _isMove = false;
        if (_cMoveBlock != null)
        {
            StopCoroutine(_cMoveBlock);
            _cMoveBlock = null;
        }
    }

    IEnumerator C_MoveBlock(eTileDirection moveDir, eBlockType blockType, BlockMoveCompleteCB completeCB, float moveTime, float delay)
    {
        if (_currentTile == null)
            yield break;

        _isMove = true;

        if (delay > 0)
        {
            WaitForSeconds wf = GameManager.Instance.GetWaitForSeconds(delay);
            yield return wf;
        }

        Vector3 targetPos = _currentTile.transform.position;
        Vector3 startPos = transform.position;

        float time = 0f;
        float t = 0f;
        while (true)
        {
            time += Time.deltaTime;
            t = time / moveTime;

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (time > moveTime)
                break;

            yield return null;
        }

        _isMove = false;
        completeCB?.Invoke(this, moveDir);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopPointerCheck();

        if (isMoveable == false)
            return;

        _cPointerCheck = StartCoroutine(C_PointerCheck());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopPointerCheck();
    }

    void StopPointerCheck()
    {
        if (_cPointerCheck != null)
        {
            StopCoroutine(_cPointerCheck);
            _cPointerCheck = null;
        }
    }

    IEnumerator C_PointerCheck()
    {
        while (true)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            Vector3 dir = worldPos - transform.position;
            float atan = Mathf.Atan2(dir.y, dir.x);
            float angle = atan * Mathf.Rad2Deg;

            eTileDirection eDirection = eTileDirection.None;
            if (angle >= 0)
                eDirection |= eTileDirection.Top;
            else
                eDirection |= eTileDirection.Bottom;

            if (Mathf.Abs(angle) <= 60)
                eDirection |= eTileDirection.Right;
            else if (Mathf.Abs(angle) >= 120f)
                eDirection |= eTileDirection.Left;

            if (dir.sqrMagnitude >= 0.1f)
            {
                SwitchBlock(eDirection);
                yield break;
            }

            yield return null;
        }
    }

    private void InitOutLine()
    {
        _outLineDic = new Dictionary<eTileDirection, GameObject>();

        GameObject outLine = Resources.Load<GameObject>("BlockOutLine");
        if (outLine != null)
        {
            GameObject instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = Vector3.zero;
            instObj.name = eTileDirection.Top.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.Top, instObj);

            instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = new Vector3(0, 0, 60f);
            instObj.name = eTileDirection.TopLeft.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.TopLeft, instObj);

            instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = new Vector3(0, 0, -60f);
            instObj.name = eTileDirection.TopRight.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.TopRight, instObj);

            instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = new Vector3(180, 0, 0);
            instObj.name = eTileDirection.Bottom.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.Bottom, instObj);

            instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = new Vector3(180, 0, 60f);
            instObj.name = eTileDirection.BottomLeft.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.BottomLeft, instObj);

            instObj = Instantiate(outLine, transform);
            instObj.transform.localPosition = Vector3.zero;
            instObj.transform.eulerAngles = new Vector3(180, 0, -60f);
            instObj.name = eTileDirection.BottomRight.ToString();
            instObj.SetActive(false);
            _outLineDic.Add(eTileDirection.BottomRight, instObj);
        }
    }

    public void SetBlockOutLineAll(bool isEnable)
    {
        foreach (GameObject outline in _outLineDic.Values)
            outline.SetActive(isEnable);
    }
    public void SetBlockOutLine(eTileDirection checkDirection, List<Block> blocks)
    {
        if (IsMatch(checkDirection) == true && blocks.Contains(this[checkDirection]) == true)
        {
            _outLineDic[checkDirection].SetActive(false);
        }
        if (IsMatch(checkDirection.GetReflect()) == true && blocks.Contains(this[checkDirection.GetReflect()]) == true)
        {
            _outLineDic[checkDirection.GetReflect()].SetActive(false);
        }
    }
}
