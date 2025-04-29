using UnityEngine;
using System.Collections.Generic;

public class Const
{
    public const int MinMatchCount = 3;
    public const float MoveBlockTime = 0.2f;
    public const float MoveBlockUndoTime = 0.1f;
    public const float MoveBlockUndoDelayTime = 0.3f;

    public static Dictionary<eBlockType, int> BlockTargetHP = new Dictionary<eBlockType, int>()
    {
        { eBlockType.Spinner, 2},
        { eBlockType.Wood, 2},
        { eBlockType.Jocker, 1},
        { eBlockType.Paper, 1}
    };

    public static Dictionary<eBlockType, string> BlockTargetEffect = new Dictionary<eBlockType, string>()
    {
        { eBlockType.Spinner, "Particle/UIParticle_Spinner"},
        { eBlockType.Wood, "Particle/UIParticle_WoodTower"},
        { eBlockType.Jocker, "Particle/UIParticle_Jocker"},
        { eBlockType.Paper, "Particle/UIParticle_Paper"}
    };

}
