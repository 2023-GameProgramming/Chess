using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour , IPointerEnterHandler
{
    private AudioSource onButtonAudio;
    private AudioSource ClickAudio;
    // Start is called before the first frame update
    void Start()
    {
        onButtonAudio = this.gameObject.AddComponent<AudioSource>();
        onButtonAudio.clip = ResourceManager.Instance.SoundList["OnButton.mp3"];
        onButtonAudio.outputAudioMixerGroup = GameManager.Instance.Mixer.FindMatchingGroups("SE")[0];
        onButtonAudio.volume -= 0.8f;
        ClickAudio = this.gameObject.AddComponent<AudioSource>();
        ClickAudio.clip = ResourceManager.Instance.SoundList["Click.mp3"];
        ClickAudio.volume -= 0.5f;
        ClickAudio.outputAudioMixerGroup = GameManager.Instance.Mixer.FindMatchingGroups("SE")[0];
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        ClickAudio.Play();
        StartCoroutine(LoadBattleScene());

    }

    IEnumerator LoadBattleScene()
    {
        while (ClickAudio.isPlaying)
        {
          yield return null;
        }
        SceneManager.LoadScene("Battle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ClickAudio.isPlaying)
        {
            onButtonAudio.Play();
        }
    }
}
