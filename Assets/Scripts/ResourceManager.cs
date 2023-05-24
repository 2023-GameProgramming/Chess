

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
        new FileInfo("0", "�������� �Ͼ ���� �� ���� �ѷ����� ���� �Ѽ��� �������ϴ�."),
        new FileInfo("1", "�� ������ ��Ȳ�� ������ �Ӹ��ӿ� �������� �� ���ҽ��ϴ�."),
        new FileInfo("2", "�׷��� �״� ����� ���� ���� �Ʒ� �� �̻� �� �� ���ٴ� ����� ������ ���� ������ϴ�."),
        new FileInfo("3", "���̿�, ���� �׸��̴�. ���� ������ �� �̻� ������ �� �� ����. ���� �� ���� ��ȭ�� ��ã�ڴ�!"),
        new FileInfo("4", "���� ������ ���� �� ���۵Ǿ����ϴ�."),

        new FileInfo("5", "����� ���� �����鼭, �� �ɷ��� ����ϴ�."),
        new FileInfo("6", "Cirl Ű�� ��ȯ �� �� �ֽ��ϴ�."),
        new FileInfo("7", "ShiftŰ�� ����� �� �� �ִ� ��ġ�� ������ ���Դϴ�."),
        new FileInfo("8", "ShiftŰ�� ������ �ִ� ��, ������ ����� ���� ���ӵ˴ϴ�."),
        new FileInfo("9", "�������� ���̴� ���� ����� ���� ���ʿ� �����մϴ�."),

        new FileInfo("10", "�Ƹ���. ���� ������ �־��� ����� �� �� ���� ���� ���ϵ��� ���񷶽��ϴ�."),
        new FileInfo("11", "�Ƹ���. ����� �� �������� ������ �ùε��� �ູ�� ������ �޾ҽ��ϴ�."),
        new FileInfo("12", "�� �����ϰ� �� ���� ���� �ȴ� ����� �ϳ� ���ƽ��ϴ�."),
        new FileInfo("13", "�׷����� ������ Į���� �������� �ʾҰ�"),
        new FileInfo("14", "����� ������ �ٽ� �ż��� �ٱ� �����մϴ�."),
        new FileInfo("15", "���� �����Դϴ�. ��� ������ ���� ������ �������մϴ�.."),
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
