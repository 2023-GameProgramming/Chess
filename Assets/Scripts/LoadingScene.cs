using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
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
