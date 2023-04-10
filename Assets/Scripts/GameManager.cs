using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SingleTon

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    #endregion

    private int CurrentStageIndex;

    public GameObject Enemies;
    public GameObject Tiles;
    public Player Player;


    public bool PlayerTurn;
    public int MovingObjNum;


    #region MonoBehavior

    void Start()
    {
        CurrentStageIndex = -1;
        PlayerTurn = true;
        GameObject playerPrefab = Resources.Load<GameObject>("Basic/Player");
        Player = GameObject.Instantiate(playerPrefab).GetComponent<Player>();
        UpdateStage();
    }

    void Update()
    {
        if (Player.GetComponent<BoardObj>().turn == 0)
        {
            //if 적이 아무도 움직일 수 없고 MovingObjNum가 0인 경우
                PlayerTurn = true;
                Player.GetComponent<BoardObj>().turn = Player.GetComponent<BoardObj>().delay;
            //else 그것이 아니라면
                if ((MovingObjNum == 1 && Player.GetComponent<BoardObj>().IsMoving) ||
                    MovingObjNum == 0)
                {
                    //플레이어를 바로 공격할 수 있는 enemy를 찾습니다.
                    GameObject enemy = null; // enemies를 순회하면서 발견한 첫 오브젝트
                    if (enemy != null)
                    {
                        if (MovingObjNum == 0)
                        {
                            Player.OnAttacked(new Vector2());
                            //enemy turn 초기화
                        }
                        else
                        {
                            //enemy.MoveCoord
                        }
                    }
                    else
                    {
                        //플레이어를 체크할 수 있는 enemy를 움직여 줍니다.
                        //나머지는 랜덤하게 움직여줍니다.
                        //움직여주었으면 enemy의 턴을 초기화합니다.
                    }
                }
        }
        // 플레이어 위치가 골인 지점이면
        Clear();
    }
    void Clear()
    {
        //스테이지가 더 남아있으면
        UpdateStage();
        //아니라면 엔딩 보여줌
    }

    void UpdateStage()
    {
        CurrentStageIndex++;
        GameObject CurrentStage = GameObject.Find("Stage" + CurrentStageIndex.ToString());
        Enemies = CurrentStage.transform.Find("Enemies").gameObject;
        Tiles = CurrentStage.transform.Find("Tiles").gameObject;
        Player.GetComponent<BoardObj>().turn = Player.GetComponent<BoardObj>().delay;
    }
    #endregion
}
