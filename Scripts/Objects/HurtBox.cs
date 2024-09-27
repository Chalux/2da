using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    public partial class HurtBox : Area2D
    {
        [Signal]
        public delegate void onHurtEventHandler(Damage damage);

        //public override void _Ready()
        //{
        //    AreaEntered += OnAreaEntered;
        //}

        //private void OnAreaEntered(Area2D area)
        //{
        //    EmitSignal(SignalName.onHurt, area as HitBox);
        //    (area as HitBox).EmitSignal(HitBox.SignalName.onHit, this);
        //}
    }
}
