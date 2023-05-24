

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
        new FileInfo("0", "감옥에서 일어난 폰은 방 안을 둘러보며 깊은 한숨을 쉬었습니다."),
        new FileInfo("1", "그 비참한 상황이 여전히 머릿속에 떠오르는 것 같았습니다."),
        new FileInfo("2", "그러나 그는 절대로 왕의 폭정 아래 더 이상 살 수 없다는 결심을 다지며 고개를 들었습니다."),
        new FileInfo("3", "왕이여, 이제 그만이다. 너희 폭정에 더 이상 눈감아 줄 수 없다. 내가 이 땅의 평화를 되찾겠다!"),
        new FileInfo("4", "폰의 모험은 이제 막 시작되었습니다."),

        new FileInfo("5", "당신은 적을 잡으면서, 그 능력을 얻습니다."),
        new FileInfo("6", "Cirl 키로 전환 할 수 있습니다."),
        new FileInfo("7", "Shift키는 당신이 갈 수 있는 위치를 보여줄 것입니다."),
        new FileInfo("8", "Shift키를 누르고 있는 한, 가능한 당신의 턴이 지속됩니다."),
        new FileInfo("9", "움직임을 보이는 적은 당신을 다음 차례에 공격합니다."),

        new FileInfo("10", "아마도. 많은 전투가 있었고 당신은 셀 수 없이 왕의 수하들을 무찔렀습니다."),
        new FileInfo("11", "아마도. 당신은 그 과정에서 무고한 시민들의 축복과 지지를 받았습니다."),
        new FileInfo("12", "이 지루하고도 먼 길을 따라 걷는 당신은 꽤나 지쳤습니다."),
        new FileInfo("13", "그럼에도 복수의 칼날은 무뎌지지 않았고"),
        new FileInfo("14", "당신의 심장은 다시 거세게 뛰기 시작합니다."),
        new FileInfo("15", "이제 도착입니다. 어느 때보다 더욱 각오를 다져야합니다.."),
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
