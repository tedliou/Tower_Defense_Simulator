using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    [Header("Stats")]
    public bool isAvailable = true;
    public CellIndex firstSelectGrid = new (-1, -1);
    public CellIndex lastSelectGrid = new(-1, -1);
    public List<Cell> selectedCells = new ();

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
        firstSelectGrid = CellIndex.NegativeOne;
        lastSelectGrid = CellIndex.NegativeOne;
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
            firstSelectGrid = cell.index;
        }

        if (mouseUp || mouseStay)
        {
            lastSelectGrid = cell.index;
        }

        if (firstSelectGrid != CellIndex.NegativeOne)
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
                        var c = GridManager.instance.GetCellByIndex(new CellIndex(i, j));
                        if (GridManager.instance.deleteMode && Application.isEditor)
                        {
                            c.SetStats(CellState.Block);
                        }
                        else
                        {
                            c.SetStats(CellState.Select);
                        }
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
            if (GridManager.instance.deleteMode && Application.isEditor)
            {
                cell.SetStats(CellState.Block);
            }
            else
            {
                cell.SetStats(CellState.Select);
            }
        }

        // Complete
        if (mouseUp)
        {
            firstSelectGrid = CellIndex.NegativeOne;
            lastSelectGrid = CellIndex.NegativeOne;

            if (GridManager.instance.deleteMode && Application.isEditor)
            {
                foreach (var s in selectedCells)
                {
                    if (!s.CanBuild)
                        s.DestroyBuilding();
                }
            }
            else
            {
                var canBuild = true;
                foreach (var s in selectedCells)
                {
                    if (!s.CanBuild)
                    {
                        canBuild = false;
                        break;
                    }
                }

                if (canBuild)
                {
                    foreach (var c in selectedCells)
                    {
                        c.Build();
                    }
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
