using da.Objects;
using da.Scripts.Interfaces;
using Godot;

namespace da.Scripts.Objects
{
    public partial class InteractableArea : Area2D, IInteractale
    {
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;
            CollisionLayer = 0;
            CollisionMask = 0;
            SetCollisionMaskValue(2, true);
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body is Player player)
            {
                player.InteractableAnim.Visible = true;
                player.InteractableAnim.Play();
                if (player.NowInteraction.IndexOf(this) == -1) player.NowInteraction.Add(this);
            }
        }

        private void OnBodyExited(Node2D body)
        {
            if (body is Player player)
            {
                player.NowInteraction.Remove(this);
                if (player.NowInteraction.Count == 0)
                {
                    player.InteractableAnim.Visible = false;
                    player.InteractableAnim.Stop();
                }
            }
        }

        public virtual void Interact()
        {
            GD.Print($"Interacting with {Name}");
        }
    }
}
