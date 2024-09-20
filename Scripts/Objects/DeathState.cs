using da.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects
{
    internal class DeathState : BaseState
    {
        public DeathState()
        {
            State = PlayerState.Death;
        }
        public override void Enter(Player owner)
        {
            owner.CharactorAnimPlayer.Play("death");
        }
    }
}
