using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FroguesFramework
{
    public class PlayerInput : BaseInput
    {
        [SerializeField] private VisualizeSelectedCell selectedCellVisualizer;
        [SerializeField] private CellByMousePosition cellByMousePosition;
        [SerializeField] private UnitsUIEnabler unitsUIEnabler;
        [SerializeField] private IntContainer preCostContainer;
        [SerializeField] private SpriteRotator spriteRotator;

        [Header("Movement Input")] [SerializeField]
        private HighlightValidForMovementCells movementCellsHighlighter;

        [SerializeField] private FindWayInValidCells findWayInValidCells;
        [SerializeField] private VisualizePath pathVisualizer;

        [Header("Abilities")] [SerializeField] private Weapon inspectAbility;
        [SerializeField] private Weapon nativeAbility;
        public Weapon currentAbility;

        [Header("Cursor Icons")] [SerializeField]
        private Sprite defaultCursor;

        [SerializeField] private Sprite attackCursor;
        [SerializeField] private Sprite attackIsNotPossibleCursor;
        [SerializeField] private Sprite inspectCursor;

        private List<Cell> _path = new();
        private float bottomUiPanelHeight = 120f;
        private ActionPointsIconsShaker _actionPointsIconsShaker;
        private PauseIsActiveContainer _pauseIsActiveContainer;

        private void Awake()
        {
            _pauseIsActiveContainer = FindObjectOfType<PauseIsActiveContainer>();
            _actionPointsIconsShaker = FindObjectOfType<ActionPointsIconsShaker>();
        }

        public bool InputIsPossible => UnitsQueue.Instance.IsUnitCurrent(unit)
                                       && !CurrentlyActiveObjects.SomethingIsActNow
                                       && !_pauseIsActiveContainer.Content;

        public override void Act(){}

        private void Update()
        {
            Cursor.SetCursor(defaultCursor.texture, Vector2.zero, CursorMode.ForceSoftware);
            DisableAllVisualizationFromPlayerOnMap();
            unitsUIEnabler.AllUnitsUISetActive(false);

            if (!InputIsPossible)
                return;

            if (_path.Count != 0)
            {
                unit.movable.Move(_path[0]);
                _path.RemoveAt(0);
                return;
            }

            selectedCellVisualizer.ApplyEffect();

            ResetPreCostContainers();

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (currentAbility != null)
                {
                    currentAbility = null;
                }
                else
                {
                    currentAbility = inspectAbility;
                }
            }

            if (currentAbility != null)
            {
                AbilityInput(currentAbility);
            }
            else
            {
                MovementInput();
            }

            unitsUIEnabler.AllUnitsUISetActive(true);
        }

        public void DisableAllVisualizationFromPlayerOnMap()
        {
            Map.Instance.allCells.ForEach(cell => cell.DisableAllCellVisualization());
            var cellsWithContent = Map.Instance.allCells.WithContentOnly();
            cellsWithContent.Where(cell => cell.Content.health != null).ToList()
                .ForEach(cell => cell.Content.health.ResetPreDamageValue());
            cellsWithContent.Where(cell => cell.Content.pushable != null).ToList()
                .ForEach(cell => cell.Content.pushable.ResetPrePushValue());
        }

        private void ResetPreCostContainers()
        {
            preCostContainer.Content = 0;
        }

        private void MovementInput()
        {
            pathVisualizer.ApplyEffect();
            movementCellsHighlighter.ApplyEffect();

            if (findWayInValidCells.Take() != null)
                preCostContainer.Content = findWayInValidCells.Take().Count - 1;
            else
                preCostContainer.Content = 0;

            if (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.y > bottomUiPanelHeight)
            {
                _path = findWayInValidCells.Take() == null ? new List<Cell>() : findWayInValidCells.Take();
                preCostContainer.Content = 0;
            }
        }

        private void AbilityInput(Weapon ability)
        {
            spriteRotator.TurnByMousePosition();
            ability.HighlightCells();
            preCostContainer.Content = ability.CurrentActionCost;

            Sprite currentCursor = ability.PossibleToUse ? attackCursor : attackIsNotPossibleCursor;
            if (ability == inspectAbility)
                currentCursor = inspectCursor;


            Cursor.SetCursor(
                currentCursor.texture,
                Vector2.zero,
                CursorMode.ForceSoftware);

            if (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.y > bottomUiPanelHeight)
            {
                if (!ability.IsActionPointsEnough())
                    _actionPointsIconsShaker.Shake();

                ability.Use();
            }
        }

        public void SetCurrentAbility(Ability ability) => currentAbility = ability;

        public void SetCurrentAbilityNull() => currentAbility = null;
    }
}
