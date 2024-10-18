using Godot;
using System;

namespace da.Scripts
{
    public partial class Status : Node
    {
        [Signal]
        public delegate void OnHealthChangedEventHandler(double newhealth, double oldhealth);
        public double _maxHealth;
        [Signal]
        public delegate void OnMaxHealthChangedEventHandler(double maxhealth);
        public double MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                EmitSignal(SignalName.OnMaxHealthChanged, value);
            }
        }
        public double _health;
        public double Health
        {
            get => _health;
            set
            {
                var oldvalue = _health;
                _health = Math.Clamp(value, 0, MaxHealth);
                if (oldvalue != _health)
                {
                    EmitSignal(SignalName.OnHealthChanged, value, oldvalue);
                }
                if (Owner is Enemy && _health <= 0)
                {
                    EventMgr.DispatchEvent("EnemyDied", Owner);
                }
            }
        }

        public override void _Ready()
        {
            MaxHealth = 5;
            Health = MaxHealth;
        }

        public Godot.Collections.Dictionary<string, Variant> ToDict()
        {
            return new Godot.Collections.Dictionary<string, Variant>()
            {
                { "maxhealth", MaxHealth },
                { "health", Health }
            };
        }

        public void FromDict(Godot.Collections.Dictionary<string, Variant> dict)
        {
            MaxHealth = dict["maxhealth"].AsDouble();
            Health = dict["health"].AsDouble();
        }
    }
}
