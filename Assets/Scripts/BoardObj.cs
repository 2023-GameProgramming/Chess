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

    #region MonoBehavior

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Resource.Instance.GetPieceSprite(Type);
    }

    private void Update()
    {
        
    }
    #endregion MonoBehavior






    #region IBoardObj
    public Vector2 SetCoord(Vector2 crd) 
    {
        Coord = crd;
        StartCoroutine(Move());
        //GameManager.MovingObj += 1;
        return crd;
    }
    public int GetSight()
    {
        return 0;
    }
    public Vector2 GetCoord() 
    {
        return new Vector2(); 
    }
    public ePiece GetObjType() 
    {

        return ePiece.king;    
    }
    public int GetDelay() 
    {
        return 0;
    }
    #endregion

    IEnumerator Move()
    {
        float elapsedTime = 0f; // ��� �ð�
        while (elapsedTime < movetime) 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / movetime; 
            // Lerp �Լ��� ����Ͽ� ���� ��ġ ���
            //Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            // ��ġ ����
            //target.position = currentPos;

            // ���� �����ӱ��� ���
            yield return null;
        }
        //GameManager.MovingObj -= 1;
    }

}
