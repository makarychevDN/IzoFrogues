using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cell : Container<Unit>
{
    public MapLayer _mapLayer;
    public Vector2Int _coordinates;

    public UnityEvent OnBecameFull;
    public UnityEvent OnBecameEmpty;

    public GameObject _tileHighlighter;
    public GameObject _pathDot;
    public GameObject _selectedVisualization;

    public override Unit Content 
    { 
        get => base.Content;
        set
        {
            base.Content = value;
            if(value != null)
            {
                value._currentCell = this;
                OnBecameFull.Invoke();
            }
            else
            {
                OnBecameEmpty.Invoke();
            }
        }
    }

    public void EnableHighlight(bool isOn)
    {
        _tileHighlighter.SetActive(isOn);
    }

    public void EnablePathDot(bool isOn)
    {
        _pathDot.SetActive(isOn);
    }
    
    public void EnableSelectedVisualization(bool isOn)
    {
        _selectedVisualization.SetActive(isOn);
    }
}
