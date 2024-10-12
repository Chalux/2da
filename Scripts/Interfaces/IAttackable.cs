using da.Scripts.Objects;

namespace da.Scripts.Interfaces
{
    public interface IAttackable
    {
        public abstract HitBox GetHitBox();
        public abstract Damage DoAttack(IAttackable attacker, IHurtable target);
    }
}
