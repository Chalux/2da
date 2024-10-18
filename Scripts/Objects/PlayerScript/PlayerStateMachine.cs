using da.Objects;
using Godot;
using System.Collections.Generic;

namespace da.Scripts.Objects.PlayerScript
{
    public enum PlayerState : int
    {
        Idle,
        Jump,
        Dash,
        Crouch,
        Walk,
        Fall,
        Attack1,
        Attack2,
        Attack3,
        Hurt,
        Death,
        QuickDown,
        WallSliding,
        Landing,
        WallJump,
        Slide,
        Sliding,
        ClimbLadder,
    }
    public partial class PlayerStateMachine : Node
    {
        public BaseState currState;
        public PlayerState oldStateEnum;
        [Signal]
        public delegate void OnStateChangeEventHandler(PlayerState oldState, PlayerState newState);

        public Dictionary<PlayerState, BaseState> stateDict = new() {
            { PlayerState.Idle, new IdleState() },
            { PlayerState.Jump, new JumpState() },
            { PlayerState.Dash, new DashState() },
            { PlayerState.Crouch, new CrouchState() },
            { PlayerState.Walk, new WalkState() },
            { PlayerState.Fall, new FallState() },
            { PlayerState.Attack1, new Attack1State() },
            { PlayerState.Attack2, new Attack2State() },
            { PlayerState.Attack3, new Attack3State() },
            { PlayerState.Death, new DeathState() },
            { PlayerState.QuickDown, new QuickDownState() },
            { PlayerState.WallSliding, new WallSlidingState() },
            { PlayerState.Landing, new LandingState() },
            { PlayerState.WallJump, new WallJumpState() },
            { PlayerState.Hurt, new HurtState() },
            { PlayerState.Slide, new SlideState() },
            { PlayerState.Sliding, new SlidingState() },
            { PlayerState.ClimbLadder, new ClimbLadderState() },
        };

        public void ChangeState(PlayerState newState)
        {
            if (stateDict.ContainsKey(newState) == false) return;
            if (currState != null && currState.State == newState) return;
            oldStateEnum = currState?.State ?? PlayerState.Idle;
            currState?.Exit(Owner as Player);
            currState = stateDict[newState];
            (Owner as Player).CharactorAnimPlayer.Advance(0);
            currState?.Enter(Owner as Player);
            //GD.Print($"Change State from {oldStateEnum} to {newState}");
            if ((oldStateEnum != PlayerState.Jump && oldStateEnum != PlayerState.WallJump) && newState == PlayerState.Fall)
            {
                (Owner as Player).JumpCount -= 1;
            }
            EmitSignal(SignalName.OnStateChange, Variant.From((int)oldStateEnum), Variant.From((int)newState));
        }

        public void UnhandledInput(InputEvent @event)
        {
            currState?.UnhandledInput(@event, Owner as Player);
        }

        public void PhysicsProcess(double delta)
        {
            currState?.PhysicsProcess(delta, Owner as Player);
        }

        public void Update(double delta)
        {
            currState?.Update(delta, Owner as Player);
        }

        public void AfterMove(double delta)
        {
            currState?.AfterMove(delta, Owner as Player);
        }
    }
}
