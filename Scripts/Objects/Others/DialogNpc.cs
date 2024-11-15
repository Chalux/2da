using DialogicRuntime;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.Others
{
    public partial class DialogNpc : InteractableArea
    {
        [Export] string dialogName = "";
        public override void Interact()
        {
            Dialogic.Start(dialogName);
        }
    }
}
