using UnityEngine;

public class Grid<T>
{
    private int rows, cols;
    private float cellSize;
    private Vector3 originPosition;
    private T[,] gridArray;

    public Grid(int rows, int cols, float cellSize, Vector3 originPosition)
    {
        this.rows = rows;
        this.cols = cols;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new T[cols, rows];
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosition(int y, int x)
    {
        return new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2) + originPosition;
    }

    public void SetGridValue(int y, int x, T value)
    {
        if (x >= 0 && y >= 0 && x < cols && y < rows)
        {
            gridArray[x, y] = value;
        }
    }

    public T GetGridValue(int y, int x)
    {
        if (x >= 0 && y >= 0 && x < cols && y < rows)
        {
            return gridArray[x, y];
        }
        return default(T);
    }


    public T GetGridValue(Vector3 worldPosition)
    {
        Vector2Int gridPos = GetGridPosition(worldPosition);
        return GetGridValue(gridPos.x, gridPos.y);
    }

    public int Rows => rows;
    public int Cols => cols;
    public float CellSize => cellSize;
    public T[,] GridArray => gridArray;
}