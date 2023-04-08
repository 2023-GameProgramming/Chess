
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tile tile = target as Tile;
        EditorGUILayout.LabelField($"Order: {tile.coord.x}, {tile.coord.y}");
        EditorGUI.BeginChangeCheck();
        tile.type = (TileAttr)EditorGUILayout.EnumPopup("Type", tile.type);
        if (EditorGUI.EndChangeCheck())
        {
            SetColor();
        }
    }

    void SetColor()
    {
        Tile tile = target as Tile;
        var tempMaterial = new Material(tile.GetComponent<Renderer>().sharedMaterial);
        tile.GetComponent<Renderer>().sharedMaterial = tempMaterial;
        switch (tile.type)
        {
            case TileAttr.basic:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                break;
            case TileAttr.water:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.blue;
                break;
            case TileAttr.wall:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.black;
                break;
            case TileAttr.goal:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.green;
                break;
            default:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                break;
        }
    }
}