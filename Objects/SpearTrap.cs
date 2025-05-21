using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using Godot;

public partial class SpearTrap : Node2D, IAttackable
{
    [Export] Timer delay;
    [Export] public float delayTime = 0f;
    [Export] AnimationPlayer animationPlayer;
    [Export] HitBox HitBox;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (delayTime > 0)
        {
            delay.WaitTime = delayTime;
            delay.Start();
            delay.Timeout += () =>
            {
                animationPlayer.Play("loop");
            };
        }
        else
        {
            animationPlayer.Play("loop");
        }
    }

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
