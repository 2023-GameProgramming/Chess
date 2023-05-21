using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public Image bakcground;
    // Start is called before the first frame update
    void Start()
    {
        bakcground.sprite = ResourceManager.Instance.ImageList["Main.PNG"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
