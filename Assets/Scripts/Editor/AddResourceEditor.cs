using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AddResourceEditor : Editor
{
    static AddResourceEditor()
    {
        Resource.Instance.Initialize();
    }
}
