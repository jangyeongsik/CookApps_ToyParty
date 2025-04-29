using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public enum eTileDirection
{
    Top         = 0x000001,
    TopRight    = 0x000101,
    TopLeft     = 0x001001,
    Bottom      = 0x000010,
    BottomRight = 0x000110,
    BottomLeft  = 0x001010,
    End,

    Left        = 0x001000,
    Right       = 0x000100,
    None = 0x000000,
}

public enum eBlockType
{
    [Description("")] Empty,
    [Description("Blocks/blue_normal")] Blue,
    [Description("Blocks/green_normal")] Green,
    [Description("Blocks/orange_normal")] Orange,
    [Description("Blocks/purple_normal")] Purple,
    [Description("Blocks/red_normal")] Red,
    [Description("Blocks/yellow_normal")] Yellow,
    [Description("")] Count,
    [Description("Blocks/spinner")] Spinner = 9,
    [Description("Blocks/WoodTower")] Wood,
    [Description("Blocks/Jocker")] Jocker,
    [Description("")] TileType,
    [Description("Blocks/NormalTile")] Normal,
    [Description("Blocks/PaperTile")] Paper,
}

public class Tile : MonoBehaviour
{
    public bool isCreateTile;
    public Block blockTest;
    public eBlockType blockType;
    public eBlockType tileType = eBlockType.Normal;
    public SpriteRenderer tileSprite;
    public Tile TopTile;
    public Tile TopRightTile;
    public Tile TopLeftTile;
    public Tile BottomTile;
    public Tile BottomRightTile;
    public Tile BottomLeftTile;
    public Tile this[eTileDirection direction]
    {
        get
        {
            switch (direction)
            {
                case eTileDirection.Top: return TopTile;
                case eTileDirection.TopRight: return TopRightTile;
                case eTileDirection.TopLeft: return TopLeftTile;
                case eTileDirection.Bottom: return BottomTile;
                case eTileDirection.BottomRight: return BottomRightTile;
                case eTileDirection.BottomLeft: return BottomLeftTile;
                default: return null;
            }
        }
        set
        {
            switch (direction)
            {
                case eTileDirection.Top: TopTile = value; break;
                case eTileDirection.TopRight: TopRightTile = value; break;
                case eTileDirection.TopLeft: TopLeftTile = value; break;
                case eTileDirection.Bottom: BottomTile = value; break;
                case eTileDirection.BottomRight: BottomRightTile = value; break;
                case eTileDirection.BottomLeft: BottomLeftTile = value; break;
            }
        }
    }

    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    private int _tileHp = 0;

    public void Init()
    {
        if (Const.BlockTargetHP.ContainsKey(tileType) == true)
            _tileHp = Const.BlockTargetHP[tileType];
        else
            ResetTile();

        CreateBlock();
        SetTileIcon();
        blockTest.SetBlockType(blockType);
    }

    private void SetTileIcon()
    {
        if (tileSprite == null)
            return;

        string path = tileType.ToDescription();
        if (string.IsNullOrEmpty(path) == true)
        {
            tileSprite.enabled = false;
        }
        if (tileSprite != null)
        {
            tileSprite.enabled = true;
            tileSprite.sprite = Resources.Load<Sprite>(path);
        }
    }

    public bool CheckTile()
    {
        if (tileType == eBlockType.Normal)
            return false;

        if (--_tileHp <= 0)
        {
            return true;
        }
        return false;
    }

    public void ResetTile()
    {
        tileType = eBlockType.Normal;
        SetTileIcon();
    }

    private Tile GetTopTile()
    {
        if (isCreateTile == true)
            return null;

        Tile tile = null;
        if (CheckTopTile(eTileDirection.Top) == true)
            tile = this[eTileDirection.Top];
        else if (CheckTopTile(eTileDirection.TopLeft) == true)
            tile = this[eTileDirection.TopLeft];
        else if (CheckTopTile(eTileDirection.TopRight) == true)
            tile = this[eTileDirection.TopRight];

        return tile;
    }

    private bool CheckTopTile(eTileDirection dir)
    {
        Tile tile = this[dir];

        if (tile == null)
            return false;

        if (tile.isCreateTile == true)
            return true;

        if (tile != null && tile.blockTest != null && tile.blockTest.isMoveable == true)
            return true;

        return false;
    }

    public void MoveDown()
    {
        if (isCreateTile == true)
        {
            if (IsEmptyBlock(eTileDirection.Bottom) == true)
            {
                CreateBlock();
                blockTest.SetRanomType();
            }
            else if (IsEmptyBlock(eTileDirection.BottomLeft) == true)
            {
                CreateBlock();
                blockTest.SetRanomType();
            }
            else if (IsEmptyBlock(eTileDirection.BottomRight) == true)
            {
                CreateBlock();
                blockTest.SetRanomType();
            }
        }

        if (blockTest == null)
        {
            Tile tile = GetTopTile();
            if (tile != null)
                tile.MoveDown();

            return;
        }
        else
        {
            if (IsEmptyBlock(eTileDirection.Bottom) == true)
            {
                blockTest.MoveDown(eTileDirection.Bottom);
                Tile tile = GetTopTile();
                if (tile != null)
                    tile.MoveDown();
            }
            else if (IsEmptyBlock(eTileDirection.BottomLeft) == true)
            {
                blockTest.MoveDown(eTileDirection.BottomLeft);
                Tile tile = GetTopTile();
                if (tile != null)
                    tile.MoveDown();
            }
            else if (IsEmptyBlock(eTileDirection.BottomRight) == true)
            {
                blockTest.MoveDown(eTileDirection.BottomRight);
                Tile tile = GetTopTile();
                if (tile != null)
                    tile.MoveDown();
            }
        }
    }

    public bool IsEmptyBlock(eTileDirection direction)
    {
        return this[direction] != null && this[direction].isCreateTile == false && this[direction].blockTest == null;
    }

    public void CreateBlock()
    {
        blockTest = StageManager.Instance.CreateBlock(this);
    }
}
