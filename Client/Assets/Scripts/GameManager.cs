using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public HighlightController highlightGrid;
    public GameObject normalGrid;
    public TowerData[] towers;
    public Vector3[,] grid;
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public Vector2Int cellSize = new Vector2Int(2, 2);

#if UNITY_EDITOR
    private void OnValidate()
    {
        towers =
            AssetDatabase.FindAssets($"t:{nameof(TowerData)}")
                         .Select(g => AssetDatabase.LoadAssetAtPath<TowerData>(AssetDatabase.GUIDToAssetPath(g)))
                         .ToArray();
    }
#endif

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        DetectGridRay();
    }

    private void GenerateGrid()
    {
        grid = new Vector3[gridSize.x, gridSize.y];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                grid[i, j] = transform.position + new Vector3(i * cellSize.x, 0, j * cellSize.y);
                normalGrid.transform.position = grid[i, j];
                Instantiate(normalGrid);
            }
        }
    }

    private void DetectGridRay()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, 1 << 8))
        {
            highlightGrid.SetPosition(hit.point);
            Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 100);
        }
    }

    public Vector3 GetGridPosition(Vector3 position)
    {

        (int x, int y) index = (0, 0);
        (float width, float height) cellRect = (cellSize.x / 2f, cellSize.y / 2f);
        Debug.Log(grid[0, 0]);
        for (int i = 0; i < gridSize.x; i++)
        {
            //Debug.Log($"{position.x}, {grid[i, 0].x - cellRect.width}, {grid[i, 0].x + cellRect.width}");
            if (position.x >= grid[i, 0].x - cellRect.width && position.x <= grid[i, 0].x + cellRect.width)
            {
                index.x = i;
                Debug.LogWarning(i);
                break;
            }
        }

        for (int j = 0; j < gridSize.y; j++)
        {
            if (position.z > grid[0, j].z - cellRect.height /*&& position.z < grid[i, j].z + cellSize.y - cellRect.height*/)
            {
                index.y = j;
            }
        }
        return grid[index.x, index.y];
    }
}
