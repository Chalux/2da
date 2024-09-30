using Godot;

namespace da.Scripts.Interfaces
{
    public interface IInteractale
    {
        public virtual void Interact()
        {
            GD.Print($"Interacting with {ToString()}");
        }
    }
}
