using Godot;

namespace da.Scripts.Objects.EnemyScript
{
    public class EnemyState
    {
        public string State;
        public virtual void UnhandledInput(InputEvent @event, Enemy owner)
        {

        }

        public virtual void PhysicsProcess(double delta, Enemy owner)
        {

        }

        public virtual void Exit(Enemy owner)
        {

        }

        public virtual void Enter(Enemy owner)
        {

        }

        public virtual void Update(double delta, Enemy owner)
        {

        }

        public virtual void AfterMove(double delta, Enemy owner)
        {

        }

        public virtual void OnAdd(Enemy owner)
        {

        }
    }
}
