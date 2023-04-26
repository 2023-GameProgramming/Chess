using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool Isalive;

    Dictionary<ePiece, int> hat;
    int hatIndex;
    List<GameObject> MovableTile;
    List<GameObject> Temp2MovableTile;
    List<GameObject> CurrentDrawTile;

    GameObject focusTile;

    bool dirMove;

    private void Start()
    {
        dirMove = false;
        Isalive = true;
        MovableTile = new List<GameObject>();
        hatIndex = ((int)ePiece.king);
        GetComponent<BoardObj>().Type = ePiece.king;
        hat = new Dictionary<ePiece, int>();
        hat.Add(ePiece.pawn, 1);
        hat.Add(ePiece.rook, 1);
        hat.Add(ePiece.bishop, 1);
        hat.Add(ePiece.knight, 1);
        hat.Add(ePiece.queen, 1);
        hat.Add(ePiece.king, 1);
    }

    private void Update()
    {
        SetTilesColor(MovableTile, ColTile.basic);
        if (MovableTile.Count == 0) { MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());}

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (dirMove)
            {
                GetComponent<BoardObj>().DecreaseTurn();
                SetTilesColor(MovableTile, ColTile.basic);
                MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
                dirMove = false;
            }
        }
        if ( !GameManager.Instance.PlayerTurn || GetComponent<BoardObj>().IsMoving)
        {
            focusTile?.GetComponent<Tile>().ChangeColor(ColTile.basic);
            focusTile = null;
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
        {
            SwitchHat();
            SetTilesColor(MovableTile, ColTile.basic);
            MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetTilesColor(MovableTile, ColTile.movable);
        }
        SetFocusTileColor();
        if (focusTile != null && Input.GetMouseButtonDown(0) && MovableTile.IndexOf(focusTile)>=0)
        {
            if(GetComponent<BoardObj>().Type == ePiece.pawn || GetComponent<BoardObj>().Type == ePiece.king|| GetComponent<BoardObj>().Type == ePiece.knight ||
                !Input.GetKey(KeyCode.LeftShift))
            {
                GetComponent<BoardObj>().DecreaseTurn();
                SetTilesColor(MovableTile, ColTile.basic);
                GetComponent<BoardObj>().MoveCoord(focusTile.GetComponent<Tile>().Coord);
                MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
            }
            else
            {
                Vector2Int dir = GameManager.Instance.board.GetCrdDir(GetComponent<BoardObj>().Coord, focusTile.GetComponent<Tile>().Coord);
                List<GameObject> newMovableTile = new List<GameObject>();
                foreach(var v in MovableTile)
                {
                    if(GameManager.Instance.board.GetCrdDir(focusTile.GetComponent<Tile>().Coord, v.GetComponent<Tile>().Coord) == dir)
                    {
                        newMovableTile.Add(v);
                    }
                }
                SetTilesColor(MovableTile, ColTile.basic);
                MovableTile = newMovableTile;
                if(MovableTile.Count ==0)
                {
                    GetComponent<BoardObj>().DecreaseTurn();
                    dirMove = false;
                }
                dirMove = true;
                GetComponent<BoardObj>().MoveCoord(focusTile.GetComponent<Tile>().Coord);
                if(GameManager.Instance.enemies.GetObj(focusTile.GetComponent<Tile>().Coord))
                {
                    SetTilesColor(MovableTile, ColTile.basic);
                    GetComponent<BoardObj>().DecreaseTurn();
                    MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
                }
            }
        }
    }


    public void CaptureEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            AcquireHat(enemy.GetComponent<BoardObj>().Type);
            GlobalFuction.SafeDestroy(enemy);
        }
    }
    public void OnAttacked(Vector2Int destCrd)
    {
        LooseRandomHat();
        Isalive = SwitchHat();
        SetTilesColor(MovableTile, ColTile.basic);
        GetComponent<BoardObj>().MoveCoord(destCrd);
        MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
    }

    void SetFocusTileColor()
    {
        if (focusTile != null)
        {
            focusTile.GetComponent<Tile>().ChangeColor(ColTile.basic);
            focusTile = null;
        }
        Vector3 mousePosition = Input.mousePosition;
       Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        int layerMask = LayerMask.GetMask("Tile");
        if (Input.GetKey(KeyCode.LeftShift) && Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
        {
            focusTile = hitInfo.collider.gameObject;
            if (MovableTile.IndexOf(focusTile) < 0)
            {
                focusTile.GetComponent<Tile>().ChangeColor(ColTile.unmovable);
            }
            else
            {
                focusTile.GetComponent<Tile>().ChangeColor(ColTile.focus);
            }
        }
        else
        {

            GameObject neartile = null;
            float nearangle = 360.0f;
            foreach (var v in MovableTile)
            {
                Vector2 tiledir = Vector2.zero;
                tiledir.x = (v.transform.position.x - transform.position.x);
                tiledir.y = (v.transform.position.z - transform.position.z);
                Vector2 raydir = Vector2.zero;
                raydir.x = ray.direction.x;
                raydir.y = ray.direction.z;

                float dot = Vector3.Dot(tiledir.normalized, raydir.normalized);
                float rad = Mathf.Acos(dot);
                float angle = rad * Mathf.Rad2Deg;
                if (angle < 45 && nearangle > angle )
                {
                    if(neartile == null||
                        (neartile != null && 
                        (neartile.transform.position - transform.position).sqrMagnitude > (v.transform.position - transform.position).sqrMagnitude)) 
                    {
                        nearangle = angle;
                        neartile = v;
                    }
                }
            }
            focusTile = neartile;
            focusTile?.GetComponent<Tile>().ChangeColor(ColTile.focus);
        }
    }

    void SetTilesColor(List<GameObject> list,Color col)
    {
        foreach (var v in list)
        {
            v.GetComponent<Tile>().ChangeColor(col);
        }
    }

    void LooseRandomHat()
    {
        List<ePiece> myhatList = new List<ePiece>();
        for (int i = 0; i < System.Enum.GetNames(typeof(ePiece)).Length; i++)
        {
            if(hat[(ePiece)i]==1)
            {
                myhatList.Add((ePiece)i);
            }
        }
        
        if(myhatList.Count !=0)
        {
            int random = Random.Range(0, myhatList.Count);
            hat[myhatList[random]] = 0;
        }
        else
        {
            hat[ePiece.king] = 0;
        }
    }
    bool SwitchHat()
    {
        int finedType = -1;
        for (int i = 0; i <= System.Enum.GetNames(typeof(ePiece)).Length; i++)
        {
            hatIndex += 1;
            if (hatIndex > ((int)ePiece.king))
            {
                hatIndex = ((int)ePiece.pawn);
            }
            if (hat[(ePiece)hatIndex] == 1)
            {
                finedType = hatIndex;
                break;
            }
        }
        if (finedType == -1)
        {
            return false;
        }
        GetComponent<BoardObj>().Type = (ePiece)finedType;
        Debug.Log($" changed :  { (ePiece)finedType }");
        return true;
    }
    void AcquireHat(ePiece type)
    {
        int num = hat[type];
        if (num == 0)
        {
            hat[type] = 1;
        }
    }
}
