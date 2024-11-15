using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Buffs
{
    public partial class HealBuff : Buff
    {
        public float HealAmount { get; set; }
        private float interval = 1.5f;
        private float timer = 0;
        private GpuParticles2D healParticles;
        public HealBuff(Role owner, float timeout, int count, float healAmount) : base(owner, timeout, count)
        {
            HealAmount = healAmount;
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Resources/Particles/heal_particles.tscn");
            if (res != null)
            {
                healParticles = res.Instantiate<GpuParticles2D>();
                if (_owner is Player p)
                {
                    p.Sprite.AddChild(healParticles);
                }
                else if (_owner is Enemy e)
                {
                    e.sprite.AddChild(healParticles);
                }
            }
        }
        protected override void BeforeRemoved()
        {
            base.BeforeRemoved();
            if (healParticles != null)
            {
                if (_owner is Player p)
                {
                    p.Sprite.RemoveChild(healParticles);
                }
                else if (_owner is Enemy e)
                {
                    e.sprite.RemoveChild(healParticles);
                }
                healParticles.QueueFree();
            }
        }
        public override void PhysicsProcess(double delta, Role owner)
        {
            base.PhysicsProcess(delta, owner);
            Timeout -= (float)delta;
            timer += (float)delta;
            if (timer >= interval)
            {
                timer = 0;
                if (_owner is Player p)
                {
                    p.status.Health += HealAmount;
                }
                else if (_owner is Enemy e)
                {
                    e.status.Health += HealAmount;
                }
                healParticles.Restart();
                SoundManager.Ins.PlaySFX("Heal");
            }
        }
    }
}
