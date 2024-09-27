using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //GD.Print($"[Hit] {Owner.Name} hit {area.Owner.Name}");
            Damage damage = new()
            {
                value = 1,
                source = this,
                target = area as HurtBox
            };
            EmitSignal(SignalName.onHit, damage);
            (area as HurtBox).EmitSignal(HurtBox.SignalName.onHurt, damage);
        }
    }
}
