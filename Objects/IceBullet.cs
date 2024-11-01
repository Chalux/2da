using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using Godot;
using System;

public partial class IceBullet : Control, IAttackable
{
    [Export] AnimationPlayer AnimPlayer;
    [Export] HitBox HitBox;
    [Export] Sprite2D sprite;

    private Tween tween;

    IAttackable User;
    public int Direction = 1;

    public const float BULLET_SPEED = 400f;
    public void Inited()
    {
        AnimPlayer.Play("create");
        HitBox.BodyEntered += BulletCollision;
    }

    public Damage DoAttack(IAttackable attacker, IHurtable target)
    {
        if (User == null)
        {
            throw new Exception("IceBullet's User is null");
        }
        tween?.Stop();
        AnimPlayer.Play("disappear");
        Damage d = User.DoAttack(attacker, target);
        d.onHitSound = ResourceLoader.Load<AudioStream>("res://Resources/SFX/26_sword_hit_3.wav");
        d.stunPower = 0.01f;
        d.stunDuration = 0.5f;
        return d;
    }

    public HitBox GetHitBox() => HitBox;

    public void Init(IAttackable user, Vector2 GlobalPosition, int Direction = 1)
    {
        User = user;
        this.GlobalPosition = new(GlobalPosition.X + (Direction == 1 ? 0 : -Size.X), GlobalPosition.Y);
        this.Direction = Direction;
        sprite.Scale = new(sprite.Scale.X * Direction, sprite.Scale.Y);
        Inited();
    }

    private void BulletCollision(Node body)
    {
        if (body is TileMapLayer tml)
        {
            tween?.Stop();
            AnimPlayer.Play("disappear");
        }
    }

    public void DoTween()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(Position.X + BULLET_SPEED * 0.5f * Direction, Position.Y), 0.5f);
        tween.TweenCallback(Callable.From(() => AnimPlayer.Play("disappear")));
    }
}
