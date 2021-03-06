namespace FroguesFramework
{
    public class PlayerAbilityButtonSlot : AbilityButtonContainer
    {
        private void Awake()
        {
            if (Content == null)
                return;

            Content = content;
        }

        public override AbilityButton Content
        {
            set
            {
                if (!(content == null || value == null) && value.slot != null)
                    value.slot.Content = content;

                content = value;

                if (content == null)
                    return;

                content.slot = this;
                content.transform.parent = transform;
                content.transform.position = transform.position;
            }
        }

        public void HotkeyPressed()
        {
            content?.PickAbility();
        }
    }
}