using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatUI : MonoBehaviour
{
    float lerp;
    int dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        lerp = 0;
        transform.Find("Front").GetComponent<Image>().sprite= ResourceManager.Instance.ImageList["Front.PNG"];
        transform.Find("Back").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["Back.PNG"];
        transform.Find("queenSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["queen.PNG"];
        transform.Find("bishopSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["bishop.PNG"];
        transform.Find("pawnSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["pawn.PNG"];
        transform.Find("rookSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["rook.PNG"];
        transform.Find("knightSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["knight.PNG"];
        transform.Find("kingSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["king.PNG"];
        transform.Find("kingSlot").GetComponent<Image>().color = Color.magenta;
        transform.Find("CurPieceSlot").GetComponent<Image>().sprite = ResourceManager.Instance.ImageList["king.PNG"]; ;
    }

    // Update is called once per frame
    void Update()
    {
        lerp += dir*Time.deltaTime;
        if(lerp>1)
        {
            dir = -1;
        }
        else if(lerp<0)
        {
            dir = 1;
        }
        transform.Find("CurPieceSlot").GetComponent<Image>().color = Color.Lerp(Color.white, Color.magenta, lerp);
    }
}
