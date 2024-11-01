using Godot;
using System.Collections.Generic;

namespace da.Scripts.Objects.EnemyScript
{
    public partial class EnemyStateMachine : Node
    {
        readonly Dictionary<string, EnemyState> StateDic = new();
        public EnemyState currState;

        public void AddState(string key, EnemyState state)
        {
            if (!StateDic.ContainsKey(key.ToUpper())) StateDic.Add(key.ToUpper(), state);
            else StateDic[key] = state;
            state.OnAdd(Owner as Enemy);
        }

        public void ChangeState(string newState)
        {
            if (StateDic.ContainsKey(newState.ToUpper()) == false) return;
            if (currState != null && currState.State == newState.ToUpper()) return;
            var oldState = currState?.State;
            currState?.Exit(Owner as Enemy);
            currState = StateDic[newState.ToUpper()];
            currState?.Enter(Owner as Enemy);
            //GD.Print($"{Owner.Name} Change State from {oldState?.ToUpper()} to {newState.ToUpper()}");
            //EmitSignal(SignalName.OnStateChange, Variant.From((int)oldState), Variant.From((int)newState));
        }

        public void UnhandledInput(InputEvent @event)
        {
            currState?.UnhandledInput(@event, Owner as Enemy);
        }

        public void PhysicsProcess(double delta)
        {
            currState?.PhysicsProcess(delta, Owner as Enemy);
        }

        public void Update(double delta)
        {
            currState?.Update(delta, Owner as Enemy);
        }

        public void AfterMove(double delta)
        {
            currState?.AfterMove(delta, Owner as Enemy);
        }
    }
}
