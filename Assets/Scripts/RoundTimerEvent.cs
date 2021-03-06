using UnityEngine;
using UnityEngine.Events;

namespace FroguesFramework
{
    public class RoundTimerEvent : MonoBehaviour
    {
        [SerializeField] private int roundsToEvent;
        public UnityEvent timerEvent;
        private bool timerEnabled;

        public void StartTimer(int roundsToEvent)
        {
            this.roundsToEvent = roundsToEvent;
            timerEnabled = true;
        }

        public void StartTimer(IntContainer roundsToEventContainer) => StartTimer(roundsToEventContainer.Content);

        public void DecreaseTimer() => DecreaseTimer(1);

        public void DecreaseTimer(int decreaseValue)
        {
            if (!timerEnabled)
                return;

            roundsToEvent -= decreaseValue;

            if (roundsToEvent <= 0)
            {
                timerEnabled = false;
                timerEvent.Invoke();
                //roundsToEvent = 0;
            }
        }
    }
}
