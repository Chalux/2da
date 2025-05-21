using da.Scripts.Interfaces;
using da.Scripts.Objects;
using da.Scripts;
using Godot;

public partial class SpikeTrap : Node2D, IAttackable
{
    [Export] HitBox HitBox;

    public HitBox GetHitBox()
    {
        return HitBox;
    }

    public Damage DoAttack(IAttackable attacker, IHurtable target)
    {
        return new()
        {
            value = 1,
            source = GetHitBox(),
            target = target.GetHurtBox(),
        };
    }
}
