using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Dictionary<string, int> hat;

    private void Start()
    {
        
    }

    private void Update()
    {
        // 1.마우스 입력에 따라 보는 방향을 바꿉니다.

        // 2.shift를 누르면 이동 가능 타일을 보여줍니다.
        
        // 3.초점 맞는 타일을 그려줍니다.

        if(GameManager.Instance.PlayerTurn && !GetComponent<BoardObj>().IsMoving)
        {
            //마우스 클릭시 상황에 맞게 이동
            GetComponent<BoardObj>().turn -=1;
            //만약 이동한 타일에 적이 있었다면 해당 적을 삭제하고 모자를 얻음.
        }
    }


    Vector2 FocusedTileCoord()
    {
        // 플레이어가 초점을 맞춘 타일을 반환합니다.
        return new Vector2();
    }
    void ShowTileColor()
    {
        /*
         상황에 따른 타일 색을 띄워 줍니다.
        이동 불가 // red
        이동 가능 // green 인데 좀 투명하게
        sight 타일 // green
         */
    }
    
    public void OnAttacked(Vector2 dir)
    {
        /*
         피격시 호출되는 코드입니다.
        랜덤한 모자의 개수를 줄입니다.
        피격돈 방향으로 2Tile 밀려나게 만듭니다.
         */
    }

    ePiece SwitchHat(ePiece type)
    {
        // 현재 장착한 해당 타입의 모자로 바꿉니다.    
        return ePiece.king;
    }
    void AcquireHat(string hat)
    {
        //모자를 얻습니다.
    }
}
