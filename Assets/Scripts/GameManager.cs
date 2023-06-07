using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int moveSound;
    public Slider Slider;
    public GameObject StroyObj;
    GameObject EndingObj;
    GameObject LightObj;
    [Range(0.001f, 1.0f)]
    public float BgmLoud;
    [Range(0.001f, 1.0f)]
    public float SELoud;
    #region SingleTon
    [HideInInspector]
    public static GameManager Instance = null;
    #endregion

    public int MaxStageNum = 5;
    public int CurrentStageIndex;

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
    public GameObject CurrentStage;

    [HideInInspector]
    public AudioMixer Mixer;
    float progressbar;
    
    
    AudioSource bgmAudio;
    AudioSource VictoryAudio;
    AudioSource battleAudio;

    void SoundAllStop()
    {
        bgmAudio.Pause();
        VictoryAudio.Pause();
        battleAudio.Pause();
    }

    int SceneIndex;

    #region MonoBehavior
    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
            //BgmLoud = 0.5f;
            //SELoud = 1.0f;
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



    IEnumerator ShowClearScene(bool clear)
    {
        EndingObj.SetActive(true);
        Text text = EndingObj.GetComponentInChildren<Text>();
        if (clear)
        {
            text.text = "I did it. Hurrah!";
        }
        else
        {
            text.text = "Unfortunately, he's dead!";
        }
        while (EndingObj.GetComponent<Image>().color.a != 0.9f || !Input.GetMouseButton(0))
        {
            Color value = EndingObj.GetComponent<Image>().color;
            value.a += Time.deltaTime / 1.5f;
            if (value.a > 0.9f)
            {
                value.a = 0.9f;
            }
            EndingObj.GetComponent<Image>().color = value;
            Color value2 = EndingObj.transform.GetChild(0).GetComponent<Text>().color;
            value2.a = value.a;
            EndingObj.transform.GetChild(0).GetComponent<Text>().color = value2;
            yield return null;
        }
        EndStage();
        if (!Slider.gameObject.active)
        {
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene()
    {
        Slider.gameObject.SetActive(true);
        AsyncOperation operation =  SceneManager.LoadSceneAsync("Main");
        while(!operation.isDone)
        {
            Slider.value = operation.progress;
            yield return null;
        }
    }






    void Update()
    {
        if (SceneIndex == 1)
        {
            UpdateGameLogic();
            //클리어했다면,
            if (CurrentStageIndex == MaxStageNum)
            {
                SoundAllStop();
                VictoryAudio.Play();
                CurrentStageIndex += 1;
                StartCoroutine(ShowClearScene(true));

            }
            else if (!player.Isalive) // 죽었다면 
            {
                SoundAllStop();
                StartCoroutine(ShowClearScene(false));
            }
        }
    }


    void StartStage()
    {

        Slider = GameObject.FindWithTag("LoadingSlider").GetComponent<Slider>();
        Slider.gameObject.SetActive(false);
        LightObj = GameObject.FindWithTag("Light");
        EndingObj =  GameObject.FindWithTag("Canvas").transform.Find("Ending").gameObject;
        EndingObj.SetActive(false);
        moveSound = 0;
        Color value = EndingObj.GetComponent<Image>().color;
        value.a = 0;
        EndingObj.GetComponent<Image>().color = value;
        value = EndingObj.transform.GetChild(0).GetComponent<Text>().color;
        value.a = 0;
        EndingObj.transform.GetChild(0).GetComponent<Text>().color = value;

        StroyObj = GameObject.FindWithTag ("Canvas").transform.Find("Story").gameObject;
        StroyObj.SetActive(false);
        value = StroyObj.GetComponent<Image>().color;
        value.a = 0;
        StroyObj.GetComponent<Image>().color = value;
        value = StroyObj.transform.GetChild(0).GetComponent<Text>().color;
        value.a = 0;
        StroyObj.transform.GetChild(0).GetComponent<Text>().color = value;

        bgmAudio = gameObject.AddComponent<AudioSource>();
        bgmAudio.clip = ResourceManager.Instance.SoundList["Bgm.mp3"];
        bgmAudio.outputAudioMixerGroup = Mixer.FindMatchingGroups("Bgm")[0];
        bgmAudio.Play();

        battleAudio = gameObject.AddComponent<AudioSource>();
        battleAudio.clip = ResourceManager.Instance.SoundList["Battle.mp3"];
        battleAudio.outputAudioMixerGroup = Mixer.FindMatchingGroups("Bgm")[0];


        VictoryAudio = gameObject.AddComponent<AudioSource>();
        VictoryAudio.clip = ResourceManager.Instance.SoundList["Victory.mp3"];
        VictoryAudio.outputAudioMixerGroup = Mixer.FindMatchingGroups("Bgm")[0];




        CurrentStageIndex = -1;
        player = GameObject.Instantiate(Resources.Load<GameObject>("Basic/Player")).GetComponent<Player>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Cursor.SetCursor(null, screenCenter, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        UpdateStage();
    }


    void EndStage()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        bgmAudio.Stop();
    }


    void ResetCondition()
    {
        moveSound = 0;
        Attacker = new MoveInfo();
        PlayerTurn = true;
        ProgressTurn = false;
        CheckCapturedEnemy = false;
        PlayerAttacked = false;
        EnemyAttacked = false;
        MovingObjNum = 0;
        Caturedenemy = null;
        StroyObj.SetActive(false);
        EndingObj.SetActive(false);
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Flag");
        foreach(GameObject i in obj)
        {
            Destroy(i);
        }
    }
    void UpdateGameLogic()
    {
        if (CurrentStageIndex == MaxStageNum || !player.Isalive )
        {
            return;
        }
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
                    if(HandleAttack())
                    {
                        return;
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
    bool HandleAttack()
    {
        if (EnemyAttacked)
        {
            ePiece catured = Caturedenemy.GetComponent<BoardObj>().Type;
            player.CaptureEnemy(Caturedenemy);
            if (catured == ePiece.king)
            {
                Clear();
                return true;
            }
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
        return false;
    }

    
    void Clear()
    {
        if (MaxStageNum > CurrentStageIndex + 1)
        {
            UpdateStage();
        }
        else
        {
            CurrentStageIndex ++;
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
        LightObj.SetActive(CurrentStageIndex != 0);
        if(CurrentStageIndex == 2 || CurrentStageIndex == 4)
        {
            SoundAllStop();
            battleAudio.Play();
        }
        else
        {
            SoundAllStop();
            bgmAudio.Play();
        }
    }



    #endregion
}
