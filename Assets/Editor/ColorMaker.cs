using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class ColorMaker : EditorWindow
{
    private string[] options = new string[] { "basic", "water", "wall", "goal" };
    private int selectedOption = 0;
    private bool active = false;
    [MenuItem("Window/ColorMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ColorMaker>("ColorMaker");
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Color Maker");
        GUIStyle style = new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.yellow } };
        if (active)
        {
            style.normal.textColor = Color.yellow;
        }
        else
        {
            style.normal.textColor = Color.white;
        }
        if (GUILayout.Button("Active", style))
        {
            active = !active;

        }
        selectedOption = EditorGUILayout.Popup("Choose", selectedOption, options);
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (active && e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 위치에 있는 타일을 찾습니다.
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.type = (Tile.Type)selectedOption;
                    SetColor(tile);
                    // 변경된 씬을 저장합니다.
                    EditorSceneManager.MarkSceneDirty(hit.collider.gameObject.scene);

                }
            }
        }
        void SetColor(Tile tile)
        {
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
}
