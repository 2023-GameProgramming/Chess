using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public enum Type { Basic, Water, Wall , Goal};

    [HideInInspector]
    public Vector2 order;

    [SerializeField]
    public Tile.Type type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
