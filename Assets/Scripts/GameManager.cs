using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject StroyObj;
    [Range(0.001f, 1.0f)]
    public float BgmLoud;
    [Range(0.001f, 1.0f)]
    public float SELoud;
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

    [HideInInspector]
    public AudioMixer Mixer;
    float progressbar;
    AudioSource bgmAudio;
    int SceneIndex;

    #region MonoBehavior
    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
            BgmLoud = 0.5f;
            SELoud = 1.0f;
            Mixer = Resources.Load<AudioMixer>("Mixer");
            Mixer.SetFloat("Bgm", BgmLoud);
            Mixer.SetFloat("Bgm", SELoud);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        SceneIndex = 0;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Battle")
        {
            SceneIndex = 1;
            StartStage();
        }
        if (SceneManager.GetActiveScene().name == "Main")
        {
            SceneIndex = 2;
        }
    }

    void Update()
    {
        if (SceneIndex == 1)
        {
            UpdateGameLogic();
        }
        else if (SceneIndex == 2)
        {
            // 메인 화면 돌아가기.
        }
    }



    public IEnumerator ShowStory(bool b)
    {
        if(b)
        {
            StroyObj.SetActive(true);
            while (StroyObj.GetComponent<Image>().color.a != 0.9f)
            {
                Color value = StroyObj.GetComponent<Image>().color;
                value.a += Time.deltaTime/2;
                if (value.a > 0.9f)
                {
                    value.a = 0.9f;
                }
                StroyObj.GetComponent<Image>().color = value;

                Color value2 = StroyObj.transform.GetChild(0).GetComponent<Text>().color;
                value2.a = value.a;
                StroyObj.transform.GetChild(0).GetComponent<Text>().color = value2;
                yield return null;
            }

        }
        else
        {
            while (StroyObj.GetComponent<Image>().color.a != 0.0f)
            {
                Color value = StroyObj.GetComponent<Image>().color;
                value.a -= Time.deltaTime/2;
                if (value.a < 0.0f)
                {
                    value.a = 0.0f;
                }
                StroyObj.GetComponent<Image>().color = value;
                Color value2 = StroyObj.transform.GetChild(0).GetComponent<Text>().color;
                value2.a = value.a;
                StroyObj.transform.GetChild(0).GetComponent<Text>().color = value2;
                yield return null;
            }
            StroyObj.SetActive(false);
        }

    }

    void StartStage()
    {
        StroyObj = GameObject.FindWithTag ("Canvas").transform.Find("Story").gameObject;
        bgmAudio = gameObject.AddComponent<AudioSource>();
        bgmAudio.clip = ResourceManager.Instance.SoundList["Bgm.mp3"];
        bgmAudio.outputAudioMixerGroup = Mixer.FindMatchingGroups("Bgm")[0];
        bgmAudio.Play();
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


    void UpdateGameLogic()
    {
        if (player.GetComponent<BoardObj>().turn == 0)
        {
            if (board.GetTile(player.GetComponent<BoardObj>().Coord).GetComponent<Tile>().Type == eTileAttr.goal)
            {
                if (!player.GetComponent<BoardObj>().IsMoving)
                {
                    Clear();
                }
            }
            else

            {
                PlayerTurn = false;
                if (!player.GetComponent<BoardObj>().IsMoving)
                {
                    HandleAttack();
                    if (!player.Isalive)
                    {
                        Debug.Log("죽음");
                    }
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
                    v.GetComponent<BoardObj>().MoveCoord(movabletile[UnityEngine.Random.Range(0, movabletile.Count)].GetComponent<Tile>().Coord);
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
            SceneIndex = 2;
        }
    }

    void UpdateStage()
    {
        ResetCondition();
        CurrentStageIndex++;
        if (CurrentStage != null)
        {
            GameObject.Destroy(CurrentStage);
        }
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
