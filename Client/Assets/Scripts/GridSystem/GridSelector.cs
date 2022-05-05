using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    [Header("Stats")]
    public bool isAvailable = true;
    public Vector2Int firstSelectGrid = new Vector2Int(-1, -1);
    public Vector2Int lastSelectGrid = new Vector2Int(-1, -1);
    public List<Cell> selectedCells = new List<Cell>();

    [Header("Buildings")]
    public Building buildingPrefab;

    [Header("Colors")]
    public Color hoverColor = Color.white;
    public Color blockColor = Color.red;

    [HideInInspector]
    public Material material;


    public static GridSelector instance;

    public bool IsBuilded => GameManager.instance.build[gridIndex.x, gridIndex.y];
    public Vector2Int gridIndex = new Vector2Int(0, 0);

    private void Awake()
    {
        instance = this;
        firstSelectGrid = -Vector2Int.one;
        lastSelectGrid = -Vector2Int.one;
    }

    private void Start()
    {
        gridIndex = new Vector2Int(0, 0);
    }

    public void SelectCell(Cell cell)
    {
        var mouseStay = Input.GetMouseButton(0);
        var mouseDown = Input.GetMouseButtonDown(0);
        var mouseUp = Input.GetMouseButtonUp(0);

        if (mouseDown)
        {
            firstSelectGrid = GridManager.instance.GetCellIndexByCell(cell);
        }

        if (mouseUp || mouseStay)
        {
            lastSelectGrid = GridManager.instance.GetCellIndexByCell(cell);
        }

        if (firstSelectGrid != -Vector2Int.one)
        {
            var firstSelectCell = GridManager.instance.GetCellByIndex(firstSelectGrid);
            firstSelectCell.SetStats(CellState.Select);

            // Draw Rectangle
            if (firstSelectGrid != lastSelectGrid)
            {
                selectedCells.Clear();
                var x = lastSelectGrid.x - firstSelectGrid.x;
                var y = lastSelectGrid.y - firstSelectGrid.y;
                for (int i = firstSelectGrid.x; i != lastSelectGrid.x + (x > 0 ? 1 : -1); i += x > 0 ? 1 : -1)
                {
                    for (int j = firstSelectGrid.y; j != lastSelectGrid.y + (y > 0 ? 1 : -1); j += y > 0 ? 1 : -1)
                    {
                        var c = GridManager.instance.GetCellByIndex(new Vector2Int(i, j));
                        c.SetStats(CellState.Select);
                        selectedCells.Add(c);
                    }
                }
            }
            else
            {
                selectedCells.Clear();
                selectedCells.Add(GridManager.instance.GetCellByIndex(firstSelectGrid));
            }
        }
        else
        {
            cell.SetStats(CellState.Select);
        }

        // Complete
        if (mouseUp)
        {
            firstSelectGrid = -Vector2Int.one;
            lastSelectGrid = -Vector2Int.one;

            var canBuild = true;
            foreach(var c in selectedCells)
            {
                if (!c.CanBuild)
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                foreach (var c in selectedCells)
                {
                    var building = Instantiate(buildingPrefab);
                    building.transform.position = c.transform.position;
                    c.Build(building);
                }
            }
        }
    }

    public void SetStats(bool isHover)
    {
        isAvailable = isHover;
        if (isHover)
        {
            material.color = hoverColor;
        }
        else
        {
            material.color = blockColor;
        }
    }

    public void Spawn(GameObject obj)
    {
        obj.transform.position = transform.position;
        StartCoroutine(BuildHouse(Instantiate(obj)));
        GameManager.instance.build[gridIndex.x, gridIndex.y] = true;
    }

    private IEnumerator BuildHouse(GameObject obj)
    {
        var scale = obj.transform.localScale;
        scale.y = 0;
        obj.transform.localScale = scale;
        var targetScaleY = Random.Range(1, 6);

        while(obj.transform.localScale.y < targetScaleY)
        {
            scale.y += .1f;
            obj.transform.localScale = scale;
            yield return new WaitForSecondsRealtime(.02f);
        }
    }

    public void SetGrid(int x, int y)
    {
        gridIndex = new Vector2Int(x, y);
    }
}
