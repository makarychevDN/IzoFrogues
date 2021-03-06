using System.Collections.Generic;
using UnityEngine;

namespace FroguesFramework
{
    public class VisualizePath : BaseCellsEffect
    {
        [SerializeField] private FindWayInValidCells pathFinderInValidCells;

        public override void ApplyEffect()
        {
            ApplyEffect(pathFinderInValidCells.Take());
        }

        public override void ApplyEffect(List<Cell> cells)
        {
            TurnOffVisualization();
            if (cells == null)
                return;

            cells.GetLast().EnablePathDot(true);

            if (cells.Count == 1)
                return;

            for (int i = 1; i < cells.Count - 1; i++)
            {
                cells[i].EnableTrail((cells[i - 1].transform.position - cells[i].transform.position).normalized.ToVector2());
                cells[i].EnableTrail((cells[i + 1].transform.position - cells[i].transform.position).normalized.ToVector2());
            }

            cells[0].EnableTrail((cells[1].transform.position - cells[0].transform.position).normalized.ToVector2());
            cells[cells.Count - 1].EnableTrail((cells[cells.Count - 2].transform.position - cells[cells.Count - 1].transform.position).normalized.ToVector2());
        }

        public void TurnOffVisualization()
        {
            Map.Instance.allCells.ForEach(cell =>
            {
                cell.DisableTrails();
                cell.EnablePathDot(false);
            });
        }
    }
}