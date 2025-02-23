using UnityEditor;
using UnityEngine;

public class BoardSpawnerEditor : EditorWindow
{
    private GameObject tilePrefab;
    private int rows;
    private int columns;
    private int squareSize = 1;


    [MenuItem("Tools/Board Spawner")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(BoardSpawnerEditor));
        window.titleContent = new GUIContent("Board Spawner");    
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(500,500, 200, 100), "Board Spawner");
        
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);
        squareSize = EditorGUILayout.IntSlider("Square Size", squareSize, 1, 10);
        
        if (GUILayout.Button("Spawn Board"))
        {
            SpawnBoard();
        }
    }

    private void SpawnBoard()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("No tile prefab selected.");
            return;
        }
        
        if (Board.Instance != null)
        {
            DestroyImmediate(Board.Instance.gameObject, false);
        }

        if (columns % 2 == 0 || rows % 2 == 0)
        {
            Debug.LogError("Please select an uneven number of rows and columns so that a center tile can be selected");
            return;
        }
        
        float offsetX = (columns - 1 * squareSize) / 2f;
        float offsetZ = (rows - 1 * squareSize) / 2f;
        
        GameObject parent = new GameObject("Board Parent");
        Board board = parent.AddComponent<Board>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Tile newTile = Instantiate(tilePrefab, new Vector3(j * squareSize - squareSize * offsetX, 0, i * squareSize - squareSize * offsetZ), Quaternion.identity)
                    .GetComponent<Tile>();
                newTile.name = "Tile " + i + "_" + j;
                newTile.transform.parent = parent.transform;
                newTile.transform.localScale *= squareSize;
                board.AddTile(newTile);
                
                if (i == columns / 2 && j == rows / 2)
                {
                    board.CenterTile = newTile;
                    newTile.name = "Center Tile";
                }
            }
        }

        for (int i = 0; i < squareSize; i++)
        {
            GameObject tileSizeIndicator = new GameObject(Board.SizeIndicatorString);
            tileSizeIndicator.transform.parent = parent.transform;
        }
    }
}
