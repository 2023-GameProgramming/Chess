using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(GameManager.Instance.player.transform.position);
    }
}
