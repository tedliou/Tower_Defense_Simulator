using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Rectangle
{
    public int width;
    public int height;

    public Rectangle(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(width, 0, height);
    }
}

public class HighlightController : MonoBehaviour
{
    public int CellAmount {
        get {
            return _cellAmount;
        }
        set {
            _cellAmount = value;
            GenerateCell();
        }
    }
    private int _cellAmount;

    public Rectangle CellSize
    {
        get
        {
            return _cellSize;
        }
        set {
            _cellSize = value;
        }
    }
    private Rectangle _cellSize;

    public int Amount => cells.Length;

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
        CellSize = new Rectangle(2, 2);
        CellAmount = 1;
    }

    private void Update()
    {
        DetectCellBorder();

        if ((int)Input.mouseScrollDelta.y != 0)
        {
            CellAmount += (int)Input.mouseScrollDelta.y;
        }
    }

    public void GenerateCell()
    {
        // Clear
        if (cells != null)
            for (int i = 0; i < cells.Length; i++)
                if (cells[i] != null)
                    Destroy(cells[i].gameObject);

        // Generate
        cells = new CellController[CellAmount];
        if (mode == Mode.Line)
        {
            for (int i = 0; i < CellAmount; i++)
            {
                cellQuad.transform.position = new Vector3(CellSize.width * i, 0, 0);
                cells[i] = Instantiate(cellQuad, transform);
            }
        }
        else if (mode == Mode.Rectangle)
        {
            var pow = Mathf.Pow(Amount, .5f);
            var powInt = Mathf.CeilToInt(pow);
            var targetAmount = (int)Mathf.Pow(powInt, 2);
            var position = new Vector3(0, 0, 0);
            var xOffset = 0f;
            var amount = 0;
            for (int i = 0; i < _cellAmount; i++)
            {
                amount++;
                if (amount > powInt)
                {
                    position.z += CellSize.height;
                    xOffset = i * CellSize.width;
                    amount = 1;
                }

                position.x = i * CellSize.width - xOffset;
                cellQuad.transform.position = position;
                cells[i] = Instantiate(cellQuad, transform);
            }
            // Debug.Log($"amount={Amount}, targetAmount={targetAmount}");

            // Offset
            var index = (targetAmount - 1) / 2;
            if (targetAmount % 2 == 0)
            {
                index -= Mathf.Max(powInt - 2 - (powInt / 2 - 2), 0);
            }
            if (Amount > 4) {
                var offset = (cells[index].transform.localPosition - cells[0].transform.localPosition);
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i].transform.position -= offset;
                }
            }
        }
    }

    private void DetectCellBorder()
    {
        var grid = GameManager.instance.grid;
        var gridSize = GameManager.instance.gridSize;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].transform.position.x < grid[0, 0].x || cells[i].transform.position.z < grid[0, 0].z || cells[i].transform.position.x > grid[gridSize.x - 1, 0].x || cells[i].transform.position.z > grid[0, gridSize.y - 1].z)
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
            //var middle = (cells[cells.Length - 1].transform.localPosition - cells[0].transform.localPosition) / 2;
            transform.position = GameManager.instance.GetGridPosition(position);
            return;

            // 矯正滑鼠為多邊形中心位置
            //if (cellAmount % 2 == 0 && cellAmount != 8)
            //{
            //    var middle = (cells[cells.Length - 1].transform.localPosition - cells[0].transform.localPosition) / 2;
            //    transform.position = GameManager.instance.GetGridPosition(position - middle);
            //}
            //else if (cellAmount > 1)
            //{
            //    var middle = cells[Mathf.RoundToInt(cells.Length / 2f)].transform.localPosition;
            //    transform.position = GameManager.instance.GetGridPosition(position - middle);
            //}
            //else
            //{
            //    transform.position = GameManager.instance.GetGridPosition(position);
            //}
        }
    }
}
