using Godot;

namespace da.Scripts.Objects
{
    public partial class Teleporter : InteractableArea
    {
        [Export(PropertyHint.File, "*.tscn")] string path;
        [Export] string TelepositionName;
        [Export] int Direction = 1;
        override public void Interact()
        {
            base.Interact();
            GameGlobal.Instance.Teleport(path, TelepositionName, Direction);
        }
    }
}
