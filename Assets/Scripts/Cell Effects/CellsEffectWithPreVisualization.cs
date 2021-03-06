using System.Collections.Generic;

namespace FroguesFramework
{
    public abstract class CellsEffectWithPreVisualization : BaseCellsEffect
    {
        public abstract void PreVisualizeEffect();
        public abstract void PreVisualizeEffect(List<Cell> cells);
    }
}