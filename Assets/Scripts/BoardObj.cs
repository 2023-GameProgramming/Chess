using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObj :MonoBehaviour
{
    #region EditInit
    [HideInInspector]
    public Vector2Int Coord; // EditInit
    [HideInInspector]
    public ePiece Type; // EditInit
    [HideInInspector]
    public int sight; // EditInit
    [HideInInspector]
    public int delay;  // EditInit
    [HideInInspector]
    public float movetime; // EditInit
    #endregion EditInit

    public int turn;
    [HideInInspector]
    public bool IsMoving;
    public AudioSource MoveAudio;
    public event Action<GameObject, Vector2Int, Vector2Int> OnMoveCoord;

    Vector3 startPoint;
    Vector3 endPoint;

    Animator anim;
    AnimationClip readyAnim;
    #region MonoBehavior

    private void Start()
    {
        anim = this.gameObject.AddComponent<Animator>();
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Ready");
        readyAnim = Resources.Load<AnimationClip>("Ready");
        readyAnim.name = "Ready";
        readyAnim.wrapMode = WrapMode.Loop; 
        turn = delay;
        IsMoving = false;
        GameObject.Instantiate(ResourceManager.Instance.GetPiecePrefab(Type)).transform.SetParent(this.transform, false);
        MoveAudio = gameObject.AddComponent<AudioSource>();
        MoveAudio.clip = ResourceManager.Instance.SoundList["Move.mp3"];
        MoveAudio.outputAudioMixerGroup =GameManager.Instance.Mixer.FindMatchingGroups("SE")[0];
    }
    #endregion MonoBehavior

    #region IBoardObj
    public void ResetTurn()
    {
        turn = delay;
    }


    private void Update()
    {
        if (turn == 1)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Ready"))
            {
                anim.Play("Ready");
            }
        }
        else
        {
            anim.Play(null);
        }
    }

    public void DecreaseTurn()
    {
        turn -= 1;
        if(turn <0)
        {
            turn = 0;
        }
    }
    public Vector2 SetCoord(Vector2Int crd)
    {
        Coord = crd;
        transform.position = endPoint = GameManager.Instance.board.GetTile(crd).transform.position;
        return crd;
    }

    public Vector2 MoveCoord(Vector2Int crd) 
    {
        Player playerComponent;
        TryGetComponent<Player>(out playerComponent);
        if (playerComponent == null)

        {
            GameManager.Instance.enemies.UpdateDict(gameObject, Coord, crd);
        }
        Coord = crd;
        IsMoving = true;
        startPoint = transform.position;
        endPoint = GameManager.Instance.board.GetTile(crd).transform.position;
        StartCoroutine(ChangeTransfrom());
        return crd;
    }
    #endregion


    IEnumerator ChangeTransfrom()
    {
        GameManager.Instance.MovingObjNum += 1;
        // 이동이 linear해도 좋고, https://easings.net 를 참고해서 역동적으로 움직여도 좋습니다
        float elapsedTime = 0f;
        while (elapsedTime < movetime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPos = startPoint + (endPoint - startPoint) / movetime * elapsedTime;
            transform.position = newPos;
            yield return null;
        }
        transform.position = endPoint;
        IsMoving = false;
        GameManager.Instance.MovingObjNum -= 1;

        if (gameObject.name != "temp")
        {
            MoveAudio.Play();
        }
    }
}
