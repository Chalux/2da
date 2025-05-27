using Godot;
using System;
using System.Collections.Generic;

namespace da.Scripts
{
    public partial class Status : Node
    {
        [Signal]
        public delegate void OnHealthChangedEventHandler(double newhealth, double oldhealth);

        public double _maxHealth;

        [Signal]
        public delegate void OnMaxHealthChangedEventHandler(double maxhealth);

        [Signal]
        public delegate void OnDeathEventHandler();

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
                    EmitSignal(SignalName.OnDeath);
                    EventMgr.DispatchEvent("EnemyDied", Owner);
                }
            }
        }

        private double _dashCoolDown = 15;

        public double DashCoolDown
        {
            get => _dashCoolDown;
            set => _dashCoolDown = value;
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
                { "health", Health },
                { "dashcool", DashCoolDown },
            };
        }

        public void FromDict(Godot.Collections.Dictionary<string, Variant> dict)
        {
            MaxHealth = dict.TryGetValue("maxhealth", out Variant value) ? value.AsDouble() : 0;
            Health = dict.TryGetValue("health", out Variant hvalue) ? hvalue.AsDouble() : 0;
            DashCoolDown = dict.TryGetValue("dashcool", out Variant dvalue) ? dvalue.AsDouble() : 15;
        }
    }
}