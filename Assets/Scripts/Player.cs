using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{


    public GameObject HatUI;

    public bool Isalive;
    Dictionary<ePiece, int> hat;
    int hatIndex;
    public List<GameObject> MovableTile;
    GameObject focusTile;
    bool dirMove;





    private void Start()
    {
        HatUI = GameObject.FindGameObjectWithTag("HatUI");
        dirMove = false;
        Isalive = true;
        MovableTile = new List<GameObject>();
        hatIndex = ((int)ePiece.king);
        GetComponent<BoardObj>().Type = ePiece.king;
        hat = new Dictionary<ePiece, int>();
        hat.Add(ePiece.pawn, 0);
        hat.Add(ePiece.rook, 0);
        hat.Add(ePiece.bishop, 0);
        hat.Add(ePiece.knight, 0);
        hat.Add(ePiece.queen, 0);
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
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetTilesColor(MovableTile, ColTile.movable);
        }
        SetFocusTileColor();
        if (focusTile != null && Input.GetMouseButton(0)&& Isalive && MovableTile.IndexOf(focusTile)>=0)
        {
            if (GetComponent<BoardObj>().Type == ePiece.pawn || GetComponent<BoardObj>().Type == ePiece.king|| GetComponent<BoardObj>().Type == ePiece.knight ||
                !Input.GetKey(KeyCode.LeftShift))
            {
                GetComponent<BoardObj>().DecreaseTurn();
                GetComponent<BoardObj>().MoveCoord(focusTile.GetComponent<Tile>().Coord);

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


                if (MovableTile.Count ==0)
                {
                    GetComponent<BoardObj>().DecreaseTurn();
                    dirMove = false;
                }
                else
                {
                    dirMove = true;
                }
                GetComponent<BoardObj>().MoveCoord(focusTile.GetComponent<Tile>().Coord);
                if(GameManager.Instance.enemies.GetObj(focusTile.GetComponent<Tile>().Coord))
                {
                    GetComponent<BoardObj>().DecreaseTurn();
                }
            }
        }
    }


    public void CaptureEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            AcquireHat(enemy.GetComponent<BoardObj>().Type);
            GameManager.Instance.enemies.KillEnemy(enemy);
        }
    }
    public void OnAttacked(Vector2Int destCrd)
    {
        LooseRandomHat();
        Isalive = SwitchHat();
        GetComponent<BoardObj>().MoveCoord(destCrd);
    }

    void SetFocusTileColor()
    {
        if (focusTile != null)
        {
            focusTile.GetComponent<Tile>().ChangeColor(ColTile.basic);
            focusTile = null;
        }
        Vector3 mousePosition = Input.mousePosition;
        mousePosition += new Vector3(0, mousePosition.y/3, 0);
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
                    if (neartile == null && GameManager.Instance.board.FindOneMovableTiles(this.GetComponent<BoardObj>()).Contains(v) ||
                        (neartile != null&&
                        (CalTileDistance(neartile)  >= CalTileDistance(v))))
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
    public void ResetTileColor()
    {
        foreach (var v in MovableTile)
        {
            v.GetComponent<Tile>().ChangeColor(ColTile.basic);
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
            if(hat[(ePiece)i]==1 && (ePiece)i!= ePiece.king)
            {
                myhatList.Add((ePiece)i);
            }
        }
        ePiece Value;
        if (myhatList.Count !=0)
        {
            int random = Random.Range(0, myhatList.Count);
            hat[myhatList[random]] = 0;
            Value = (ePiece)myhatList[random];
        }
        else
        {
            hat[ePiece.king] = 0;
            Value = ePiece.king;
        }
        string name = ePiece.GetName(typeof(ePiece), Value);
        HatUI.transform.Find(name + "Slot").GetComponent<Image>().color = Color.black;
    }
    bool SwitchHat()
    {
        ResetTileColor();
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

        if(hat[GetComponent<BoardObj>().Type] == 1)
        {
            HatUI.transform.Find(ePiece.GetName(typeof(ePiece), GetComponent<BoardObj>().Type) + "Slot").GetComponent<Image>().color = Color.white;
        }
        ePiece Value = (ePiece)finedType;
        string name = ePiece.GetName(typeof(ePiece), Value);
        HatUI.transform.Find("CurPieceSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList[name+".PNG"];
        HatUI.transform.Find(name + "Slot").GetComponent<Image>().color = Color.magenta;
        GetComponent<BoardObj>().Type = (ePiece)finedType;
        FindMovableTile();
        return true;
    }

    public void FindMovableTile()
    {
        MovableTile = GameManager.Instance.board.FindMovableTiles(GetComponent<BoardObj>());
    }

    int CalTileDistance(GameObject obj)
    {
        Vector2Int compare = obj.GetComponent<Tile>().Coord;
        Vector2Int origin = GetComponent<BoardObj>().Coord;
        int x = Mathf.Abs(compare.x - origin.x);
        int y = Mathf.Abs(compare.y - origin.y);
        return (x > y ? x:y);       
        
    }
    void AcquireHat(ePiece type)
    {
        int num = hat[type];
        if (num == 0)
        {
            hat[type] = 1;
            string name = ePiece.GetName(typeof(ePiece), type);
            HatUI.transform.Find(name + "Slot").GetComponent<Image>().color = Color.white;
        }
    }
}
