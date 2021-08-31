using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTilesByCells : MonoBehaviour
{
    [SerializeField] private BaseCellsTaker _cellTaker;

    public void Highlight()
    {
        _cellTaker.Take().ForEach(cell => { cell.EnableHighlight(true); cell.EnablePathDot(true);});
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Highlight();
    }
}
