using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(BoardObj))]
public class BoardObjEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoardObj obj = target as BoardObj;
        EditorGUILayout.LabelField($"Order: {obj.Coord.x}, {obj.Coord.y}");
        EditorGUILayout.LabelField($"type: {obj.Type}");
        EditorGUILayout.LabelField($"sight: {obj.sight}");
        EditorGUILayout.LabelField($"turn: {obj.turn}");
        EditorGUILayout.LabelField($"delay: {obj.delay}");
        EditorGUILayout.LabelField($"movetime: {obj.movetime}");
    }
}
