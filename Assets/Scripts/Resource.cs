using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Resource
{
    public Dictionary<string, Sprite> sprite;

    struct FileInfo
    {
        public string path;
        public string name;

        public FileInfo(string path, string name)
        {
            this.path = path;
            this.name = name;
        }
    }

    FileInfo[] AssetPngfiles =
    {
        new FileInfo("Piece", "B_Pawn"),
        new FileInfo("Piece", "B_Knight"),
        new FileInfo("Piece", "B_Rook"),
        new FileInfo("Piece", "B_Bishop"),
        new FileInfo("Piece", "B_Queen"),
        new FileInfo("Piece", "B_King"),
        new FileInfo("Piece", "W_Pawn"),
        new FileInfo("Piece", "W_Knight"),
        new FileInfo("Piece", "W_Rook"),
        new FileInfo("Piece", "W_Bishop"),
        new FileInfo("Piece", "W_Queen"),
        new FileInfo("Piece", "W_King"),
    };


    private static Resource _instance;
    public static Resource Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Resource();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    public void Initialize()
    {
        sprite = new Dictionary<string, Sprite>();
        foreach (var info in AssetPngfiles)
        {
            Debug.Log(info);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, info.path, info.name+".png")));
            sprite[info.name] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
            Debug.Log(sprite[info.name]);
        }
        Debug.Log("Resource.Initialize");
    }
}
