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
        // 1.���콺 �Է¿� ���� ���� ������ �ٲߴϴ�.

        // 2.shift�� ������ �̵� ���� Ÿ���� �����ݴϴ�.
        
        // 3.���� �´� Ÿ���� �׷��ݴϴ�.

        if(GameManager.Instance.PlayerTurn && !GetComponent<BoardObj>().IsMoving)
        {
            //���콺 Ŭ���� ��Ȳ�� �°� �̵�
            GetComponent<BoardObj>().turn -=1;
            //���� �̵��� Ÿ�Ͽ� ���� �־��ٸ� �ش� ���� �����ϰ� ���ڸ� ����.
        }
    }


    Vector2 FocusedTileCoord()
    {
        // �÷��̾ ������ ���� Ÿ���� ��ȯ�մϴ�.
        return new Vector2();
    }
    void ShowTileColor()
    {
        /*
         ��Ȳ�� ���� Ÿ�� ���� ��� �ݴϴ�.
        �̵� �Ұ� // red
        �̵� ���� // green �ε� �� �����ϰ�
        sight Ÿ�� // green
         */
    }
    
    public void OnAttacked(Vector2 dir)
    {
        /*
         �ǰݽ� ȣ��Ǵ� �ڵ��Դϴ�.
        ������ ������ ������ ���Դϴ�.
        �ǰݵ� �������� 2Tile �з����� ����ϴ�.
         */
    }

    ePiece SwitchHat(ePiece type)
    {
        // ���� ������ �ش� Ÿ���� ���ڷ� �ٲߴϴ�.    
        return ePiece.king;
    }
    void AcquireHat(string hat)
    {
        //���ڸ� ����ϴ�.
    }
}
