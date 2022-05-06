using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

public class GridManager : SerializedMonoBehaviour
{
    [Header("Editors")]
    // Just delete or create building, you cannot use this on create grid when play mode.
    public bool saveOnPlay;
    public bool deleteMode;

    [Header("Settings")]
    public Vector2Int gridSize;
    public Vector2Int cellSize;
    public Vector3 centerPos;

    [Header("Components")]
    public GridSelector gridSelector;
    [SceneObjectsOnly]
    public Transform gridParant;
    [SceneObjectsOnly]
    public Transform buildingParant;

    [Header("Prefabs"), AssetsOnly]
    public Cell cellPrefab;

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<CellIndex, Cell> content;

    public static GridManager instance;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (saveOnPlay)
        {
            ReadSaveInEditor();
        }
    }

    private void ReadSaveInEditor()
    {
        if (PlayerPrefs.HasKey(nameof(GridManager)))
        {
            var existBuilding = FindObjectsOfType<Building>();
            foreach(var e in existBuilding)
            {
                StartCoroutine(DestoryInEditor(e.gameObject));
            }

            var json = PlayerPrefs.GetString(nameof(GridManager));
            var save = JsonConvert.DeserializeObject<List<CellSaveData>>(json);
            var removeQueue = new List<CellIndex>();
            foreach (var c in content)
            {
                // Exist
                if (!save.Exists(s => s.x == c.Key.x && s.y == c.Key.y))
                {
                    if (c.Value != null)
                    {
                        StartCoroutine(DestoryInEditor(c.Value.gameObject));
                        removeQueue.Add(c.Key);
                    }
                    continue;
                }

                // Building
                var data = save.Single(s => s.x == c.Key.x && s.y == c.Key.y).buildings;
                c.Value.buildings = data;
                c.Value.BuildWithData();
            }

            foreach (var r in removeQueue)
            {
                content.Remove(r);
            }

            PlayerPrefs.DeleteKey(nameof(GridManager));
        }
    }

    private IEnumerator DestoryInEditor(GameObject gameObject)
    {
        yield return null;
        DestroyImmediate(gameObject);
    }
#endif

    private void Awake()
    {
        instance = this;
        deleteMode = false;
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

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (saveOnPlay)
        {
            var cellSaveData = new CellSaveData[content.Count];
            var saveIndex = 0;
            foreach (var c in content)
            {
                cellSaveData[saveIndex] = new CellSaveData
                {
                    x = c.Key.x,
                    y = c.Key.y,
                    buildings = c.Value.buildings
                };
                saveIndex++;
            }
            var cellSave = JsonConvert.SerializeObject(cellSaveData);
            PlayerPrefs.SetString(nameof(GridManager), cellSave);
            Debug.Log(cellSave);
        }
#endif
    }

    [Button("Create Grid", ButtonSizes.Large)]
    public void GenerateGrid()
    {
        if (content == null)
        {
            content = new Dictionary<CellIndex, Cell>();
        }

        DeleteGrid();

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
                var cell = Instantiate(cellPrefab, gridParant);
                cell.name = $"{y}-{x}";
                cell.transform.position = tmpSpawnPos;
                cell.transform.localScale = new Vector3(cellSize.x, 1, cellSize.y);
                cell.index = new CellIndex(x, y);
                tmpSpawnPos.x += cellSize.x;

                content[cell.index] = cell;
            }
            tmpSpawnPos.x = spawnPos.x;
            tmpSpawnPos.z += cellSize.y;
        }
    }


    [Button("Delete Grid", ButtonSizes.Large)]
    public void DeleteGrid()
    {
        var existBuilding = FindObjectsOfType<Building>();
        foreach (var e in existBuilding)
        {
            StartCoroutine(DestoryInEditor(e.gameObject));
        }

        if (content != null)
        {
            foreach (var c in content)
            {
                DestroyImmediate(c.Value.gameObject);
            }
            content.Clear();
        }
    }

    [HideIf(nameof(deleteMode))]
    [Button("Delete Mode", ButtonSizes.Large)]
    public void EnableDeleteMode()
    {
        deleteMode = true;
    }

    [ShowIf(nameof(deleteMode))]
    [Button("Delete Mode", ButtonSizes.Large), GUIColor(1,0,0)]
    public void DisableDeleteMode()
    {
        deleteMode = false;
    }

    private void DetectRay()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, 1 << 8))
        {
            gridSelector.SelectCell(hit.transform.GetComponent<Cell>());
        }
    }

    private void ResetCells()
    {
        var removeQueue = new List<CellIndex>();
        foreach (var c in content)
        {
            if (c.Value == null)
            {
                removeQueue.Add(c.Key);
                continue;
            }
            c.Value.SetStats(CellState.Empty);
        }

        foreach(var r in removeQueue)
        {
            content.Remove(r);
        }
    }

    private void UpdateCells()
    {
        var removeQueue = new List<CellIndex>();
        foreach (var c in content)
        {
            if (c.Value == null)
            {
                removeQueue.Add(c.Key);
                continue;
            }
            c.Value.UpdateStats();
        }

        foreach (var r in removeQueue)
        {
            content.Remove(r);
        }
    }

    public Cell GetCellByIndex(CellIndex index)
    {
        return content[index];
    }
}

[InlineProperty]
public struct CellIndex
{
    public int x;
    public int y;

    public static CellIndex NegativeOne => new CellIndex(1, 1);

    public CellIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(CellIndex a, CellIndex b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(CellIndex a, CellIndex b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override bool Equals(object obj)
    {
        return this == (CellIndex)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

public enum CellState
{
    Empty = 0,
    Select = 1,
    Block = 2
}

public struct CellSaveData
{
    public int x;
    public int y;
    public List<BuildingData> buildings;
}