using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FroguesFramework
{
    public abstract class Weapon : CostsActionPointsBehaviour
    {
        [Space] [SerializeField] protected List<BaseCellsEffect> cellEffects;
        [SerializeField] protected PlayAnimation usingAnimation;
        [Header("Target For AI Only")] public UnitContainer expectedTargetContainer;
        public UnityEvent OnUse;

        private void Awake()
        {
            usingAnimation?.OnAnimationPlayed.AddListener(ApplyCellEffects);
        }

        public abstract void Use();
        public abstract void ApplyCellEffects();
        public abstract void HighlightCells();

        public abstract bool PossibleToHitExpectedTarget { get; }
        public abstract bool PossibleToUse { get; }
    }
}