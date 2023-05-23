using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTrigger : MonoBehaviour
{

    bool showLine;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        showLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        int a = 0;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (!showLine)
        {
            if (num == -1)
            {
                showLine = false;
              StartCoroutine(GameManager.Instance.ShowStory(false));
            }
            else
            {
                showLine = false;
                if (!GameManager.Instance.StroyObj.active)
                {
                    StartCoroutine(GameManager.Instance.ShowStory(true));
                }
                GameManager.Instance.StroyObj.transform.GetChild(0).GetComponent<Text>().text = ResourceManager.Instance.StoryLIne[num].name;
            }
        }
    }

}
