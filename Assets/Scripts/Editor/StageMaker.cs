using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class StageMaker : EditorWindow
{
    Vector2Int matrix;
    eTileAttr tileType;
    ePiece pieceType;
    int delay = 1;
    float movetime = 1;
    int sight = -1;

    private bool active1;
    private bool active2;
    GUIStyle style1;
    GUIStyle style2;

    private void OnEnable()
    {
        EditorPrefs.DeleteAll();
        if(ResourceManager.Instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = "Resource";
            ResourceManager.Instance = obj.AddComponent<ResourceManager>();

        }
        ResourceManager.Instance.LoadPrefabsync();
       
        active1 = false;
        active2 = false;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (ResourceManager.Instance != null)
        {
            DestroyImmediate(ResourceManager.Instance.gameObject);
        }
    }


    private void OnGUI()
    {
        if (style1 == null)
        {
            style1 = new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.yellow } };
        }
        if (style2 == null)
        {
            style2 = new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.yellow } };
        }

        #region MakeBoard
        matrix = EditorGUILayout.Vector2IntField("MakeBoard col : row", matrix);
        if (GUILayout.Button("MakeNewBoard"))
        {
            CreateTiles(matrix.x, matrix.y);
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
        tileType = (eTileAttr)EditorGUILayout.EnumPopup("Choose", tileType);
        if (EditorGUI.EndChangeCheck())
        {
            if (Selection.activeGameObject != null)
            {
                Tile tile;
                if (Selection.activeGameObject.TryGetComponent<Tile>(out tile))
                {
                    SetTileAttr(tile);
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
                if (Selection.activeGameObject.TryGetComponent<BoardObj>(out boardobj) || (Selection.activeGameObject.transform.parent != null && Selection.activeGameObject.transform.parent.TryGetComponent<BoardObj>(out boardobj)) )
                {
                    SetBoardObjAttr(boardobj);
                }
                Tile tile;
                if (Selection.activeGameObject.TryGetComponent<Tile>(out tile))
                {
                    boardobj = FindEnemy(tile);
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
            }
        }
    }
    void SetTileAttr(Tile tile)
    {
        tile.Type = tileType;
        EditorUtility.SetDirty(tile);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        GameObject obj = GetClickedObj(e);
        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                if (active1)
                {
                    SetTileAttr(tile);
                    SetColor(tile);
                }
                if (tileType != tile.Type)
                {
                    tileType = tile.Type;
                    Repaint();
                }
                BoardObj boardobj = FindEnemy(tile);
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
                        SetBoardObjAttr(boardobj);
                        Repaint();
                    }
                }

                if (boardobj == null && active2 && e.button == 0 && !e.shift)
                {
                    GameObject enemyPrefab = Resources.Load<GameObject>("Basic/Enemy");
                    GameObject enemyobj = PrefabUtility.InstantiatePrefab(enemyPrefab) as GameObject;
                    enemyobj.transform.position = new Vector3(tile.transform.position.x,  tile.transform.position.y, tile.transform.position.z);
                    BoardObj enemy = enemyobj.GetComponent<BoardObj>();
                    GameObject stage = tile.transform.parent.parent.gameObject;
                    enemy.Coord = tile.Coord;
                    Transform enemiesTrans = stage.transform.Find("Enemies");
                    GameObject enemies = null;
                    if (enemiesTrans == null)
                    {
                        enemies = new GameObject("Enemies");
                        enemies.transform.SetParent(stage.transform);
                        enemies.AddComponent<Enemies>();
                    }
                    else
                    {
                        enemies = enemiesTrans.gameObject;
                    }
                    enemy.transform.SetParent(enemies.transform);
                    SetBoardObjAttr(enemy);
                }
            }
        }
    }

    BoardObj FindEnemy(Tile tile)
    {
        GameObject stage = tile.transform.parent.parent.gameObject;
        Vector2 coord = tile.Coord;
        Transform enemiesTrans = stage.transform.Find("Enemies");
        GameObject enemies = null;
        if (enemiesTrans == null)
        {
            enemies = new GameObject("Enemies");
            enemies.transform.SetParent(stage.transform);
            enemies.AddComponent<Enemies>();
        }
        else
        {
            enemies = enemiesTrans.gameObject;
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


    Vector3 GetTilePos(GameObject stage,Vector2Int crd)
    {
        GameObject tiles = stage.transform.Find("Tiles").gameObject;
        if (tiles != null)
        {
            for (int i = 0; i < tiles.transform.childCount; i++)
            {
                if(tiles.transform.GetChild(i).GetComponent<Tile>().Coord == crd)
                {
                    return tiles.transform.GetChild(i).transform.position;
                }
            }
        }
        return Vector3.zero;
    }

    void SetBoardObjAttr(BoardObj enemy)
    {
        Pawn pawn; 
        enemy.TryGetComponent<Pawn>(out pawn);
        if (pieceType == ePiece.pawn)
        {
            if (pawn == null)
            {
                enemy.gameObject.AddComponent<Pawn>();
            }
        }
        else
        {
            if (pawn != null)
            {
                DestroyImmediate(pawn);
            }
        }
        enemy.delay = delay;
        enemy.sight = sight;
        enemy.Type = pieceType;
        enemy.movetime = movetime;

        if(enemy.transform.childCount != 0)
        {
            GameObject.DestroyImmediate(enemy.transform.GetChild(0).gameObject);
        }

        GameObject obj = GameObject.Instantiate(ResourceManager.Instance.GetPiecePrefab(pieceType));

        Selection.activeGameObject = obj;
        obj.transform.SetParent(enemy.transform, false);
        obj.hideFlags = HideFlags.NotEditable;
        Vector3 pos = GetTilePos(enemy.transform.parent.parent.gameObject, enemy.Coord);
        enemy.transform.position = pos;
        EditorUtility.SetDirty(enemy);
    }

    private void CreateTiles(int col, int row)
    {
        GameObject stage = new GameObject("Stage");
        stage.tag = "Stage";
        //프리팹 로드.

        GameObject enemies = new GameObject("Enemies");
        enemies.AddComponent<Enemies>();
        enemies.transform.SetParent(stage.transform);

        GameObject tilePrefab = Resources.Load<GameObject>("Basic/Tile");
        GameObject tiles = new GameObject("Tiles");
        tiles.transform.SetParent(stage.transform);

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
                t.Coord = new Vector2Int(k, i);
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
        Material mat;
        if (Application.isPlaying)
        {
            mat = tile.GetComponent<Renderer>().material;
        }
        else
        {
            mat = Resources.Load<Material>("Materials/Tile");
            mat = new Material(mat);
            tile.GetComponent<Renderer>().sharedMaterial = mat;
        }

        switch (tile.Type)
        {
            case eTileAttr.basic:
                mat.SetColor("_Color", Color.white);
                break;
            case eTileAttr.water:
                mat.SetColor("_Color", Color.blue);
                break;
            case eTileAttr.wall:
                mat.SetColor("_Color", Color.black);
                break;
            case eTileAttr.goal:
                mat.SetColor("_Color", Color.green);
                break;
            case eTileAttr.Start:
                mat.SetColor("_Color", Color.yellow);
                break;
            default:
                mat.SetColor("_Color", Color.white);
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
