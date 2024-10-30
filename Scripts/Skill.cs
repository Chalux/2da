using da.Objects;
using System;

namespace da.Scripts
{
    public class Skill
    {
        public float _Cooldown = 0.5f;
        public float Cooldown
        {
            get { return _Cooldown; }
            set { _Cooldown = Math.Max(value, 0.5f); }
        }
        public float CurrentCooldown;
        public string IconPath;
        public virtual void SkillUse()
        {

        }
        public virtual bool SkillUseFilter()
        {
            return true;
        }
        public bool IsReady = false;
        public void CheckIsReady()
        {
            IsReady = CurrentCooldown <= 0 && SkillUseFilter();
        }
        public virtual void Enter(Player owner)
        {

        }
        public virtual void PhysicsProcess(double delta, Player owner)
        {

        }
        public virtual void Exit(Player owner)
        {

        }
    }
}
