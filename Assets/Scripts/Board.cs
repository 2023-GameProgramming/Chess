using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [HideInInspector]
    public int row; // EditInit
    [HideInInspector]
    public int col; // EditInit
    [HideInInspector]
    public GameObject board; // EditInit

    List<Tile> PosibleToMove(BoardObj obj)
    {
        ePiece piece = obj.GetObjType();
        int delay = obj.GetDelay();

        return null;
    }

    bool IsAnyObj(Vector2 coord)
    {
        //해당 좌표 타일에 위치한 오브젝트가 있는가
        
        return false;
    }
}
