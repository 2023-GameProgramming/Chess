using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    bool IsAnyMover()
    {
        // 움직 일 수 있는 적이 있다면 true 반환.
        return false;
    }

    public GameObject GetRandomMover()
    {
        return null;
    }

    public GameObject GetChecker()
    {
        return null;
    }

    public GameObject GetAttacker()
    {
        return null;
    }
}
