using UnityEditor;
using UnityEngine;

public class BoardSpawnerEditor : EditorWindow
{
    private GameObject prefab;
    private int rows;
    private int columns;

    [MenuItem("Tools/Board Spawner")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(BoardSpawnerEditor));
        window.titleContent = new GUIContent("Board Spawner");    
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(500,500, 200, 100), "Board Spawner");
        
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);

        if (GUILayout.Button("Spawn Board"))
        {
            SpawnBoard();
        }
    }

    private void SpawnBoard()
    {
        if (prefab == null)
        {
            Debug.LogError("No tile prefab selected.");
            return;
        }
        
        float offsetX = (columns - 1) / 2f;
        float offsetZ = (rows - 1) / 2f;
        
        GameObject parent = new GameObject("Board Parent");
        parent.AddComponent<Board>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject newTile = Instantiate(prefab, new Vector3(j - offsetX, 0, i - offsetZ), Quaternion.identity);
                newTile.name = "Tile " + i + "_" + j;
                newTile.transform.parent = parent.transform;
            }
        }
    }
    
}
