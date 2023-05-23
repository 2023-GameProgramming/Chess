using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    public RawImage bakcgournd;
    public Slider slider; 
    float Totalprogress;
    float Imgprogress;
    float Soundprogress;
    float Prefabprogress;

    // Start is called before the first frame update
    void Start()
    {
        Totalprogress = 0;
        Imgprogress = 0;
        Soundprogress = 0;
        Prefabprogress = 0;
        string imagePath = Path.Combine(Application.streamingAssetsPath, "Image","Main.PNG");
        if (File.Exists(imagePath))
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            bakcgournd.texture = texture;
        }

        StartCoroutine(ResourceManager.Instance.LoadImage((progress) =>
        {
            Imgprogress = progress;
        }));
        StartCoroutine(ResourceManager.Instance.LoadSound((progress) =>
        {
            Soundprogress = progress;
        }));
        StartCoroutine(ResourceManager.Instance.LoadPrefabAsync((progress) =>
        {
            Prefabprogress = progress;
        }));
    }

    // Update is called once per frame
    void Update()
    {
        Totalprogress = (Imgprogress + Soundprogress + Prefabprogress) / 3 ;
        slider.value = Totalprogress;
        if(Totalprogress == 1)
        {
           SceneManager.LoadScene("Main");
        }
    }
}
