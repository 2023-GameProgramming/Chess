using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ������ �ִ� ���� Ÿ�԰� ���� ����
    Dictionary<string, int> hat;
    
    Vector2 FocusedTileCoord()
    {
        return new Vector2();
    }

    void ShowTile()
    {

    }
    
    void OnAttacked()
    {
        
    }

    ePiece SwitchHat(ePiece type)
    {
        
        return ePiece.king;
    }
    void AcquireHat(string hat)
    {

    }
}
