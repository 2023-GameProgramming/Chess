using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeBoard))]
public class MakeBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MakeBoard makeBoard = target as MakeBoard; // MakeBoard Ŭ������ �ν��Ͻ��� �����ɴϴ�.
        makeBoard.row = EditorGUILayout.IntField("Row", makeBoard.row);
        makeBoard.col = EditorGUILayout.IntField("Col", makeBoard.col);

        // "Create Tiles" ��ư�� �����ϰ�, Ŭ�� �� CreateTiles() �޼ҵ带 �����մϴ�.
        if (GUILayout.Button("Create Tiles"))
        {
            CreateTiles(makeBoard.row, makeBoard.col);
        }
    }
    private void CreateTiles(int row, int col)
    {
        //������ �ε�.
        GameObject tilePrefab = Resources.Load<GameObject>("Tile");
        GameObject tiles = GameObject.Find("Tiles");
        if (tiles != null)
        {
            DestroyImmediate(tiles);
        }
        tiles = new GameObject("Tiles");
        // Ÿ�� ������Ʈ ����
        for (int i = 0; i<row; i ++)
        {
            for (int k = 0; k < col; k++)
            {
                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                Tile t = tileObject.GetComponent<Tile>();
                t.order = new Vector2(i, k);
                // Mesh�� bounds�� �̿��Ͽ� quad ������Ʈ�� ���̸� ���Ѵ�.
                MeshRenderer meshRenderer = tileObject.GetComponent<MeshRenderer>();
                // Ÿ�� ������Ʈ�� ��ġ ����
                float tileLength = meshRenderer.bounds.size.x;
                tileObject.transform.position = new Vector3(k* tileLength, i* tileLength, 0);
                //Ÿ�� ������Ʈ�� Ÿ�Ͻ� ������Ʈ�� �ڽ����� �ִ´�.
                tileObject.transform.SetParent(tiles.transform);
            }
        }
    }
}