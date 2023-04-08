using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 가지고 있는 모자 타입과 개수 저장
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
