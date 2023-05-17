

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Resource
{
    public Dictionary<string, GameObject> Chessprefab;
    public Dictionary<string, AudioClip> SoundList;
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

    FileInfo[] AssetChessfiles =
    {
        new FileInfo("Model", "BishopDark"),
        new FileInfo("Model", "KingDark"),
        new FileInfo("Model", "QueenDark"),
        new FileInfo("Model", "PawnDark"),
        new FileInfo("Model", "PawnLight"),
        new FileInfo("Model", "KngihtDark"),
        new FileInfo("Model", "RookDark"),
    };

    FileInfo[] Soundfiles =
    {
        new FileInfo("Sound", "Bgm.mp3"),
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
        SoundList = new Dictionary<string, AudioClip>();
        LoadSound();
        Chessprefab = new Dictionary<string, GameObject>();
        foreach (var info in AssetChessfiles)
        {
            GameObject prefabObj = Resources.Load<GameObject>(Path.Combine(info.path, info.name));
            Chessprefab[info.name] = prefabObj;
        }
        Debug.Log("Resource.InitializeDone");
    }

    public void LoadSound()
    {
        foreach (var info in Soundfiles)
        {
            string audioClipURL = Path.Combine(Application.streamingAssetsPath, Path.Combine(info.path, info.name));

            WWW www = new WWW(audioClipURL);
            while (!www.isDone)
            {
                Debug.Log("사운드 불러오는 중...");
            }
            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log("사운드 불러오기 성공!");
                AudioClip audioClip = www.GetAudioClip();
                SoundList[info.name] = audioClip;
            }
            else
            {
                Debug.Log("Failed to load sound: " + www.error);
            }
        }
    }

    public GameObject GetPiecePrefab(ePiece type)
    {
        switch (type)
        {
            case ePiece.pawn:
                return Chessprefab["PawnDark"];
            case ePiece.rook:
                return Chessprefab["RookDark"];
            case ePiece.bishop:
                return Chessprefab["BishopDark"];
            case ePiece.knight:
                return Chessprefab["KngihtDark"];  
            case ePiece.queen:
                return Chessprefab["QueenDark"];
            case ePiece.king:
                return Chessprefab["KingDark"];
            default:
                return Chessprefab["PawnDark"];
        }
    }
}
