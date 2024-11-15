using da.Objects;
using da.Scripts.Objects;
using da.Scripts.Objects.PlayerScript;
using Godot;

namespace da.Scripts.Skills
{
    internal class IceBulletSkill : Skill
    {
        private Player _owner;
        public IceBulletSkill()
        {
            Cooldown = 3;
            IconPath = "res://Resources/Assets/skillicons/ice-spear.png";
        }
        public override void SkillUse()
        {
            if (_owner == null) return;
            if (_owner.SlashChecker.IsColliding()) return;
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Objects/IceBullet.tscn");
            if (res == null) return;
            IceBullet IceBullet = res.Instantiate<IceBullet>();
            var CurrMap = _owner.FindParent("*Map*");
            CurrMap.AddChild(IceBullet);
            IceBullet.Init(_owner, _owner.SlashMarker.GlobalPosition, _owner.Direction);
        }
        public override bool SkillUseFilter()
        {
            var player = GameGlobal.Instance.player;
            if (player == null) return false;
            return player.StateMachine.currState.State switch
            {
                PlayerState.Walk => true,
                PlayerState.Idle => true,
                PlayerState.Crouch => true,
                _ => false,
            };
        }
        public override void PhysicsProcess(double delta, Player owner)
        {
            PlayerStaticFunc.MoveOnlyGravity(owner, delta, owner.gravity);
        }
        public override void Enter(Player owner)
        {
            _owner = owner;
            if (!owner.SkillList.ContainsKey("IceBullet"))
            {
                ChangeToIdle("skill");
                return;
            }
            owner.Velocity = Vector2.Zero;
            owner.CharactorAnimPlayer.Play("skill");
            try
            {
                owner.CharactorAnimPlayer.AnimationFinished += ChangeToIdle;
            }
            catch
            {

            }
            owner.SkillList["IceBullet"].CurrentCooldown = owner.SkillList["IceBullet"].Cooldown;
        }

        private void ChangeToIdle(StringName animName)
        {
            if (animName == "skill")
                _owner.StateMachine.ChangeState(PlayerState.Idle);
        }

        public override void Exit(Player owner)
        {
            try { owner.CharactorAnimPlayer.AnimationFinished -= ChangeToIdle; } catch { }
            owner.currSkill = null;
        }
    }
}
