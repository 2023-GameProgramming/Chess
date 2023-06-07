

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

    public Dictionary<string, GameObject> AssetPrefab;
    public Dictionary<string, AudioClip> SoundList;
    public Dictionary<string, Sprite> ImageList;
    public struct FileInfo
    {
        public string path;
        public string name;

        public FileInfo(string path, string name)
        {
            this.path = path;
            this.name = name;
        }
    }
    public FileInfo[] StoryLIne =
    {
        new FileInfo("0", "Pawn, rising from prison, sighed deeply as he looked around the room."),
        new FileInfo("1", "The miserable situation still seemed to come into my head."),
        new FileInfo("2", "But he raised his head, determined that he could never live any longer under the tyranny of the king."),
        new FileInfo("3", "King, that's enough. I can no longer overlook your tyranny. I will restore the peace of this land!"),
        new FileInfo("4", "Von's adventure has just begun."),

        new FileInfo("5", "You get the ability, catching the enemy."),
        new FileInfo("6", "You can switch to the Cirl key."),
        new FileInfo("7", "The Shift key will show you where you can go."),
        new FileInfo("8", "As long as you press the Shift key, your turn will last as long as possible."),
        new FileInfo("9", "The enemy who makes a move attacks you next time."),

        new FileInfo("10", "Perhaps. There were many battles and you defeated the king's men countless times."),
        new FileInfo("11", "Perhaps. You have received the blessings and support of innocent citizens in the process."),
        new FileInfo("12", "You're pretty tired of walking along this boring, long road."),
        new FileInfo("13", "And yet the blade of revenge has not become dull"),
        new FileInfo("14", "Your heart starts beating hard again."),
        new FileInfo("15", "We're here now. You have to be more determined than ever.."),
    };

    FileInfo[] Assetfiles =
    {
        new FileInfo("Model", "BishopDark"),
        new FileInfo("Model", "KingDark"),
        new FileInfo("Model", "QueenDark"),
        new FileInfo("Model", "PawnDark"),
        new FileInfo("Model", "PawnLight"),
        new FileInfo("Model", "KnightDark"),
        new FileInfo("Model", "RookDark"),
        new FileInfo("Basic", "GoalFlag"),
    };

    FileInfo[] Soundfiles =
    {
        new FileInfo("Sound", "Bgm.mp3"),
        new FileInfo("Sound", "Move.mp3"),
        new FileInfo("Sound", "Click.mp3"),
        new FileInfo("Sound", "OnButton.mp3"),
        new FileInfo("Sound", "Victory.mp3"),
        new FileInfo("Sound", "Battle.mp3"),
    };
   public FileInfo[] Videofiles =
  {
        new FileInfo("Video", "Opening.mp4"),
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
            string imagePath = GetUrl(info);
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



    public string GetUrl(FileInfo info)
    {
        string url = Path.Combine(Application.streamingAssetsPath, Path.Combine(info.path, info.name));
        if(Application.platform==RuntimePlatform.OSXEditor)
        {
            url = "file://" + url;
        }
        return url;
    }



    public void LoadPrefabsync()
    {
        AssetPrefab = new Dictionary<string, GameObject>();
        foreach (var info in Assetfiles)
        {
            string prefabPath = Path.Combine(info.path, info.name);
            AssetPrefab[info.name] = Resources.Load<GameObject>(prefabPath);
        }
    }

    public IEnumerator LoadPrefabAsync(Action<float> callback)
    {
        AssetPrefab = new Dictionary<string, GameObject>();
        int count = 0;
        foreach (var info in Assetfiles)
        {
            string prefabPath = Path.Combine(info.path, info.name);
            ResourceRequest request = Resources.LoadAsync<GameObject>(prefabPath);
            while (!request.isDone)
            {
                callback((request.progress + count) / Assetfiles.Length);
                yield return null;

            }
            callback(request.progress);

            GameObject prefabObj = request.asset as GameObject;
            if (prefabObj != null)
            {
                AssetPrefab[info.name] = prefabObj;
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
            string audioClipURL = GetUrl(info);
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
                return AssetPrefab["PawnDark"];
            case ePiece.rook:
                return AssetPrefab["RookDark"];
            case ePiece.bishop:
                return AssetPrefab["BishopDark"];
            case ePiece.knight:
                return AssetPrefab["KnightDark"];  
            case ePiece.queen:
                return AssetPrefab["QueenDark"];
            case ePiece.king:
                return AssetPrefab["KingDark"];
            default:
                return AssetPrefab["PawnDark"];
        }
    }
}
