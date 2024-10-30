using da.Objects;

namespace da.Scripts.Objects.PlayerScript
{
    internal class SkillState : BaseState
    {
        public SkillState()
        {
            State = PlayerState.Skill;
        }

        public override void Enter(Player owner)
        {
            owner.currSkill?.Enter(owner);
        }

        public override void Exit(Player owner)
        {
            owner.currSkill?.Exit(owner);
        }

        public override void PhysicsProcess(double delta, Player owner)
        {
            owner.currSkill?.PhysicsProcess(delta, owner);
        }
    }
}
