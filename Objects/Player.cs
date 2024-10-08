using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using da.Scripts.Objects.PlayerScript;
using Godot;
using System.Collections.Generic;

namespace da.Objects
{
    public partial class Player : CharacterBody2D, IEvent
    {
        public const int RIGHTPOSITION = 1;
        public const int LEFTPOSITION = -1;
        //[Export] public Sprite2D CharactorSprite;
        [Export] public Node2D Graphics;
        private Sprite2D CharactorSprite;
        [Export] public AnimationPlayer CharactorAnimPlayer;
        [Export] public CollisionShape2D Collision;
        [Export] public CollisionPolygon2D AttackCollision;
        [Export] PackedScene GhostNode;
        [Export] public Timer GhostTimer;
        [Export] public Timer DashTimer;
        [Export] public Timer DashCoolDownTimer;
        [Export] public Timer CoyoteTimer;
        [Export] public Timer JumpTimer;
        [Export] public Timer JumpRequestTimer;
        [Export] public Timer AttackRequestTimer;
        [Export] public Timer WhosYourDaddy;
        [Export] public GpuParticles2D DashParticles;
        [Export] public AnimationTree AnimTree;
        [Export] public RayCast2D HandRay;
        [Export] public RayCast2D FootRay;
        [Export] public HitBox HitBox;
        [Export] public HurtBox HurtBox;
        public const float Speed = 160;
        public const float JumpVelocity = -400;
        public const float FloorAcceleration = Speed / 0.2f;
        public const float AirAcceleration = Speed / 0.1f;
        public Vector2 WallJumpVelocity = new(240, JumpVelocity);
        [Export] public bool CanCancelAttack = false;
        [Export] public bool CanHurtMove = false;
        public int CanDashCount = 1;
        public int DashCount = 0;
        public int CanJumpCount = 2;
        public int JumpCount = 0;
        public float DashSpeed = 500f;
        private int _direction = 1;
        public bool HasWallJumped = false;
        [Export] public Status status;
        public int Direction
        {
            get => _direction; set
            {
                _direction = value;
                Graphics.Scale = new(value < 0 ? -1 : 1, 1);
                //AttackCollision.Scale = new Vector2(value > 0 ? 1 : -1, 1);
            }
        }
        public bool IsQuickDowned = false;
        public bool HasReleasedJumpKey = true;
        public bool HasReleasedCrouchKey = true;
        [Export] public PlayerStateMachine StateMachine;
        public BaseState NextAttackState;
        public List<IInteractale> NowInteraction = new();
        [Export] public AnimatedSprite2D InteractableAnim;

        // Get the gravity from the project settings to be synced with RigidBody nodes.
        public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        public override void _PhysicsProcess(double delta)
        {
            StateMachine.PhysicsProcess(delta);

            MoveAndSlide();

            StateMachine.AfterMove(delta);
        }

        private void AddGhost()
        {
            var ghost = GhostNode.Instantiate<GhostNode>();
            ghost.Texture = CharactorSprite.Texture;
            ghost.RegionEnabled = CharactorSprite.RegionEnabled;
            ghost.RegionRect = CharactorSprite.RegionRect;
            ghost.Hframes = CharactorSprite.Hframes;
            ghost.Vframes = CharactorSprite.Vframes;
            ghost.FrameCoords = CharactorSprite.FrameCoords;
            ghost.Frame = CharactorSprite.Frame;
            ghost.FlipH = Graphics.Scale.X == -1;
            ghost.FlipV = Graphics.Scale.Y == -1;
            ghost.SetProperty(Position + CharactorSprite.Position, CharactorSprite.Scale);
            GetTree().CurrentScene.AddChild(ghost);
        }

        public override void _Ready()
        {
            CharactorSprite = Graphics.GetChild<Sprite2D>(0);
            StateMachine.ChangeState(PlayerState.Idle);
            GhostTimer.Timeout += AddGhost;
            DashTimer.Timeout += EndDash;
            //DashCoolDownTimer.Timeout += () =>
            //{
            //    GD.Print("Dash is already cooldown");
            //};
            CoyoteTimer.Timeout += () =>
            {
                if (!IsOnFloor()) StateMachine.ChangeState(PlayerState.Fall);
            };
            HurtBox.onHurt += (Damage damage) =>
            {
                status.Health -= damage.value;
                if (status.Health > 0)
                {
                    StateMachine.ChangeState(PlayerState.Hurt);
                }
                else
                {
                    StateMachine.ChangeState(PlayerState.Death);
                }
                Direction = (damage.source.Owner as Enemy).Position.X >= Position.X ? 1 : -1;
            };
            WhosYourDaddy.Timeout += () =>
            {
                //HurtBox.Visible = true;
                HurtBox.SetDeferred("monitorable", true);
            };
            EventMgr.RegisterEvent(this);
            EventMgr.DispatchEvent("PlayerReady", this);
        }

