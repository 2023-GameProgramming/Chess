
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tile tile = target as Tile;
        EditorGUILayout.LabelField($"Order: {tile.Coord.x}, {tile.Coord.y}");
        EditorGUILayout.LabelField($"type: {tile.Type}");
    }
}