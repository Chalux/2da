using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Objects.Items
{
    public partial class RedKey:InteractableArea
    {
        public override void _Ready()
        {
            base._Ready();
            Utils.CheckSaveAndFree(this);
        }
        public override void Interact()
        {
            base.Interact();
            if (GameGlobal.Instance.player?.AddItem(3, 1) ?? false)
            {
                Utils.SaveToMapData(this);
                QueueFree();
            }
        }
    }
}
