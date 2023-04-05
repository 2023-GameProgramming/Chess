
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    private Material basicMaterial;
    private Material waterMaterial;
    private Material wallMaterial;


    public override void OnInspectorGUI()
    {
        Tile tile = target as Tile;
        EditorGUILayout.LabelField($"Order: {tile.order.x}, {tile.order.y}");
        EditorGUI.BeginChangeCheck();
        tile.type = (Tile.Type)EditorGUILayout.EnumPopup("Type", tile.type);
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
            case Tile.Type.Basic:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                break;
            case Tile.Type.Water:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.blue;
                break;
            case Tile.Type.Wall:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.black;
                break;
            case Tile.Type.Goal:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.green;
                break;
            default:
                tile.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                break;
        }
    }
}