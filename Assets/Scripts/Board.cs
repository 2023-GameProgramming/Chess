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

    List<Tile> PosibleToMove(BoardObj obj)
    {
        ePiece piece = obj.Type;
        int sight = obj.sight;
        // 오브젝트가 이동 가능한 타일을 리턴합니다. 타일의 속성도 고려 해야 합니다.
        //플레이어가 아닐 경우  GameManager.Instance.Enemies.IsAnyObj()인 타일은 빼야 합니다.
        return null;
    }
}
