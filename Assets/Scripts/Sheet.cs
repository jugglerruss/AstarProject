using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Sheet: MonoBehaviour
{
    [SerializeField] private bool _isEditorMode;
    [SerializeField] private bool _isTest;
    [SerializeField] private Player _player;
    private bool _editorMode;
    private Cell[,] _cellsArray;
    private List<Cell> _cells;
    private List<Cell> _closeCells;
    private List<Cell> _wallCells;
    private List<Cell> _openCells;
    private List<Cell> _pathCells;
    public Cell StartCell;
    public Cell FinishCell;
    public Cell SelectedCell;
    public Action<string> OnPrint;
    public Action<List<Cell>> OnMakePath;
#if UNITY_EDITOR
    public void OnGUI()
    {
        if (_isEditorMode)
        {
            if (_editorMode) return;
            _cells = FindObjectsOfType<Cell>().ToList();
            foreach (var cell in _cells)
            {
                cell.SetActiveEditor(true);
            }
            _editorMode = true;
        }
        else
        {
            if (!_editorMode) return;
            _cells = FindObjectsOfType<Cell>().ToList();
            foreach (var cell in _cells)
            {
                cell.SetActiveEditor(false);
            }
            _editorMode = false;
        }
    }
#endif
    void Start()
    {
        _closeCells = new List<Cell>();
        _openCells = new List<Cell>();
        _wallCells = new List<Cell>();
        _pathCells = new List<Cell>();
        _cellsArray = new Cell[101, 101];
        _cells = FindObjectsOfType<Cell>().ToList();
        foreach (var cell in _cells)
        {
            _cellsArray[cell.RectX, cell.RectY] = cell;
            cell.OnClick += OnCellClick;
            cell.OnSetPath += AddToPath;
            cell.OnMakePath += MakePath;
            if(cell.IsWall()) _wallCells.Add(cell);
        }
    }
    private void OnDestroy()
    {
        StopCoroutine(WaitForFinder());
    }
    private void OnCellClick(Cell cell)
    {
        StopCoroutine(WaitForFinder());
        if (StartCell != null && FinishCell != null)
        {
            RemoveStartCell();
            RemoveFinishCell();
        }
        if (FinishCell != null)
            return;
        SetStartCell(cell);
        cell.SetFinish();
        SetFinishCell(cell);
    }
    private void SetStartCell(Cell cell)
    {
        var pos = _player.transform.localPosition;
        StartCell = _cellsArray[(int)pos.x+50, (int)pos.y+50];
        StartCell.SetStart();
        _closeCells.AddRange(_wallCells);
        _closeCells.Add(StartCell);
        SelectedCell = StartCell;
    }
    private void RemoveStartCell()
    {
        StartCell = null;
        foreach (var cell in _closeCells)
            cell.SetVoid();
        foreach (var cell in _openCells)
            cell.SetVoid();
        _closeCells = new List<Cell>();
        _openCells = new List<Cell>();
        _pathCells = new List<Cell>();
    }
    private void SetFinishCell(Cell cell)
    {
        FinishCell = cell;
        MoveToEnd(); 
    }
    private void RemoveFinishCell()
    {
        FinishCell.SetVoid();
        FinishCell = null;
    }
    private void AddNearbyCellsToOpen(Cell cell)
    {
        int currentX = cell.RectX;
        int currentY = cell.RectY;
        var straightWays = new List<Cell>
        {
            GetOpenCell(currentX - 1, currentY), GetOpenCell(currentX + 1, currentY),
            GetOpenCell(currentX, currentY - 1), GetOpenCell(currentX, currentY + 1)
        };
        AddToOpenList(straightWays, 10);
        var diagonalWays = new List<Cell>()
        {
            GetOpenCell(currentX - 1, currentY - 1), GetOpenCell(currentX + 1, currentY + 1),
            GetOpenCell(currentX + 1, currentY - 1), GetOpenCell(currentX - 1, currentY + 1)
        };
        AddToOpenList(diagonalWays, 14);
    }
    private void AddToOpenList(List<Cell> list, int length)
    {
        var listAlreadyIn = new List<Cell>();
        foreach (var variCell in list.Where(variCell => variCell != null))
        {
            if (_openCells.Contains(variCell) && SelectedCell != null)
            {
                listAlreadyIn.Add(variCell);
                if( variCell.Length < SelectedCell.Length + length)
                    continue;
            }
            variCell.SetTarget(SelectedCell);
            variCell.Length = SelectedCell.Length + length;
            variCell.ManhattenIndex = Math.Abs(variCell.RectX - FinishCell.RectX) + Math.Abs(variCell.RectY - FinishCell.RectY);
        }
        foreach (var cell in list.Where( c=> c != null && !listAlreadyIn.Contains(c) ))
        {
            _openCells.Add(cell);
            if (_isTest) cell.SetOpen();
        }
    }
    private Cell GetOpenCell(int x, int y)
    {
        if (x > 0 && y > 0 && x < 101 && y < 101 )
            if (!_closeCells.Contains(_cellsArray[x, y]))
                return _cellsArray[x, y];
        return null;
    }
    public void MoveOneStepForward()
    {
        if(_openCells.Count == 0) return;
        var minLength = _openCells.Min(c => c.Length + c.ManhattenIndex);
        SelectedCell = _openCells.First(c => c.Length + c.ManhattenIndex == minLength);
        _closeCells.Add(SelectedCell);
        _openCells.Remove(SelectedCell);
        if(_isTest) SelectedCell.SetSelected();
        OnPrint.Invoke(SelectedCell.RectX+"|"+SelectedCell.RectY);
        AddNearbyCellsToOpen(SelectedCell);
    }
    public void MoveToEnd()
    {
        AddNearbyCellsToOpen(StartCell);
        if(_isTest)
            StartCoroutine(WaitForFinder());
        else
        {
            while ( !SelectedCell.IsFinish )
                MoveOneStepForward();
            
            SelectedCell.SetPath();
        }
    }
    private void AddToPath(Cell cell)
    {
        _pathCells.Add(cell);
    }
    private void MakePath()
    {
        OnMakePath.Invoke(_pathCells);
        StartCell.SetVoid();
        FinishCell.SetVoid();
    }
    private IEnumerator WaitForFinder()
    {
        Debug.Log("WaitForFinder Start");
        while ( !SelectedCell.IsFinish )
        {
            MoveOneStepForward();
            yield return new WaitForSeconds(0.01f);
        }
        SelectedCell.SetPath();
    }
}
