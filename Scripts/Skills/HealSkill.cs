using da.Objects;
using da.Scripts.Buffs;
using da.Scripts.Interfaces;
using da.Scripts.Objects.PlayerScript;
using Godot;

namespace da.Scripts.Skills
{
    public class HealSkill : Skill, INeedSetOwner
    {
        private Player _owner;
        public HealSkill()
        {
            Cooldown = 10;
            IconPath = "res://Resources/Assets/skillicons/healing.png";
        }
        public void SetOwner(Role owner)
        {
            if (owner is Player p)
                _owner = p;
        }
        public override void SkillUse()
        {
            base.SkillUse();
            if (_owner == null) return;
            _owner.status.Health += 1;
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Resources/Particles/heal_particles.tscn");
            if (res != null)
            {
                var healParticles = res.Instantiate<GpuParticles2D>();
                _owner.Sprite.AddChild(healParticles);
                healParticles?.Restart();
                healParticles.Finished += () =>
                {
                    _owner.Sprite.RemoveChild(healParticles);
                    healParticles.QueueFree();
                };
            }
            SoundManager.Ins.PlaySFX("Heal");
            _owner.AddBuff(new HealBuff(_owner, 6, -1, 1));
            CurrentCooldown = Cooldown;
        }
        public override bool SkillUseFilter()
        {
            return _owner != null && _owner.status.Health != _owner.status.MaxHealth;
        }
        public override void Enter(Player owner)
        {
            base.Enter(owner);
            _owner = owner;
            if (!owner.SkillList.ContainsKey("Heal"))
            {
                ChangeToIdle("skill");
                return;
            }
            owner.Velocity = Vector2.Zero;
            owner.CharactorAnimPlayer.Play("skill");
            owner.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
        }
        private void ChangeToIdle(StringName animName)
        {
            if (animName == "skill")
                _owner.StateMachine.ChangeState(PlayerState.Idle);
        }
        public override void Exit(Player owner)
        {
            owner.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle;
            owner.currSkill = null;
        }
    }
}
