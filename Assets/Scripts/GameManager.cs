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

    public GameObject Enemies;
    public GameObject Tiles;
    public GameObject Player;


    public int MovingObj;


    #region MonoBehavior

    void Start()
    {
        Enemies = GameObject.Find("Enemies");
        Enemies = GameObject.Find("Tiles");
        Enemies = GameObject.Find("Player");
    }

    void Update()
    {

    }

    #endregion
}
