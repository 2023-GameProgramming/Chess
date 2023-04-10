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
            //if ���� �ƹ��� ������ �� ���� MovingObjNum�� 0�� ���
                PlayerTurn = true;
                Player.GetComponent<BoardObj>().turn = Player.GetComponent<BoardObj>().delay;
            //else �װ��� �ƴ϶��
                if ((MovingObjNum == 1 && Player.GetComponent<BoardObj>().IsMoving) ||
                    MovingObjNum == 0)
                {
                    //�÷��̾ �ٷ� ������ �� �ִ� enemy�� ã���ϴ�.
                    GameObject enemy = null; // enemies�� ��ȸ�ϸ鼭 �߰��� ù ������Ʈ
                    if (enemy != null)
                    {
                        if (MovingObjNum == 0)
                        {
                            Player.OnAttacked(new Vector2());
                            //enemy turn �ʱ�ȭ
                        }
                        else
                        {
                            //enemy.MoveCoord
                        }
                    }
                    else
                    {
                        //�÷��̾ üũ�� �� �ִ� enemy�� ������ �ݴϴ�.
                        //�������� �����ϰ� �������ݴϴ�.
                        //�������־����� enemy�� ���� �ʱ�ȭ�մϴ�.
                    }
                }
        }
        // �÷��̾� ��ġ�� ���� �����̸�
        Clear();
    }
    void Clear()
    {
        //���������� �� ����������
        UpdateStage();
        //�ƴ϶�� ���� ������
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
