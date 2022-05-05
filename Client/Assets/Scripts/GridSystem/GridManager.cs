using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sirenix.OdinInspector;

public class GridManager : SerializedMonoBehaviour
{
    [Header("Settings")]
    public Vector2Int gridSize;
    public Vector2Int cellSize;
    public Vector3 centerPos;

    [Header("Components")]
    public GridSelector gridSelector;

    [Header("Prefabs"), AssetsOnly]
    public Cell cellPrefab;

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<Vector2Int, Cell> content;

    public static GridManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        ResetCells();
        DetectRay();
    }

    private void LateUpdate()
    {
        UpdateCells();
    }

    [Button("Create Grid")]
    public void GenerateGrid()
    {
        if (content == null)
        {
            content = new Dictionary<Vector2Int, Cell>();
        }
        else
        {
            foreach (var c in content)
            {
                DestroyImmediate(c.Value.gameObject);
            }
            content.Clear();
        }

        var spawnPos = transform.position;
        if (gridSize.x % 2 == 0)
        {
            spawnPos.z -= cellSize.x * gridSize.x * .5f;
        }
        if (gridSize.y % 2 == 0)
        {
            spawnPos.x -= cellSize.y * gridSize.y * .5f;
        }

        var tmpSpawnPos = spawnPos;
        for (int y = 0; y < gridSize.x; y++)
        {
            for (int x = 0; x < gridSize.y; x++)
            {
                var cell = Instantiate(cellPrefab, transform);
                cell.name = $"{y}-{x}";
                cell.transform.position = tmpSpawnPos;
                cell.transform.localScale = new Vector3(cellSize.x, 1, cellSize.y);
                tmpSpawnPos.x += cellSize.x;

                content[new Vector2Int(x, y)] = cell;
            }
            tmpSpawnPos.x = spawnPos.x;
            tmpSpawnPos.z += cellSize.y;
        }
    }

    private void DetectRay()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, 1 << 8))
        {
            // Debug.Log(hit.collider.name);
            gridSelector.SelectCell(hit.transform.GetComponent<Cell>());
        }
    }

    private void ResetCells()
    {
        foreach (var c in content)
        {
            c.Value.SetStats(CellState.Empty);
        }
    }

    private void UpdateCells()
    {
        foreach (var c in content)
        {
            c.Value.UpdateStats();
        }
    }

    public Vector2Int GetCellIndexByCell(Cell cell)
    {
        return content.Where(c => c.Value == cell).Single().Key;
    }

    public Cell GetCellByIndex(Vector2Int index)
    {
        return content[index];
    }
}

public enum CellState
{
    Empty = 0,
    Select = 1,
    Block = 2
}