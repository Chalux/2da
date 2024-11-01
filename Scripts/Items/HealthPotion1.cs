using da.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Items
{
    internal partial class HealthPotion1 : BaseItemScript
    {
        public override void Use()
        {
            if (GameGlobal.Instance.player != null)
            {
                GameGlobal.Instance.player.status.Health += 5;
            }
            GameGlobal.Instance.AddMsg("喝下了药水，+5生命");
        }
    }
}
