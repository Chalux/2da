using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts
{
    public partial class Status : Node
    {
        [Export] public int maxHealth = 3;
        public int _health;
        public int Health
        {
            get => _health;
            set
            {
                _health = Math.Clamp(value, 0, maxHealth);
            }
        }

        public override void _Ready()
        {
            Health = maxHealth;
        }
    }
}
