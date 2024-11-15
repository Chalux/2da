using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using da.Scripts.Objects.EnemyScript;
using da.Scripts.Objects.Others;
using Godot;

public partial class Enemy : Role, IAttackable, IHurtable
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
    [Export] public GpuParticles2D HitParticles;
    [Export] public bool CantRevive = false;
    [Export] Node[] linkedObj;
    public bool ShowHealthBar = true;
    private Tween tween;
    private Tween hurtTween;
    private bool UnActive = false;

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
    [Export] public float Speed = 10;
    [Export] public float Acceleration = 20;
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
    {
        if (UnActive) return;

        base._PhysicsProcess(delta);

        StateMachine.PhysicsProcess(delta);

        MoveAndSlide();

        StateMachine.AfterMove(delta);
    }

    public override void _Process(double delta)
    {
        if (GameGlobal.Instance.player == null || GlobalPosition.DistanceSquaredTo(GameGlobal.Instance.player.GlobalPosition) > 600)
        {
            UnActive = false;
        }
        if (UnActive) return;
        StateMachine.Update(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (UnActive) return;

        StateMachine.UnhandledInput(@event);
    }

    public override void _Ready()
    {
        base._Ready();
        HealthBarTimer.Timeout += OnHealBarTimerout;
        healBar.MinValue = 0;
        healBar.MaxValue = status.MaxHealth;
        healBar.Value = status.Health;
        status.OnHealthChanged += EnemyHealthChanged;
        status.OnMaxHealthChanged += EnemyMaxHealthChanged;
        AddToGroup("enemies");
        HurtBox.onHurt += OnHurt;
        if (linkedObj != null && linkedObj.Length > 0)
        {
            status.OnDeath += OnDeathFunc;
        }
    }

    public void OnDeathFunc()
    {
        if (linkedObj != null)
        {
            foreach (Node node in linkedObj)
            {
                if (node is Oprated op)
                {
                    op.OnOpen();
                }
            }
        }
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

    private void OnHurt(Damage damage)
    {
        HitParticles.Restart();
        HitParticles.Emitting = true;
        //Engine.TimeScale = damage.stunPower;
        //var Timer = GetTree().CreateTimer(damage.stunDuration, true, false, true);
        //await Timer.ToSignal(Timer, SceneTreeTimer.SignalName.Timeout);
        //Engine.TimeScale = 1;
        hurtTween = GameGlobal.Instance.player?.CreateTween();
        if (hurtTween != null)
        {
            hurtTween.SetParallel();
            hurtTween.TweenMethod(Callable.From((float newvalue) => SetShaderBlinkIntensity(newvalue)), 1.0f, 0f, 0.5f);
            hurtTween.TweenMethod(Callable.From((float scale) => SetTimeScale(scale)), damage.stunPower, 1f, damage.stunDuration);
        }
    }

    private void SetShaderBlinkIntensity(float newvalue)
    {
        (sprite.Material as ShaderMaterial).SetShaderParameter("blink_intensity", newvalue);
    }

    private void SetTimeScale(float scale)
    {
        Engine.TimeScale = scale;
        hurtTween.SetSpeedScale(1 / (float)Engine.TimeScale);
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
            source = HitBox,
            target = target.GetHurtBox()
        };
    }

    public HurtBox GetHurtBox()
    {
        return HurtBox;
    }
}
