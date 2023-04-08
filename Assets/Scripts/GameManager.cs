using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    public GameObject enemy;
    public Board board;
    public Player player;
    public int MovingObj;

    void Start()
    {

    }

    void Update()
    {

    }
}
