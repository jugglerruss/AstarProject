using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private Sheet _sheet;
    [SerializeField] private InputCtrl _input;
    [SerializeField] private Text _output;
    [SerializeField] private ArrowMeshGenerator _arrow;
    [SerializeField] private Player _player;
    private void Start()
    {
        _input.OnPressSpace += MakeOneStepForward;
        _input.OnPressF += MoveToEnd;
        _sheet.OnPrint += Print;
        _sheet.OnMakePath += MovePlayer;
    }
    private void MakeOneStepForward()
    {
        _sheet.MoveOneStepForward();
    }
    private void MoveToEnd()
    {
        _sheet.MoveToEnd();
    }
    private void Print(string str)
    {
        _output.text = str;
    }
    private void MakeArrow(List<Cell> cells)
    {
        cells.Reverse();
        _arrow.Init(cells);
    }
    private void MovePlayer(List<Cell> cells)
    {
        cells.Reverse();
        _player.Move(cells);
    }
}
