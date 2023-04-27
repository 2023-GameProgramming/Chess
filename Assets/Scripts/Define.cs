using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ePiece
{
    pawn,
    bishop,
    rook,
    knight,
    queen,
    king
}

public enum eTileAttr
{
    basic,
    water,
    wall,
    goal,
    Start,
}

public class ColTile
{
    public static Color basic = Color.clear;
    public static Color movable = Color.green;
    public static Color unmovable = Color.red;
    public static Color focus = Color.cyan;
}
