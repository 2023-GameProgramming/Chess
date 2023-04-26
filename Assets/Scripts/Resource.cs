

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
            }
            return _instance;
        }
    }
    public void Initialize()
    {
        sprite = new Dictionary<string, Sprite>();
        foreach (var info in AssetPngfiles)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, info.path, info.name+".png")));
            sprite[info.name] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));

            if(sprite[info.name] == null)
            {
                Debug.Log("ErrorLoadResource : " +info.path + info.name);
            }
        }
        Debug.Log("Resource.InitializeDone");
    }

    public Sprite GetPieceSprite(ePiece type)
    {
        switch (type)
        {
            case ePiece.pawn:
                return sprite["B_Pawn"];
            case ePiece.rook:
                return sprite["B_Rook"];
            case ePiece.bishop:
                return sprite["B_Bishop"];
            case ePiece.knight:
                return sprite["B_Knight"];  
            case ePiece.queen:
                return sprite["B_Queen"];
            case ePiece.king:
                return sprite["B_King"];
            default:
                return sprite["B_Pawn"];
        }
    }
}
