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
        // ������Ʈ�� �̵� ������ Ÿ���� �����մϴ�. Ÿ���� �Ӽ��� ��� �ؾ� �մϴ�.
        //�÷��̾ �ƴ� ���  GameManager.Instance.Enemies.IsAnyObj()�� Ÿ���� ���� �մϴ�.
        return null;
    }
}
