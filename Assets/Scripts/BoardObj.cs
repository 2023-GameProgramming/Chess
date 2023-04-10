using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObj :MonoBehaviour
{
    #region EditInit

    [HideInInspector]
    public Vector2 Coord; // EditInit
    [HideInInspector]
    public ePiece Type; // EditInit
    [HideInInspector]
    public int sight; // EditInit
    [HideInInspector]
    public int delay;  // EditInit
    [HideInInspector]
    public float movetime; // EditInit
    #endregion EditInit

    public int turn;
    public bool IsMoving;

    #region MonoBehavior
    private void Start()
    {
        turn = delay;
        IsMoving = false;
        GetComponent<SpriteRenderer>().sprite = Resource.Instance.GetPieceSprite(Type);
    }
    #endregion MonoBehavior

    #region IBoardObj


    public Vector2 SetCoord(Vector2 crd)
    {
        Coord = crd;
        // 해당 타일 위치로 이동시킨다.
        return crd;
    }

    public Vector2 MoveCoord(Vector2 crd) 
    {
        Coord = crd;
        IsMoving = true;
        StartCoroutine(ChangeTransfrom());
        return crd;
    }
    #endregion


    IEnumerator ChangeTransfrom()
    {
        GameManager.Instance.MovingObjNum += 1;
        // Movetime동안 Transform 업데이트
        float elapsedTime = 0f; 
        while (elapsedTime < movetime) 
        {
            elapsedTime += Time.deltaTime;
             yield return null;
        }
        IsMoving = false;
        GameManager.Instance.MovingObjNum -= 1;
    }
}
