using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    #region SingleTon
    [HideInInspector]
    public static GameManager Instance = null;
    #endregion

    public int MaxStageNum = 1;
    private int CurrentStageIndex;

    [HideInInspector]
    public Enemies enemies;
    [HideInInspector]
    public Board board;
    [HideInInspector]
    public Player player;

    [HideInInspector]
    public bool PlayerTurn;
    [HideInInspector]
    public int MovingObjNum;


    bool ProgressTurn;
    bool PlayerAttacked;

    MoveInfo Attacker;

    bool EnemyAttacked;
    #region MonoBehavior
    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            Resource.Instance.Initialize();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Attacker = new MoveInfo();
        CurrentStageIndex = -1;
        PlayerTurn = true;
        ProgressTurn = false;
        GameObject playerPrefab = Resources.Load<GameObject>("Basic/Player");
        player = GameObject.Instantiate(playerPrefab).GetComponent<Player>();
        player.transform.position = Vector3.zero;
        UpdateStage();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Cursor.SetCursor(null, screenCenter, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (player.GetComponent<BoardObj>().turn == 0)
        {
            PlayerTurn = false;
            if (!player.GetComponent<BoardObj>().IsMoving)
            {
                HandleAttack();
            }
            if (player.GetComponent<BoardObj>().IsMoving)
            {
                GameObject Caturedenemy = enemies.GetObj(player.GetComponent<BoardObj>().Coord);
                if (Caturedenemy != null && !PlayerAttacked)
                {
                    Caturedenemy.GetComponent<BoardObj>().ResetTurn();
                    EnemyAttacked = true;
                }

            }
            HandleProgress();
        }
        if (!player.Isalive)
        {
            Debug.Log("쥬금");
        }

        if (board.GetTile(player.GetComponent<BoardObj>().Coord).GetComponent<Tile>().Type == eTileAttr.goal)
        {
            Clear();
        }
    }


    void HandleEnemyBehave()
    {
        MoveInfo info = enemies.GetAttacker();
        if (info.obj != null)
        {
            Attacker = new MoveInfo();
            Attacker.obj = info.obj;
            Attacker.coord = info.obj.GetComponent<BoardObj>().Coord;
            info.obj.GetComponent<BoardObj>().MoveCoord(info.coord);
            info.obj.GetComponent<BoardObj>().ResetTurn();

            PlayerAttacked = true;
        }
        else if (PlayerAttacked == false && EnemyAttacked == false)
        {
            info = enemies.GetChecker();
            while (info.obj != null)
            {
                info.obj.GetComponent<BoardObj>().MoveCoord(info.coord);
                info.obj.GetComponent<BoardObj>().ResetTurn();
                info = enemies.GetChecker();
            }
            foreach (var v in enemies.GetMover())
            {
                List<GameObject> movabletile = board.FindMovableTiles(v.GetComponent<BoardObj>());
                if (movabletile.Count != 0)
                {
                    // 좀 더 인공지능을 추가할 수 있겠지만 랜덤하게 설정.
                    v.GetComponent<BoardObj>().MoveCoord(movabletile[Random.Range(0, movabletile.Count)].GetComponent<Tile>().Coord);
                }
                v.GetComponent<BoardObj>().ResetTurn();
            }
        }
    }

    void HandleProgress()
    {
        if (ProgressTurn == false)
        {
            ProgressTurn = true;
            enemies.Decreaseturn();
        }
        else if (enemies.GetMover().Count == 0 && MovingObjNum == 0)
        {
            PlayerTurn = true;
            ProgressTurn = false;
            player.GetComponent<BoardObj>().ResetTurn();
        }
        else
        {
            HandleEnemyBehave();
        }
    }
    void HandleAttack()
    {
        if (EnemyAttacked)
        {
            GameObject Caturedenemy = enemies.GetObj(player.GetComponent<BoardObj>().Coord);
            player.CaptureEnemy(Caturedenemy);
            EnemyAttacked = false;
        }
        if (PlayerAttacked && !Attacker.obj.GetComponent<BoardObj>().IsMoving)
        {
            Vector2Int dir = board.GetCrdDir(Attacker.coord, player.GetComponent<BoardObj>().Coord);
            Vector2Int nextCrd = Vector2Int.zero;
            int i = 2;
            for (; i >= 0; i--)
            {
                nextCrd = i * dir + player.GetComponent<BoardObj>().Coord;
                if (GameManager.Instance.board.IsPossible(nextCrd))
                {
                    break;
                }
            }
            player.OnAttacked(nextCrd);
            if (i != 2)
            {
                // 플레이어가 두칸 밀려나지 않으면 attacker는 제자리로.
                Attacker.obj.GetComponent<BoardObj>().MoveCoord(Attacker.coord);
            }
            PlayerAttacked = false;
        }
    }

    
    void Clear()
    {
        if (MaxStageNum > CurrentStageIndex + 1)
        {
            UpdateStage();
        }
        else
        {
            //엔딩 보여줌
        }
    }

    void UpdateStage()
    {
        CurrentStageIndex++;
        GameObject CurrentStage = GameObject.Find("Stage" + CurrentStageIndex.ToString());
        enemies = CurrentStage.transform.Find("Enemies").GetComponent<Enemies>();
        board = CurrentStage.transform.Find("Tiles").GetComponent<Board>();
        player.GetComponent<BoardObj>().turn = player.GetComponent<BoardObj>().delay;
        player.GetComponent<BoardObj>().SetCoord(board.GetStartCoord());
        CameraController cameracontroller = Camera.main.GetComponent<CameraController>();
        cameracontroller.target = player.transform;
    }
    #endregion
}
