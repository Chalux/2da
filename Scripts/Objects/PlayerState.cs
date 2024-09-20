using da.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    public class BaseState
    {
        public PlayerState State;
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
