using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class StageMaker : EditorWindow
{
    static Resource resource;

    TileAttr tileType;
    ePiece pieceType;
    int delay = 1;
    float movetime = 1;
    int sight = -1;

    private bool active1;
    private bool active2;
    GUIStyle style1;
    GUIStyle style2;
    Vector2Int matrix;


    bool repaint = false;
    private void OnGUI()
    {
        if (repaint)
        { 
            Repaint();
            repaint = false;
        }
        if (style1 == null)
        {
            style1 = new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.yellow } };
        }
        if (style2 == null)
        {
            style2 = new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.yellow } };
        }

        #region MakeBoard

        matrix = EditorGUILayout.Vector2IntField("MakeBoard", matrix);
        if (GUILayout.Button("MakeNewBoard"))
        {
            CreateTiles(matrix.y, matrix.x);
        }
        #endregion

        GUILayout.Space(30);

        #region SetTileAttr
        EditorGUILayout.LabelField("SetTileAttr");
        SetButtonStyle(active1, style1);
        SetButtonStyle(active2, style2);
        if (GUILayout.Button("Active", style1))
        {
            active1 = !active1;
            active2 = false;
        }

        EditorGUI.BeginChangeCheck();
        tileType = (TileAttr)EditorGUILayout.EnumPopup("Choose", tileType);
        if (EditorGUI.EndChangeCheck())
        {
            if (Selection.activeGameObject != null)
            {
                Tile tile;
                if (Selection.activeGameObject.TryGetComponent<Tile>(out tile))
                {
                    tile.type = tileType;
                    SetColor(tile);
                }
            }
        }
        #endregion

        GUILayout.Space(30);

        #region ModifyPiece
        EditorGUILayout.LabelField("ModifyPiece");
        if (GUILayout.Button("Active", style2))
        {
            active2 = !active2;
            active1 = false;
        }

        EditorGUI.BeginChangeCheck();
        pieceType = (ePiece)EditorGUILayout.EnumPopup("Choose", pieceType);
        sight = EditorGUILayout.IntField("sight", sight);
        delay = EditorGUILayout.IntField("delay", delay);
        movetime = EditorGUILayout.FloatField("movetime", movetime);
        if (EditorGUI.EndChangeCheck())
        {
            if (Selection.activeGameObject != null)
            {
                BoardObj boardobj;
                if (Selection.activeGameObject.TryGetComponent<BoardObj>(out boardobj))
                {
                    SetBoardObjAttr(boardobj);
                }
                Tile tile;
                if (Selection.activeGameObject.TryGetComponent<Tile>(out tile))
                {
                    boardobj = FindEnemy(tile.coord);
                    if (boardobj != null)
                    {
                        SetBoardObjAttr(boardobj);
                    }
                }


            }
        }
        #endregion
        GUILayout.Space(30);

        if (GUILayout.Button("ReDraw"))
        {
            GameObject[] stage = GameObject.FindGameObjectsWithTag("Stage");
            foreach (var s in stage)
            {
                GameObject tiles = s.transform.Find("Tiles").gameObject;
                if (tiles != null)
                {
                    for (int i = 0; i < tiles.transform.childCount; i++)
                    {
                        Transform childTransform = tiles.transform.GetChild(i);
                        SetColor(childTransform.GetComponent<Tile>());
                    }
                }

                GameObject enemies = s.transform.Find("Enemies").gameObject;
                if (enemies != null)
                {
                    for (int i = 0; i < enemies.transform.childCount; i++)
                    {
                        Transform childTransform = enemies.transform.GetChild(i);
                        childTransform.GetComponent<SpriteRenderer>().sprite = Resource.Instance.GetPieceSprite(childTransform.GetComponent<BoardObj>().Type);
                    }
                }
            }
        }
    }


    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        GameObject obj = GetClickedObj(e);
        if (active1)
        {
            if (obj != null)
            {
                Tile tile = obj.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.type = tileType;
                    SetColor(tile);
                    EditorSceneManager.MarkSceneDirty(obj.scene);
                }
            }
        }

        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                BoardObj boardobj = FindEnemy(tile.coord);
                if (boardobj != null && e.button == 0)
                {
                    if (e.shift && active2)
                    {
                        GameObject.DestroyImmediate(boardobj.gameObject);
                    }
                    else
                    {
                        pieceType = boardobj.Type;
                        sight = boardobj.sight;
                        delay = boardobj.delay;
                        movetime = boardobj.movetime;
                        Repaint();
                    }
                }

                if (boardobj == null && active2 && e.button == 0 && !e.shift)
                {
                    GameObject enemyPrefab = Resources.Load<GameObject>("Basic/Enemy");
                    GameObject enemyobj = PrefabUtility.InstantiatePrefab(enemyPrefab) as GameObject;
                    float height = enemyobj.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
                    enemyobj.transform.position = new Vector3(tile.transform.position.x, height / 2, tile.transform.position.z);
                    BoardObj enemy = enemyobj.GetComponent<BoardObj>();
                    enemy.Coord =tile.coord;
                    SetBoardObjAttr(enemy);
                }
            }
        }

    }

    BoardObj FindEnemy(Vector2 coord)
    {
        GameObject enemies = GameObject.Find("Enemies");
        if (enemies == null)
        {
            enemies = new GameObject("Enemies");
            enemies.transform.SetParent(GameObject.Find("Stage").transform);
        }
        for (int i = 0; i < enemies.transform.childCount; i++)
        {
            Transform childTransform = enemies.transform.GetChild(i);
            BoardObj boardobj = childTransform.GetComponent<BoardObj>();
            if (boardobj.Coord == coord)
            {
                return boardobj;
            }
        }
        return null;
    }

    private void OnEnable()
    {
        Resource.Instance.Initialize();
        active1 = false;
        active2 = false;
        GameObject obj = GameObject.Find("Tiles");
        if (obj != null)
        {
            Board board = obj.GetComponent<Board>();
            matrix = new Vector2Int(board.col, board.row);
        }


        obj = GameObject.Find("Stage");
        if (obj == null)
        {
            GameObject stage  = new GameObject("Stage");
            stage.tag = "Stage";
        }
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    [MenuItem("Window/Custom/StageMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<StageMaker>("StageMaker");
    }

    void SetButtonStyle(bool active, GUIStyle style)
    {
        if (style != null)
        {
            if (active)
            {
                style.normal.textColor = Color.yellow;
            }
            else
            {
                style.normal.textColor = Color.white;
            }
        }
    }
    void SetBoardObjAttr(BoardObj enemy)
    {
        enemy.delay = delay;
        enemy.sight = sight;
        enemy.Type = pieceType;
        enemy.movetime = movetime;
        GameObject enemies = GameObject.Find("Enemies");
        if (enemies == null)
        {
            enemies = new GameObject("Enemies");
            enemies.transform.SetParent(GameObject.Find("Stage").transform);
      
        }
        enemy.transform.SetParent(enemies.transform);
        enemy.GetComponent<SpriteRenderer>().sprite = Resource.Instance.GetPieceSprite(pieceType);
        EditorSceneManager.MarkSceneDirty(enemy.gameObject.scene);
    }

    private void CreateTiles(int row, int col)
    {
        //프리팹 로드.
        GameObject tilePrefab = Resources.Load<GameObject>("Basic/Tile");
        GameObject tiles = GameObject.Find("Tiles");
        GameObject enemies = GameObject.Find("Enemies");
        if (tiles != null)
        {
            DestroyImmediate(tiles);
        }
        if (enemies != null)
        {
            DestroyImmediate(enemies);
        }

        tiles = new GameObject("Tiles");
        tiles.transform.SetParent(GameObject.Find("Stage").transform);

        Board board = tiles.AddComponent<Board>();
        board.row = row;
        board.col = col;
        // 타일 오브젝트 생성
        for (int i = 0; i < row; i++)
        {
            for (int k = 0; k < col; k++)
            {
                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                Tile t = tileObject.GetComponent<Tile>();
                t.coord = new Vector2(i, k);
                // Mesh의 bounds를 이용하여 quad 오브젝트의 길이를 구한다.
                MeshRenderer meshRenderer = tileObject.GetComponent<MeshRenderer>();
                // 타일 오브젝트의 위치 설정
                float tileLength = meshRenderer.bounds.size.x;
                tileObject.transform.position = new Vector3(k * tileLength, 0, i * tileLength);
                //타일 오브젝트를 타일스 오브젝트에 자식으로 넣는다.
                tileObject.transform.SetParent(tiles.transform);
            }
        }
    }
    void SetColor(Tile tile)
    {
        Material mat = tile.GetComponent<Renderer>().sharedMaterial;
        if(mat == null)
        {
            mat = Resources.Load<Material>("Materials/Tile"); 
        }
        var tempMaterial = new Material(mat);
        tile.GetComponent<Renderer>().sharedMaterial = tempMaterial;
        switch (tile.type)
        {
            case TileAttr.basic:
                tile.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.white);
                break;
            case TileAttr.water:
                tile.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.blue);
                break;
            case TileAttr.wall:
                tile.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.black);
                break;
            case TileAttr.goal:
                tile.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.green);
                break;
            default:
                tile.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.white);
                break;
        }
    }
    GameObject GetClickedObj(Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

}
