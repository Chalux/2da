using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    internal class PlayerAttackState : BaseState
    {
        public bool repel = false;
        public Vector2 RepelVelocity = Vector2.Zero;
        public int DamageValue = 1;
    }
}
