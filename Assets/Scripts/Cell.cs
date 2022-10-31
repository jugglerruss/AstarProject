using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

 [ExecuteAlways]
public class Cell : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textLength;
    [SerializeField] private TextMeshPro _textVes1;
    [SerializeField] private TextMeshPro _textManhatten;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private bool _isWall; 
    [SerializeField] private bool _isStart; 
    [SerializeField] private bool _isFinish; 
    [SerializeField] private float _weight;
    public Action<Cell> OnClick;
    private RectTransform _rectTransform;
    private Collider2D _collider2D;
    private Cell _targetCell;
    private int _length;
    private bool _isActiveEditor;
    public Action<Cell> OnSetPath;
    public Action OnMakePath;
    public bool IsFinish => _isFinish;
    public int Length
    {
        get => _length;
        set
        {
            _length = value;
           _textLength.text = _length.ToString();
          // _textLength.enabled = true;
        }
    }

    private int _manhattenIndex;
    public int ManhattenIndex
    {
        get => _manhattenIndex;
        set
        {
            _manhattenIndex = value * 10;
          _textManhatten.text = _manhattenIndex.ToString();
          // _textManhatten.enabled = true;
        }
    }
    public int RectX;

    public int RectY;
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        RectX = (int)_rectTransform.anchoredPosition.x;
        RectY = (int)_rectTransform.anchoredPosition.y;
        _collider2D = GetComponent<Collider2D>();
        ActivateColor();
    }
#if UNITY_EDITOR    
    public void OnGUI()
    {
        if(_isActiveEditor) ActivateColor();
    }
#endif
    private void OnMouseDown()
    {
        if(_isWall) return;
        OnClick.Invoke(this);
    }
    private void ActivateColor()
    {
        if (_isStart)
        {
            _sr.enabled = true;
            SetColor(Color.yellow);
        }
        else if (_isFinish)
        {
            _sr.enabled = true;
            SetColor(Color.red);
        }
        else if (_isWall)
        {
            _sr.enabled = true;
            gameObject.layer = 6;
            SetColor(Color.grey);
        }
    }
    public void SetVoid()
    {
        if(_isWall) return;
        Length = 0;
        ManhattenIndex = 0;
        _isStart = false;
        _isFinish = false;
        _sr.enabled = false;
        _textLength.enabled = false;
        _textManhatten.enabled = false;
        _targetCell = null;
    }
    public void SetStart()
    {
        _isStart = true;
        SetColor(Color.yellow);
    }
    public void SetFinish()
    {
        _isFinish = true;
        SetColor(Color.red);
    }
    public void SetOpen()
    {
        SetColor(Color.cyan);
    }
    private void SetColor(Color color)
    {
        _sr.enabled = true;
        _sr.color = color;
    }
    public void SetActiveEditor(bool active)
    {
        if(!active && _isWall)return;
        _sr.enabled = active;
        _isActiveEditor = active;
    }
    public bool IsWall()
    {
        return _isWall;
    }
    public void SetTarget(Cell cell)
    {
        _targetCell = cell;
    }
    public void SetSelected()
    {
        SetColor(Color.blue);
    }
    public void SetPath()
    {
        //SetColor(Color.green);
        OnSetPath.Invoke(this);
        if(_targetCell!=null) 
            _targetCell.SetPath();
        else
            OnMakePath.Invoke();
    }

}
