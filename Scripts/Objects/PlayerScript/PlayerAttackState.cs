using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.PlayerScript
{
    internal class PlayerAttackState : BaseState
    {
        public bool repel;
        public Vector2 RepelVelocity;
        public int DamageValue;
    }
}
