using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeBoard))]
public class MakeBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MakeBoard makeBoard = target as MakeBoard; // MakeBoard 클래스의 인스턴스를 가져옵니다.
        makeBoard.row = EditorGUILayout.IntField("Row", makeBoard.row);
        makeBoard.col = EditorGUILayout.IntField("Col", makeBoard.col);

        // "Create Tiles" 버튼을 생성하고, 클릭 시 CreateTiles() 메소드를 실행합니다.
        if (GUILayout.Button("Create Tiles"))
        {
            CreateTiles(makeBoard.row, makeBoard.col);
        }
    }
    private void CreateTiles(int row, int col)
    {
        //프리팹 로드.
        GameObject tilePrefab = Resources.Load<GameObject>("Tile");
        GameObject tiles = GameObject.Find("Tiles");
        if (tiles != null)
        {
            DestroyImmediate(tiles);
        }
        tiles = new GameObject("Tiles");
        // 타일 오브젝트 생성
        for (int i = 0; i<row; i ++)
        {
            for (int k = 0; k < col; k++)
            {
                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                Tile t = tileObject.GetComponent<Tile>();
                t.order = new Vector2(i, k);
                // Mesh의 bounds를 이용하여 quad 오브젝트의 길이를 구한다.
                MeshRenderer meshRenderer = tileObject.GetComponent<MeshRenderer>();
                // 타일 오브젝트의 위치 설정
                float tileLength = meshRenderer.bounds.size.x;
                tileObject.transform.position = new Vector3(k* tileLength, i* tileLength, 0);
                //타일 오브젝트를 타일스 오브젝트에 자식으로 넣는다.
                tileObject.transform.SetParent(tiles.transform);
            }
        }
    }
}