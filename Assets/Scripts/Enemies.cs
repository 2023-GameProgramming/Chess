using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInfo
{
    public GameObject obj;
    public Vector2Int coord;

    public MoveInfo()
    {
        this.obj = null;
        this.coord = Vector2Int.zero;
    }
}

public class Enemies : MonoBehaviour
{
    BoardObj tempObj;

    private void Start()
    {
        GameObject obj = new GameObject("temp");
        tempObj = obj.AddComponent<BoardObj>();
    }

    public void Decreaseturn()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childobj = transform.GetChild(i).gameObject;
            childobj.GetComponent<BoardObj>().DecreaseTurn();
        }
    }


    public GameObject GetObj(Vector2 crd)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childobj = transform.GetChild(i).gameObject;
            // 자식 오브젝트에 대한 작업 수행
            if (childobj.GetComponent<BoardObj>().Coord == crd)
            {
                return childobj;
            }
        }
        return null;
    }

    public List<GameObject> GetMover()
    {
        List<GameObject> moverlist = new List<GameObject>(); 
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childobj = transform.GetChild(i).gameObject;
            if (childobj.GetComponent<BoardObj>().turn == 0)
            {
                moverlist.Add(childobj);
            }
        }
        return moverlist;
    }
    public MoveInfo GetChecker()
    {
        MoveInfo info = new MoveInfo();
        List<GameObject> moverlist = GetMover();
        foreach (var obj in moverlist)
        {
            tempObj.Coord = GameManager.Instance.player.GetComponent<BoardObj>().Coord;
            tempObj.Type = obj.GetComponent<BoardObj>().Type;
            tempObj.sight = obj.GetComponent<BoardObj>().sight;
            List<GameObject> tiles = GameManager.Instance.board.FindMovableTiles(obj.GetComponent<BoardObj>());
            List<GameObject> tiles2 = GameManager.Instance.board.FindMovableTiles(tempObj);
            foreach (var t1 in tiles)
            {
                foreach (var t2 in tiles2)
                {
                    if(t1 == t2)
                    {
                        info.obj = obj;
                        info.coord =t1.GetComponent<Tile>().Coord;
                        return info;
                    }
                }
            }
        }
        return info;
    }
    public MoveInfo GetAttacker()
    {
        MoveInfo info = new MoveInfo();
        List<GameObject> moverlist = GetMover();
        foreach (var obj in moverlist)
        {
            List<GameObject> tiles = GameManager.Instance.board.FindMovableTiles(obj.GetComponent<BoardObj>());
            Vector2Int playerCrd = GameManager.Instance.player.GetComponent<BoardObj>().Coord;
            foreach (var v in tiles)
            {
                if (v.GetComponent<Tile>().Coord == playerCrd)
                {
                    info.obj = obj;
                    info.coord = playerCrd;
                    return info;
                }
            }
        }
        return info;
    }
}
