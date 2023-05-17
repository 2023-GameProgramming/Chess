using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    #region SingleTon
    [HideInInspector]
    public static GameManager Instance = null;
    #endregion

    public int MaxStageNum = 5;
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


    bool CheckCapturedEnemy;
    GameObject Caturedenemy;
    GameObject CurrentStage;
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
        CurrentStageIndex = -1;
        player = GameObject.Instantiate(Resources.Load<GameObject>("Basic/Player")).GetComponent<Player>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Cursor.SetCursor(null, screenCenter, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        UpdateStage();
    }

    void ResetCondition()
    {
        Attacker = new MoveInfo();
        PlayerTurn = true;
        ProgressTurn = false;
        CheckCapturedEnemy = false;
        PlayerAttacked = false;
        EnemyAttacked = false;
        MovingObjNum = 0;
        Caturedenemy = null;
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
            if (CheckCapturedEnemy)
            {
                Caturedenemy = enemies.GetObj(player.GetComponent<BoardObj>().Coord);
                if (Caturedenemy != null && !PlayerAttacked)
                {
                    Caturedenemy.GetComponent<BoardObj>().ResetTurn();
                    EnemyAttacked = true;
                }
                CheckCapturedEnemy = false;
            }
            HandleProgress();
        }
        if (!player.Isalive)
        {
            Debug.Log("���");
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
                    // �� �� �ΰ������� �߰��� �� �ְ����� �����ϰ� ����.
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
            CheckCapturedEnemy = true;
        }
        else if (enemies.GetMover().Count == 0 && MovingObjNum == 0)
        {
            PlayerTurn = true;
            ProgressTurn = false;
            player.GetComponent<BoardObj>().ResetTurn();
            player.ResetTileColor();
            player.FindMovableTile();

        }
        else if(!PlayerAttacked)
        {
            HandleEnemyBehave();
        }
    }
    void HandleAttack()
    {
        if (EnemyAttacked)
        {
            player.CaptureEnemy(Caturedenemy);
            EnemyAttacked = false;
        }
        if (PlayerAttacked && !Attacker.obj.GetComponent<BoardObj>().IsMoving)
        {
            Vector2Int dir = board.GetCrdDir(Attacker.coord, player.GetComponent<BoardObj>().Coord);
            Vector2Int nextCrd = Vector2Int.zero;
            int i = 0;
            for (; i <=2; i++)
            {
                nextCrd = (i) * dir + player.GetComponent<BoardObj>().Coord;
                if (!GameManager.Instance.board.IsPossible( dir + nextCrd))
                {
                    break;
                }
            }
            player.OnAttacked(nextCrd);
            if (i != 1)
            {
                Attacker.obj.GetComponent<BoardObj>().MoveCoord(Attacker.coord);
            }
            PlayerAttacked = false;
            CheckCapturedEnemy = true;
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
            //���� ������
        }
    }

    void UpdateStage()
    {
        ResetCondition();
        CurrentStageIndex++;
        GameObject.Destroy(CurrentStage);
        CurrentStage = GameObject.Instantiate(Resources.Load<GameObject>("Stage/Stage"+ CurrentStageIndex.ToString()));
        enemies = CurrentStage.transform.Find("Enemies").GetComponent<Enemies>();
        enemies.Initialize();
        board = CurrentStage.transform.Find("Tiles").GetComponent<Board>();
        board.Initialize();
        player.GetComponent<BoardObj>().turn = player.GetComponent<BoardObj>().delay;
        player.GetComponent<BoardObj>().SetCoord(board.GetStartCoord());
        player.MovableTile.Clear();
        CameraController cameracontroller = Camera.main.GetComponent<CameraController>();
        cameracontroller.target = player.transform;

    }
    #endregion
}
