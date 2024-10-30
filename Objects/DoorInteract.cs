using da.Scripts.Objects;

namespace da.Objects
{
    public partial class DoorInteract : InteractableArea
    {
        public override void Interact()
        {
            if (Owner is Door)
            {
                (Owner as Door).Open();
            }
        }
    }
}
