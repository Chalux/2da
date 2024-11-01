using da.Scripts.Interfaces;
using Godot;

namespace da.Scripts.Objects
{
    public partial class HitBox : Area2D
    {
        [Signal]
        public delegate void onHitEventHandler(Damage damage);

        public override void _Ready()
        {
            AreaEntered += OnAreaEntered;
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area.Owner is IHurtable)
            {
                //GD.Print($"[Hit] {Owner.Name} hit {area.Owner.Name}");
                Damage damage;
                if (Owner is IAttackable attackable)
                {
                    damage = attackable.DoAttack(attackable, area.Owner as IHurtable);
                }
                else
                {
                    damage = new()
                    {
                        value = 1,
                        source = this,
                        target = area as HurtBox
                    };
                }
                if (damage.onHitSound != null)
                {
                    SoundManager.PlaySFXByStream(damage.onHitSound);
                }
                EmitSignal(SignalName.onHit, damage);
                (area as HurtBox).EmitSignal(HurtBox.SignalName.onHurt, damage);
            }
        }
    }
}
