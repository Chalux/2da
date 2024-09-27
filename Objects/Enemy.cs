using da.Scripts;
using da.Scripts.Objects;
using da.Scripts.Objects.EnemyScript;
using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public Node2D Graphics;
    [Export] public AnimationPlayer AnimPlayer;
    [Export] public EnemyStateMachine StateMachine;
    [Export] public HurtBox HurtBox;
    [Export] public HitBox HitBox;
    [Export] public Status status;
    [Export] public Sprite2D sprite;

    public int mirror = 1;
    private int _direction = 1;
    public int Direction
    {
        get => _direction; set
        {
            _direction = value;
            Graphics.Scale = new(value < 0 ? -1 * mirror : mirror, 1);
            //AttackCollision.Scale = new Vector2(value > 0 ? 1 : -1, 1);
        }
    }
    [Export] public float Speed = 180;
    [Export] public float Acceleration = 2000;
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
    {
        StateMachine.PhysicsProcess(delta);

        MoveAndSlide();

        StateMachine.AfterMove(delta);
    }

    public override void _Process(double delta)
    {
        StateMachine.Update(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        StateMachine.UnhandledInput(@event);
    }
}
