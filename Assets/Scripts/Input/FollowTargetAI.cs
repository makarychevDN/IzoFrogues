using System.Collections.Generic;
using UnityEngine;

namespace FroguesFramework
{
    public class FollowTargetAI : BaseInput
    {
        [SerializeField] private UnitContainer targetContainer;
        [SerializeField] private AbleToSkipTurn skipTurnModule;
        [SerializeField] private bool ignoreDefaultUnits, ignoreProjectiles, ignoreSurfaces;

        private List<Cell> _pathToTarget;

        private void Start()
        {
            unit.GetComponentInChildren<ActionPoints>().OnActionPointsEnded.AddListener(ClearPath);
        }

        public override void Act()
        {
            if (_pathToTarget == null)
                _pathToTarget = PathFinder.Instance.FindWay(unit.currentCell, targetContainer.Content.currentCell,
                    ignoreDefaultUnits, ignoreProjectiles, ignoreSurfaces);

            if (_pathToTarget != null && _pathToTarget.Count > 1)
            {
                unit.movable.Move(_pathToTarget[0]);
                if (_pathToTarget != null) _pathToTarget.RemoveAt(0);
                return;
            }

            OnInputDone.Invoke();
            skipTurnModule.AutoSkip();
            ClearPath();
        }

        public void ClearPath()
        {
            _pathToTarget = null;
        }
    }
}