using da.Objects;
using Godot;
using System.Collections.Generic;

namespace da.Scripts.Objects
{
    public enum PlayerState : int
    {
        Idle,
        Jump,
        Dash,
        Crouch,
        Walk,
        Fall,
        Attack,
        Death,
        QuickDown,
        WallSliding,
        Landing,
    }
    public partial class PlayerStateMachine : Node
    {
        public BaseState currState;
        [Signal]
        public delegate void OnStateChangeEventHandler(PlayerState oldState, PlayerState newState);

        public Dictionary<PlayerState, BaseState> stateDict = new() {
            { PlayerState.Idle, new IdleState() },
            { PlayerState.Jump, new JumpState() },
            { PlayerState.Dash, new DashState() },
            { PlayerState.Crouch, new CrouchState() },
            { PlayerState.Walk, new WalkState() },
            { PlayerState.Fall, new FallState() },
            { PlayerState.Attack, new AttackState() },
            { PlayerState.Death, new DeathState() },
            { PlayerState.QuickDown, new QuickDownState() },
            { PlayerState.WallSliding, new WallSlidingState() },
            { PlayerState.Landing, new LandingState() },
        };

        public void ChangeState(PlayerState newState)
        {
            if (stateDict.ContainsKey(newState) == false) return;
            if (currState != null && currState.State == newState) return;
            var oldState = currState?.State ?? PlayerState.Idle;
            currState?.Exit(Owner as Player);
            currState = stateDict[newState];
            currState?.Enter(Owner as Player);
            GD.Print($"Change State from {oldState} to {newState}");
            EmitSignal(SignalName.OnStateChange, Variant.From((int)oldState), Variant.From((int)newState));
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
