

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector]
    public static ResourceManager Instance = null;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Dictionary<string, GameObject> Chessprefab;
    public Dictionary<string, AudioClip> SoundList;
    public Dictionary<string, Sprite> ImageList;
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
        new FileInfo("Model", "KnightDark"),
        new FileInfo("Model", "RookDark"),
    };

    FileInfo[] Soundfiles =
    {
        new FileInfo("Sound", "Bgm.mp3"),
        new FileInfo("Sound", "Bgm.mp3"),
    };
    FileInfo[] Imagefiles =
{
        new FileInfo("Image", "Main.PNG"),
        new FileInfo("Image", "bishop.PNG"),
        new FileInfo("Image", "king.PNG"),
        new FileInfo("Image", "queen.PNG"),
        new FileInfo("Image", "pawn.PNG"),
        new FileInfo("Image", "knight.PNG"),
        new FileInfo("Image", "rook.PNG"),
        new FileInfo("Image", "Back.PNG"),
        new FileInfo("Image", "Front.PNG"),
    };

    public IEnumerator LoadImage(Action<float> callback)
    {
        ImageList = new Dictionary<string, Sprite>();
        int count = 0;
        foreach (var info in Imagefiles)
        {
            string imagePath = Path.Combine(Application.streamingAssetsPath, info.path, info.name);
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imagePath))
            {
                var request = www.SendWebRequest();
                while (!request.isDone)
                {
                    callback((request.progress + count)/Imagefiles.Length);
                    yield return null;

                }
                callback(request.progress);
               

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    ImageList[info.name] = sprite;
                }
                else
                {
                    Debug.Log("Failed to load image: " + imagePath);
                }
            }
            count++;
        }
    }

    public IEnumerator LoadPrefab(Action<float> callback)
    {
        Chessprefab = new Dictionary<string, GameObject>();
        int count = 0;
        foreach (var info in AssetChessfiles)
        {
            string prefabPath = Path.Combine(info.path, info.name);
            ResourceRequest request = Resources.LoadAsync<GameObject>(prefabPath);
            while (!request.isDone)
            {
                callback((request.progress + count) / AssetChessfiles.Length);
                yield return null;

            }
            callback(request.progress);

            GameObject prefabObj = request.asset as GameObject;
            if (prefabObj != null)
            {
                Chessprefab[info.name] = prefabObj;
            }
            else
            {
                Debug.Log("Failed to load prefab: " + prefabPath);
            }
            count++;
        }
    }

    public IEnumerator LoadSound(Action<float> callback)
    {
        SoundList = new Dictionary<string, AudioClip>();
        int count = 0;
        foreach (var info in Soundfiles)
        {
            string audioClipURL = Path.Combine(Application.streamingAssetsPath, Path.Combine(info.path, info.name));
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioClipURL, AudioType.UNKNOWN))
            {

                var request = www.SendWebRequest();
                while(!request.isDone)
                {
                    callback((request.progress + count) / Soundfiles.Length);
                    yield return null;

                }
                callback(request.progress);
                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    SoundList[info.name] = audioClip;
                }
                else
                {
                    Debug.Log("Failed to load sound: " + www.error);
                }
            }
            count++;
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
                return Chessprefab["KnightDark"];  
            case ePiece.queen:
                return Chessprefab["QueenDark"];
            case ePiece.king:
                return Chessprefab["KingDark"];
            default:
                return Chessprefab["PawnDark"];
        }
    }
}
