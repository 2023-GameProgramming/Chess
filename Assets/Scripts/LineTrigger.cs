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
    public IEnumerator ShowStory(bool b, GameObject StroyObj)
    {
        if (b)
        {
            StroyObj.SetActive(true);
            while (StroyObj.GetComponent<Image>().color.a != 0.9f)
            {
                Color value = StroyObj.GetComponent<Image>().color;
                value.a += Time.deltaTime / 2;
                if (value.a > 0.9f)
                {
                    value.a = 0.9f;
                }
                StroyObj.GetComponent<Image>().color = value;

                Color value2 = StroyObj.transform.GetChild(0).GetComponent<Text>().color;
                value2.a = value.a;
                StroyObj.transform.GetChild(0).GetComponent<Text>().color = value2;
                yield return null;
            }

        }
        else
        {
            while (StroyObj.GetComponent<Image>().color.a != 0.0f)
            {
                Color value = StroyObj.GetComponent<Image>().color;
                value.a -= Time.deltaTime / 2;
                if (value.a < 0.0f)
                {
                    value.a = 0.0f;
                }
                StroyObj.GetComponent<Image>().color = value;
                Color value2 = StroyObj.transform.GetChild(0).GetComponent<Text>().color;
                value2.a = value.a;
                StroyObj.transform.GetChild(0).GetComponent<Text>().color = value2;
                yield return null;
            }
            StroyObj.SetActive(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!showLine)
        {
            if (num == -1)
            {
              showLine = false;
              StartCoroutine(ShowStory(false, GameManager.Instance.StroyObj));
            }
            else
            {
                showLine = false;
                if (!GameManager.Instance.StroyObj.active)
                {
                    StartCoroutine(ShowStory(true, GameManager.Instance.StroyObj));
                }
                GameManager.Instance.StroyObj.transform.GetChild(0).GetComponent<Text>().text = ResourceManager.Instance.StoryLIne[num].name;
            }
        }
    }

}
