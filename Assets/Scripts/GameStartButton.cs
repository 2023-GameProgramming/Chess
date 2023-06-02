using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
public class GameStartButton : MonoBehaviour , IPointerEnterHandler
{
    private AudioSource onButtonAudio;
    private AudioSource ClickAudio;
    public VideoPlayer videoPlayer;
    public RawImage video;
    public RenderTexture render;
    // Start is called before the first frame update
    void Start()
    {
        video.gameObject.SetActive(false);
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
        StartCoroutine(LoadCutScene());

    }

    IEnumerator LoadCutScene()
    {
        while (ClickAudio.isPlaying)
        {
          yield return null;
        }
        videoPlayer.url = ResourceManager.Instance.GetUrl(ResourceManager.Instance.Videofiles[0]);
        videoPlayer.Play();
        while(!videoPlayer.isPlaying)
        {
           yield return null;
        }
        video.gameObject.SetActive(true);
        StartCoroutine(LoadBattleScene());
    }

    IEnumerator LoadBattleScene()
    {
        float ratio = (float)videoPlayer.width / videoPlayer.height;
        int targetHeight = 250;
        int targetWidth = Mathf.RoundToInt(targetHeight * ratio);
        RenderTexture renderTexture = new RenderTexture(targetWidth, targetHeight, 0);
        video.texture = renderTexture;
        videoPlayer.targetTexture = renderTexture;
        while (videoPlayer.isPlaying)
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
