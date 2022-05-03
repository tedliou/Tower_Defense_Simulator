using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public int cellAmount = 1;
    public Vector2 cellSize = new Vector2(2, 2);
    public CellController cellQuad;
    public CellController[] cells;
    public Mode mode = Mode.Rectangle;

    public enum Mode
    {
        Line = 0,
        Rectangle = 1
    }

    private void Start()
    {
        GenerateCell();
    }

    private void Update()
    {
        DetectCellBorder();
    }

    public void GenerateCell()
    {
        if (cells != null)
            for (int i = 0; i < cells.Length; i++)
                if (cells[i] != null)
                    Destroy(cells[i]);

        cells = new CellController[cellAmount];
        if (mode == Mode.Line)
        {
            for (int i = 0; i < cellAmount; i++)
            {
                cellQuad.transform.position = new Vector3(cellSize.x * i, 0, 0);
                cells[i] = Instantiate(cellQuad, transform);
            }
        }
        else if (mode == Mode.Rectangle)
        {
            var count = Mathf.RoundToInt(Mathf.Pow(cellAmount, .5f));
            var position = new Vector3(0, 0, 0);
            float xDelta = 0;
            for (int i = 0; i < cellAmount; i++)
            {
                if (i > 0 && i % count == 0)
                {
                    position.z += cellSize.y;
                    xDelta = i * cellSize.x;
                }
                position.x = i * cellSize.x - xDelta;
                cellQuad.transform.position = position;
                cells[i] = Instantiate(cellQuad, transform);
            }
        }
    }

    private void DetectCellBorder()
    {
        var grid = GameManager.instance.grid;
        var gridSize = GameManager.instance.gridSize;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].transform.position.x < grid[0, 0].x || cells[i].transform.position.z < grid[0, 0].z)
            {
                cells[i].SetMode(false);
                continue;
            }

            if (cells[i].transform.position.x > grid[gridSize.x - 1, 0].x || cells[i].transform.position.z > grid[0, gridSize.y - 1].z)
            {
                cells[i].SetMode(false);
                continue;
            }

            cells[i].SetMode(true);
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (mode == Mode.Rectangle)
        {
            // 矯正滑鼠為多邊形中心位置
            if (cellAmount % 2 == 0 && cellAmount != 8)
            {
                var middle = (cells[cells.Length - 1].transform.localPosition - cells[0].transform.localPosition) / 2;
                transform.position = GameManager.instance.GetGridPosition(position - middle);
            }
            else if (cellAmount > 1)
            {
                var middle = cells[Mathf.RoundToInt(cells.Length / 2f)].transform.localPosition;
                transform.position = GameManager.instance.GetGridPosition(position - middle);
            }
            else
            {
                transform.position = GameManager.instance.GetGridPosition(position);
            }
        }
    }
}
