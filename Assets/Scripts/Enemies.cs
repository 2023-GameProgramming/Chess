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
    Dictionary<Vector2Int, GameObject> objectDict;

    private void Start()
    {
        objectDict = new Dictionary<Vector2Int, GameObject>();
        GameObject obj = new GameObject("temp");
        tempObj = obj.AddComponent<BoardObj>();
        BoardObj[] boardObjs = GetComponentsInChildren<BoardObj>();
        foreach (BoardObj boardObj in boardObjs)
        {
            objectDict[boardObj.Coord] = boardObj.gameObject;
            boardObj.OnMoveCoord += UpdateDict;
        }
    }

    public void KillEnemy(GameObject obj)
    {
        objectDict[obj.GetComponent<BoardObj>().Coord] = null;
        obj.transform.SetParent(null);
        obj.name = null;
        obj.SetActive(false);
        GameObject.Destroy(obj);
    }


    void UpdateDict(GameObject obj, Vector2Int oldcrd , Vector2Int newCrd)
    {
        if(objectDict[oldcrd].Equals(obj))
        {
            objectDict[oldcrd] = null;
        }
        objectDict[newCrd] = obj;
    }

    public void Decreaseturn()
    {
        BoardObj[] boardObjs = GetComponentsInChildren<BoardObj>();
        foreach (BoardObj boardObj in boardObjs)
        {
            boardObj.DecreaseTurn();
        }
    }


    public GameObject GetObj(Vector2Int crd)
    {
        if (objectDict.TryGetValue(crd, out GameObject obj))
        {
            return obj;
        }
        else
        {
            return null;
        }
    }

    public List<GameObject> GetMover()
    {
        List<GameObject> moverlist = new List<GameObject>();
        BoardObj[] boardObjs = GetComponentsInChildren<BoardObj>();
        foreach (BoardObj boardObj in boardObjs)
        {
            if (boardObj.GetComponent<BoardObj>().turn == 0)
            {
                moverlist.Add(boardObj.gameObject);
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
