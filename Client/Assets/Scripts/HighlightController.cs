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

    public GridSelector cellQuad;
    public GridSelector[] cells;
    public Mode mode = Mode.Rectangle;
    public GameObject building;
    public bool isCorrent;

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

    private int _targetCount = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _targetCount++;
            var target = (int)Mathf.Pow(_targetCount + (int)Input.mouseScrollDelta.y, 2);
            CellAmount = target;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _targetCount--;
            var target = (int)Mathf.Pow(_targetCount + (int)Input.mouseScrollDelta.y, 2);
            CellAmount = target;
        }
    }

    private void LateUpdate()
    {
        DetectCellBorder();

        if (Input.GetMouseButtonDown(0) && isCorrent)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Spawn(building);
            }
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
        cells = new GridSelector[CellAmount];
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
        isCorrent = true;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].transform.position.x < grid[0, 0].x || cells[i].transform.position.z < grid[0, 0].z || cells[i].transform.position.x > grid[gridSize.x - 1, 0].x || cells[i].transform.position.z > grid[0, gridSize.y - 1].z)
            {
                cells[i].SetStats(false);
                isCorrent = false;
                continue;
            }

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (cells[i].transform.position == grid[x, y])
                    {
                        cells[i].SetGrid(x, y);
                        break;
                    }
                }
            }

            if (cells[i].IsBuilded)
            {
                cells[i].SetStats(false);
                isCorrent = false;
            }
            else
            {
                cells[i].SetStats(true);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (mode == Mode.Rectangle)
        {
            transform.position = GameManager.instance.GetGridPosition(position);
            return;
        }
    }
}