        public bool CheckCanDash => (DashCount > 0 || StateMachine.currState.State == PlayerState.WallSliding) && DashTimer.IsStopped() && DashCoolDownTimer.IsStopped();
        public bool TryDash()
        {
            if (!CheckCanDash)
            {
                return false;
            }
            if (StateMachine.currState.State == PlayerState.Attack1)
            {
                StateMachine.ChangeState(PlayerState.Idle);
            }
            if (StateMachine.oldStateEnum != PlayerState.WallSliding) DashCount--;
            Dash();
            return true;
        }

        public void Dash()
        {
            DashTimer.Start();
            GhostTimer.Start();
            DashParticles.Emitting = true;
        }

        public bool CheckCanJump => JumpCount > 0 && JumpTimer.IsStopped();
        public void TryJump()
        {
            if (!CheckCanJump)
            {
                //GD.Print("Can't jump");
                return;
            }
            StateMachine.ChangeState(PlayerState.Jump);
        }

        public void EndDash()
        {
            GhostTimer.Stop();
            DashTimer.Stop();
            //GD.Print("DashTimer stop");
            if (DashCount > 0) DashCoolDownTimer.WaitTime = 0.2f;
            else DashCoolDownTimer.WaitTime = 1f;
            DashCoolDownTimer.Start();
            DashParticles.Emitting = false;
            if (StateMachine.currState is DashState ds)
            {
                Velocity = new(Velocity.X, ds.oldVelocityY);
            }
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (IsOnFloor())
            {
                if (direction.Y > 0.5) StateMachine.ChangeState(PlayerState.Crouch);
                else StateMachine.ChangeState(PlayerState.Idle);
            }
            else if (IsOnWall())
            {
                StateMachine.ChangeState(PlayerState.WallSliding);
                Velocity = new(0, Velocity.Y);
            }
            else
            {
                StateMachine.ChangeState(PlayerState.Fall);
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_accept"))
            {
                JumpRequestTimer.Start();
                if (IsOnFloor()) HasReleasedJumpKey = false;
            }
            if (@event.IsActionPressed("attack"))
            {
                AttackRequestTimer.Start();
            }
            if (@event.IsActionReleased("ui_accept"))
            {
                HasReleasedJumpKey = true;
                if (Velocity.Y < JumpVelocity / 2)
                {
                    Velocity = new(Velocity.X, JumpVelocity / 2);
                }
            }
            if (@event.IsActionReleased("ui_down"))
            {
                HasReleasedCrouchKey = true;
            }
            if (CheckCanDash && Input.IsActionPressed("dash"))
            {
                StateMachine.ChangeState(PlayerState.Dash);
            }
            if (Input.IsActionJustPressed("interact"))
            {
                if (NowInteraction.Count > 0) NowInteraction[0]?.Interact();
            }
            StateMachine.UnhandledInput(@event);
            if (OS.IsDebugBuild())
            {
                if (Input.IsKeyPressed(Key.P) && Input.IsKeyPressed(Key.Alt))
                {
                    StateMachine.ChangeState(PlayerState.Death);
                }
            }
        }

        public void ResetDashCount(int times = 0)
        {
            DashCount = times > 0 ? times : CanDashCount;
        }

        public void ResetJumpCount(int times = 0)
        {
            JumpCount = times > 0 ? times : CanJumpCount;
        }

        public void ReceiveEvent(string eventName, params object[] datas)
        {
        }

        public void Respawn()
        {
            status.Health = status.MaxHealth;
            StateMachine.ChangeState(PlayerState.Idle);
            WhosYourDaddy.Start();
            CollisionLayer = 0;
            SetCollisionLayerValue(2, true);
            HitBox.SetDeferred("monitoring", true);
            HurtBox.SetDeferred("monitorable", true);
        }

        public Godot.Collections.Dictionary<string, Variant> ToDict()
        {
            return new Godot.Collections.Dictionary<string, Variant>()
            {
                { "direction", Direction },
                { "status", status.ToDict() },
                { "positionX", GlobalPosition.X },
                { "positionY", GlobalPosition.Y }
            };
        }

        public void FromDict(Godot.Collections.Dictionary<string, Variant> dict)
        {
            Direction = (int)dict["direction"];
            status.FromDict(dict["status"].AsGodotDictionary<string, Variant>());
            GlobalPosition = new Vector2((float)dict["positionX"], (float)dict["positionY"]);
        }
    }
}