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
        Material mat =  GetComponent<MeshRenderer>().material =new Material(Resources.Load<Material>("Materials/Tile"));
        Color color = mat.color;
        color.a = 0; 
        mat.color = color;
    }

}
