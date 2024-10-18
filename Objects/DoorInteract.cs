using da.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Objects
{
    public partial class DoorInteract : InteractableArea
    {
        public override void Interact()
        {
            if (Owner is Door)
            {
                (Owner as Door).Open();
            }
        }
    }
}
