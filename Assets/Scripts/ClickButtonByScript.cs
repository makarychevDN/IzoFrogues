using UnityEngine;
using UnityEngine.UI;

namespace FroguesFramework
{
    public class ClickButtonByScript : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private bool ignoreButtonUnInteractive;

        public void Click()
        {
            if(button.interactable || (!button.interactable && ignoreButtonUnInteractive))
                button.onClick.Invoke();
        }
    }
}