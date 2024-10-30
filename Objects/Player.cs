using da.Scripts;
using da.Scripts.Interfaces;
using da.Scripts.Objects;
using da.Scripts.Objects.PlayerScript;
using DialogicRuntime;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace da.Objects
{
    public partial class Player : CharacterBody2D, IEvent, IAttackable, IHurtable
    {
        public const int RIGHTPOSITION = 1;
        public const int LEFTPOSITION = -1;
        [Export] public Sprite2D Sprite;
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
        [Export] public Camera2D PlayerCamera;
        [Export] public PauseScene PauseScene;
        [Export] public Marker2D BubbleMarker;
        [Export] public RayCast2D LadderRay;
        [Export] public Marker2D SlashMarker;
        [Export] public RayCast2D SlashChecker;
        [Export] public RayCast2D WaterChecker;
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
        private int _jumpcount = 0;
        public Skill currSkill;
        public Godot.Collections.Array<string> LearnedSkill = new();
        public System.Collections.Generic.Dictionary<string, Skill> SkillList = new();
        public Array<Item> Bag = new();
        private BagCompare _bagCompare = new();
        public int JumpCount
        {
            get => _jumpcount;
            set
            {
                _jumpcount = value;
                //GD.Print($"JumpCount : {_jumpcount}");
            }
        }
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
        public bool SkipInput = false;

        // Get the gravity from the project settings to be synced with RigidBody nodes.
        public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        public override void _PhysicsProcess(double delta)
        {
            if (GameGlobal.Instance.IsChangingScene || SkipInput) return;

            foreach (var skill in SkillList.Values)
            {
                if (skill.CurrentCooldown > 0)
                {
                    skill.CurrentCooldown -= (float)delta;
                    skill.CurrentCooldown = Mathf.Max(skill.CurrentCooldown, 0);
                }
            };

            StateMachine.PhysicsProcess(delta);

            MoveAndSlide();

            StateMachine.AfterMove(delta);

            if (WaterChecker.IsColliding())
            {
                StateMachine.ChangeState(PlayerState.InWater);
            }
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
                Direction = (damage.source.Owner as Enemy).Position.X >= Position.X ? 1 : -1;
                if (status.Health > 0)
                {
                    StateMachine.ChangeState(PlayerState.Hurt);
                }
                else
                {
                    StateMachine.ChangeState(PlayerState.Death);
                }
            };
            WhosYourDaddy.Timeout += InvincibleTimeout;
            EventMgr.RegisterEvent(this);
            EventMgr.DispatchEvent("PlayerReady", this);
            Dialogic.TimelineStarted += () =>
            {
                SkipInput = true;
            };
            Dialogic.TimelineEnded += () =>
            {
                SkipInput = false;
            };
        }

        public bool CheckCanDash => (DashCount > 0 || StateMachine.currState.State == PlayerState.WallSliding || (StateMachine.currState.State == PlayerState.Dash && StateMachine.oldStateEnum == PlayerState.WallSliding)) && DashTimer.IsStopped() && DashCoolDownTimer.IsStopped();
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
            if (GameGlobal.Instance.IsChangingScene) return;
            if (@event.IsActionPressed("jump"))
            {
                JumpRequestTimer.Start();
                if (IsOnFloor()) HasReleasedJumpKey = false;
            }
            if (@event.IsActionPressed("attack"))
            {
                AttackRequestTimer.Start();
            }
            if (@event.IsActionReleased("jump"))
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
            if (CheckCanDash && Input.IsActionJustPressed("dash"))
            {
                StateMachine.ChangeState(PlayerState.Dash);
            }
            if (Input.IsActionJustPressed("interact"))
            {
                if (NowInteraction.Count > 0) NowInteraction[0]?.Interact();
            }
            if (Input.IsActionJustPressed("pause"))
            {
                PauseScene.ShowPause();
            }
            if (@event.IsActionPressed("skill1"))
            {
                GameGlobal.Instance.skillBtn1.OnPressed();
            }
            else if (@event.IsActionPressed("skill2"))
            {
                GameGlobal.Instance.skillBtn2.OnPressed();
            }
            else if (@event.IsActionPressed("skill3"))
            {
                GameGlobal.Instance.skillBtn3.OnPressed();
            }
            else if (@event.IsActionPressed("skill4"))
            {
                GameGlobal.Instance.skillBtn4.OnPressed();
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
            if (Sprite.Material != null)
            {
                (Sprite.Material as ShaderMaterial).SetShaderParameter("enable", true);
            }
            CollisionLayer = 0;
            SetCollisionLayerValue(2, true);
            HitBox.SetDeferred("monitoring", true);
            HurtBox.SetDeferred("monitorable", true);
        }

        public Godot.Collections.Dictionary<string, Variant> ToDict()
        {
            Godot.Collections.Dictionary<int, int> bagdict = new();
            foreach (var item in Bag)
            {
                if (item != null)
                {
                    bagdict.Add(item.id, item.StackCount);
                }
            }
            var result = new Godot.Collections.Dictionary<string, Variant>()
            {
                { "direction", Direction },
                { "status", status.ToDict() },
                { "learned_skill", LearnedSkill },
                { "bag", bagdict },
            };
            if (IsNodeReady())
            {
                result.Add("positionX", GlobalPosition.X);
                result.Add("positionY", GlobalPosition.Y);
            }
            return result;
        }

        public void FromDict(Godot.Collections.Dictionary<string, Variant> dict, bool ignorePosition = false)
        {
            Direction = dict.GetValueOrDefault("direction", 1).AsInt32();
            if (dict.ContainsKey("status"))
            {
                status.FromDict(dict["status"].AsGodotDictionary<string, Variant>());
            }
            if (dict.ContainsKey("positionX") && dict.ContainsKey("positionY") && !ignorePosition)
            {
                GlobalPosition = new Vector2((float)dict["positionX"], (float)dict["positionY"]);
            }
            if (dict.ContainsKey("learned_skill"))
            {
                LearnedSkill = dict["learned_skill"].AsGodotArray<string>();
                SkillList = new();
                foreach (var skill in LearnedSkill)
                {
                    if (SkillManager.Instance.SkillDict.ContainsKey(skill) && SkillManager.Instance.SkillDict[skill] != null && !SkillList.ContainsKey(skill))
                    {
                        SkillList.Add(skill, SkillManager.Instance.SkillDict[skill]);
                    }
                }
            }
            if (dict.ContainsKey("bag"))
            {
                Bag = new();
                var datas = dict["bag"].AsGodotDictionary<int, int>();
                foreach (var data in datas)
                {
                    //Bag.Add(new Item(data.Key) { StackCount = data.Value });
                    AddItem(data.Key, data.Value);
                }
                if (Bag.Count < 30)
                {
                    Bag.Resize(30);
                }
            }
        }

        public HitBox GetHitBox() => HitBox;

        public Damage DoAttack(IAttackable attacker, IHurtable target)
        {
            float AttackStunDuration = 0.1f;
            float AttackStunPower = 0.01f;
            switch (StateMachine.currState.State)
            {
                case PlayerState.Attack3:
                    AttackStunDuration = 1f;
                    AttackStunPower = 0.01f;
                    break;
            }
            Damage result = new()
            {
                value = 1,
                source = GetHitBox(),
                target = target.GetHurtBox(),
                onHitSound = ResourceLoader.Load<AudioStream>("res://Resources/SFX/17_orc_atk_sword_3.wav"),
                isStunFrame = true,
                stunDuration = AttackStunDuration,
                stunPower = AttackStunPower
            };
            return result;
        }

        public HurtBox GetHurtBox()
        {
            return HurtBox;
        }

        private void InvincibleTimeout()
        {
            HurtBox.SetDeferred("monitorable", true);
            if (Sprite.Material != null)
            {
                (Sprite.Material as ShaderMaterial).SetShaderParameter("enable", false);
            }
        }

        public void GenerateSlash()
        {
            if (SlashChecker.IsColliding()) return;
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Objects/SlashVFX.tscn");
            if (res == null) return;
            SlashVfx slash = res.Instantiate<SlashVfx>();
            var CurrMap = FindParent("*Map*");
            CurrMap.AddChild(slash);
            //var transform = SlashMarker.GetRelativeTransformToParent(CurrMap);
            slash.Init(this, SlashMarker.GlobalPosition, Direction);
        }

        public void UseSkill()
        {
            //if (currSkill == null) return;
            currSkill?.SkillUse();
        }

        public bool AddItem(int id, int count)
        {
            if (count == 0) return false;
            int i = 0;
            for (; i < Bag.Count; i++)
            {
                var item = Bag[i];
                if (item == null || item.id > id)
                {
                    break;
                }
                if (item.id == id)
                {
                    if (count < 0 && item.StackCount < count) return false;
                    else
                    {
                        item.StackCount += count;
                        if (item.StackCount <= 0)
                        {
                            Bag.RemoveAt(i);
                        }
                        _ = Bag.OrderBy(i => i, _bagCompare);
                        EventMgr.DispatchEvent("BagUpdate");
                        return true;
                    }
                }
            }
            if (count > 0 && Datas.Items.ContainsKey(id))
            {
                Item item = new(id)
                {
                    StackCount = count
                };
                if (Bag.Count <= i)
                {
                    for (int j = Bag.Count; j <= i; j++)
                    {
                        Bag.Add(null);
                    }
                }
                if (Bag[i] == null) Bag[i] = item;
                else Bag.Add(item);
                _ = Bag.OrderBy(i => i, _bagCompare);
                EventMgr.DispatchEvent("BagUpdate");
                return true;
            }
            return false;
        }
    }

    public class BagCompare : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {
            if (x == null || y == null) return int.MaxValue;
            else return x.id.CompareTo(y.id);
        }
    }
}