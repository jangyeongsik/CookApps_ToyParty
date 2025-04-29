using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public static class Extentions
{
    public static string ToDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    public static eTileDirection GetReflect(this eTileDirection value)
    {
        switch (value)
        {
            case eTileDirection.Top:
                return eTileDirection.Bottom;
            case eTileDirection.TopRight:
                return eTileDirection.BottomLeft;
            case eTileDirection.TopLeft:
                return eTileDirection.BottomRight;
            case eTileDirection.Bottom:
                return eTileDirection.Top;
            case eTileDirection.BottomRight:
                return eTileDirection.TopLeft;
            case eTileDirection.BottomLeft:
                return eTileDirection.TopRight;
        }
        return eTileDirection.None;
    }

    public static bool IsDirection(this eTileDirection value, eTileDirection direction)
    {
        return (value & direction) == direction;
    }

    public static bool IsMoveableBlock(this eBlockType value)
    {
        switch (value)
        {
            case eBlockType.Empty:
            case eBlockType.Blue:
            case eBlockType.Green:
            case eBlockType.Orange:
            case eBlockType.Purple:
            case eBlockType.Red:
            case eBlockType.Yellow:
            case eBlockType.Spinner:
                return true;
        }

        return false;
    }

    public static bool IsRemoveableBlock(this eBlockType value)
    {
        switch (value)
        {
            case eBlockType.Jocker:
                return false;
        }

        return true;
    }
}
