using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObj :MonoBehaviour
{
    #region EditInit
    [HideInInspector]
    public Vector2Int Coord; // EditInit
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
    [HideInInspector]
    public bool IsMoving;

    Vector3 startPoint;
    Vector3 endPoint;


    #region MonoBehavior

    private void Start()
    {
        turn = delay;
        IsMoving = false;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer !=null)
        {
            spriteRenderer.sprite = Resource.Instance.GetPieceSprite(Type);
        }
    }
    #endregion MonoBehavior

    #region IBoardObj
    public void ResetTurn()
    {
        turn = delay;
    }
    public void DecreaseTurn()
    {
        turn -= 1;
        if(turn <0)
        {
            turn = 0;
        }
    }
    public Vector2 SetCoord(Vector2Int crd)
    {
        Coord = crd;
        Vector3 tilepos = GameManager.Instance.board.GetTile(crd).transform.position;
        // Mesh의 bounds를 이용하여 quad 오브젝트의 길이를 구한다.
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // 타일 오브젝트의 위치 설정
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        transform.position = tilepos +  new Vector3(0,height / 2,0);
        return crd;
    }

    public Vector2 MoveCoord(Vector2Int crd) 
    {
        Coord = crd;
        IsMoving = true;
        startPoint = transform.position;
        float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        endPoint = GameManager.Instance.board.GetTile(crd).transform.position + new Vector3(0, height / 2, 0);
        StartCoroutine(ChangeTransfrom());
        return crd;
    }
    #endregion


    IEnumerator ChangeTransfrom()
    {
        GameManager.Instance.MovingObjNum += 1;
        // 이동이 linear해도 좋고, https://easings.net 를 참고해서 역동적으로 움직여도 좋습니다
        float elapsedTime = 0f; 
        while (elapsedTime < movetime) 
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPos = startPoint + (endPoint - startPoint) / movetime * elapsedTime;
            transform.position = newPos;
            yield return null;
        }
        transform.position = endPoint;
        IsMoving = false;
        GameManager.Instance.MovingObjNum -= 1;
    }
}
