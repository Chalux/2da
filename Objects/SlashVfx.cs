using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using Godot;
using System;

public partial class SlashVfx : Control, IAttackable
{
    [Export] AnimationPlayer AnimPlayer;
    [Export] HitBox HitBox;
    [Export] Timer recoverTimer;
    [Export] TextureRect sprite;

    private Tween tween;

    IAttackable User;
    public int Direction = 1;

    public const float SLASH_SPEED = 50f;
    public void Inited()
    {
        AnimPlayer.Play("anim");
        tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(Position.X + SLASH_SPEED * 0.5f * Direction, Position.Y), 0.5f);
        recoverTimer.Timeout += Timeout;
        HitBox.BodyEntered += SlashCollision;
    }

    private void Timeout()
    {
        //GD.Print("SlashVFX Timeout");
        QueueFree();
    }

    public Damage DoAttack(IAttackable attacker, IHurtable target)
    {
        if (User == null)
        {
            throw new Exception("SlashVFX's User is null");
        }
        Damage d = User.DoAttack(attacker, target);
        d.onHitSound = ResourceLoader.Load<AudioStream>("res://Resources/SFX/26_sword_hit_3.wav");
        return d;
    }

    public HitBox GetHitBox() => HitBox;

    public void Init(IAttackable user, Vector2 GlobalPosition, int Direction = 1)
    {
        User = user;
        this.GlobalPosition = GlobalPosition;
        this.Direction = Direction;
        sprite.Scale = new(sprite.Scale.X * Direction, sprite.Scale.Y);
        Inited();
    }

    private void SlashCollision(Node body)
    {
        if (body is TileMapLayer tml)
        {
            tween.Stop();
        }
    }
}
