using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int Coord; //Edit Init

    //[HideInInspector]
    public eTileAttr Type; //Edit Init

    public bool IsPossibleToMove()
    {
        return (Type == eTileAttr.basic) || (Type == eTileAttr.goal) || (Type == eTileAttr.Start);
    }
    
    public void ChangeColor(Color col)
    {
        Material mat = GetComponent<MeshRenderer>().material;
        if (mat.color != col)
        {
            mat.color = col;
        }
    }
    void Start()
    {
        Material mat = GetComponent<MeshRenderer>().material = new Material(Resources.Load<Material>("Materials/Tile"));
        Color color = mat.color;
        color.a = 0;
        mat.color = color;
    }

}
