using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ePiece;
using static eTileAttr;
public class Board : MonoBehaviour
{
    //[HideInInspector]
    public int row; // EditInit
    //[HideInInspector]
    public int col; // EditInit
    [HideInInspector]
    GameObject[,] TileList;


    private void Awake()
    {
        TileList = new GameObject[row, col];
        Tile[] tileObjs = GetComponentsInChildren<Tile>();
        foreach (Tile tileObj in tileObjs)
        {
            Vector2Int crd = tileObj.Coord;
            TileList[crd.y, crd.x] = tileObj.gameObject;
        }
    }
    public Vector2Int GetStartCoord()
    {
        Tile[] tileObjs = GetComponentsInChildren<Tile>();
        foreach (Tile tileObj in tileObjs)
        {
            if (tileObj.Type == eTileAttr.Start)
            {
                return tileObj.Coord;
            }
        }
        return Vector2Int.zero;
    }

    public List<GameObject> FindMovableTiles(BoardObj obj)
    {
        List<GameObject> possibleTiles = new List<GameObject>();
        switch (obj.Type)
        {
            case pawn:
                CheckPawn(obj, possibleTiles);
                break;
            case bishop:
                CheckBishop(obj, possibleTiles);
                break;
            case rook:
                CheckRook(obj, possibleTiles);
                break;
            case knight:
                CheckKnight(obj, possibleTiles);
                break;
            case queen:
                CheckQueen(obj, possibleTiles);
                break;
            case king:
                CheckKing(obj, possibleTiles);
                break;
        }
        return possibleTiles;
    }


    public GameObject GetTile(Vector2Int crd)
    {
        if (crd.x < 0 || crd.x >= col ||
            crd.y < 0 || crd.y >= row)
        {
            return null;
        }
        return TileList[crd.y, crd.x];
    }

    #region private Fuction

    bool IsPlayer(GameObject obj)
    {
        Player player = null;
        obj.TryGetComponent<Player>(out player);
        return (player != null);
    }

    bool IsTileMovable(Vector2Int crd)
    {
        GameObject tile = GetTile(crd);
        eTileAttr type = tile.GetComponent<Tile>().Type;
        return (type == basic || type == goal || type == eTileAttr.Start);
    }

    public Vector2Int GetCrdDir(Vector2Int origin, Vector2Int compare)
    {
        Vector2Int dir = Vector2Int.zero;
        dir.x = (origin.x < compare.x) ? 1 :
            (origin.x > compare.x) ? -1 : 0;
        dir.y = origin.y < compare.y ? 1 :
            (origin.y > compare.y) ? -1 : 0;
        return dir;
    }

    public bool IsPossible(Vector2Int crd)
    {
        GameObject tile = GetTile(crd);
        if(tile != null)
        {
            return tile.GetComponent<Tile>().IsPossibleToMove();
        }
        return false;
    }

    bool CheckMovableTile(BoardObj obj, Vector2Int nextcrd, List<GameObject> possibleTiles)
    {
        GameObject tile = GetTile(nextcrd);
        if (tile != null)
        {

            if(IsTileMovable(nextcrd))
            {
               GameObject nextCrdObj = GameManager.Instance.enemies.GetObj(nextcrd);
                if(nextCrdObj == null || (IsPlayer(obj.gameObject)))
                {
                    possibleTiles.Add(tile);
                    return true;
                }
            }
            else if (tile.GetComponent<Tile>().Type == eTileAttr.water && obj.GetComponent<BoardObj>().Type == ePiece.bishop)
            {
                return true;
            }
        }
        return false;
    }

    void CheckKing(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        if (sight == 0) { return; }
        Vector2Int coord = obj.Coord;
        Vector2Int nextCrd = new Vector2Int(coord.x - 1, coord.y);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 1, coord.y - 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 1, coord.y + 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);

        nextCrd = new Vector2Int(coord.x, coord.y + 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x, coord.y - 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);

        nextCrd = new Vector2Int(coord.x + 1, coord.y + 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x + 1, coord.y);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x + 1, coord.y - 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
    }
    void CheckQueen(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        Vector2Int coord = obj.Coord;

        bool proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);

        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);

        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);

        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
    }
    void CheckKnight(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        if (sight == 0) { return; }
        Vector2Int coord = obj.Coord;
        Vector2Int nextCrd = new Vector2Int(coord.x + 1, coord.y + 2);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x + 1, coord.y - 2);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 1, coord.y + 2);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 1, coord.y - 2);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x + 2, coord.y - 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x + 2, coord.y + 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 2, coord.y + 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
        nextCrd = new Vector2Int(coord.x - 2, coord.y - 1);
        CheckMovableTile(obj, nextCrd, possibleTiles);
    }
    void CheckRook(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        Vector2Int coord = obj.Coord;
        bool proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y);
            proceed = CheckMovableTile(obj, nextCrd,possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);

        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);

        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
    }
    void CheckBishop(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        Vector2Int coord = obj.Coord;

        bool proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x + i, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y - i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        proceed = true;
        for (int i = 1; i <= sight && proceed; i++)
        {
            Vector2Int nextCrd = new Vector2Int(coord.x - i, coord.y + i);
            proceed = CheckMovableTile(obj, nextCrd, possibleTiles);
        }


    }
    void CheckPawn(BoardObj obj, List<GameObject> possibleTiles)
    {
        int sight = obj.sight;
        if (sight == -1) { sight = (row > col ? row : col); }
        if (sight == 0) { return; }
        Vector2Int coord = obj.Coord;
        // 적이 있을 때만 대각선 이동 가능
        Vector2Int nextCrd = new Vector2Int(coord.x + 1, coord.y + 1);
        GameObject nextCrdObj = GameManager.Instance.enemies.GetObj(nextCrd);
        if (nextCrdObj != null)
        {
            CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        nextCrd = new Vector2Int(coord.x - 1, coord.y + 1);
        nextCrdObj = GameManager.Instance.enemies.GetObj(nextCrd);
        if (nextCrdObj != null)
        {
            CheckMovableTile(obj, nextCrd, possibleTiles);
        }
        //적이 없을 때만 직진 가능
        nextCrd = new Vector2Int(coord.x, coord.y + 1);
        nextCrdObj = GameManager.Instance.enemies.GetObj(nextCrd);
        if (nextCrdObj == null)
        {
            CheckMovableTile(obj, nextCrd, possibleTiles);
        }
    }


    #endregion
}