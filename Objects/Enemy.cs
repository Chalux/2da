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
    [Export] public TextureProgressBar healBar;
    [Export] public Timer HealthBarTimer;
    public bool ShowHealthBar = true;
    private Tween tween;

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

    public override void _Ready()
    {
        HealthBarTimer.Timeout += OnHealBarTimerout;
        healBar.MinValue = 0;
        healBar.MaxValue = status.MaxHealth;
        healBar.Value = status.Health;
        status.OnHealthChanged += EnemyHealthChanged;
        status.OnMaxHealthChanged += EnemyMaxHealthChanged;
        AddToGroup("enemies");
    }

    private void OnHealBarTimerout()
    {
        tween = CreateTween();
        tween.TweenProperty(healBar, "modulate", new Color(healBar.Modulate, 0), 1f);
    }

    private void EnemyHealthChanged(double newhealth, double oldhealth)
    {
        if (ShowHealthBar)
        {
            healBar.Value = newhealth;
            if (tween != null && tween.IsRunning())
            {
                tween.Stop();
            }
            healBar.Modulate = new(healBar.Modulate, 1);
            HealthBarTimer.Start();
        }
    }

    private void EnemyMaxHealthChanged(double maxhealth)
    {
        healBar.MaxValue = maxhealth;
    }
}
