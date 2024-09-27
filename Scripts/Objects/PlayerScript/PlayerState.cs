using da.Objects;
using Godot;

namespace da.Scripts.Objects.PlayerScript
{
    public class BaseState
    {
        public PlayerState State;
        public int CancelLevel = 0;
        public virtual void UnhandledInput(InputEvent @event, Player owner)
        {

        }

        public virtual void PhysicsProcess(double delta, Player owner)
        {

        }

        public virtual void Exit(Player owner)
        {

        }

        public virtual void Enter(Player owner)
        {

        }

        public virtual void Update(double delta, Player owner)
        {

        }

        public virtual void AfterMove(double delta, Player owner)
        {

        }
    }
}
