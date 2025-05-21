using DialogicRuntime;
using Godot;

namespace da.Scripts.Objects.Others
{
    public partial class DialogNpc : InteractableArea
    {
        [Export] string dialogName = "";
        public override void Interact()
        {
            Dialogic.Start(dialogName);
        }
    }
}
