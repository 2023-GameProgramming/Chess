using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{

    [HideInInspector]
    public Vector2 coord;

    [SerializeField]
    public TileAttr type;


    void ChangeColor(Color col)
    {
        
    }

    void Start()
    {
        // ���� �� �����ϰ� �ٲ۴�. 
    }

}
