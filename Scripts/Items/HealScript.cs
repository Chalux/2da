using da.Objects;
using da.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Scripts.Items
{
    public partial class HealScript : BaseItemScript
    {
        public override void Use()
        {
            GameGlobal.Instance.player?.LearnedSkill.Add("Heal");
            if (SkillManager.Instance.SkillDict.ContainsKey("Heal"))
            {
                if (!(GameGlobal.Instance.player?.SkillList.ContainsKey("Heal") ?? true)) GameGlobal.Instance.player?.SkillList.Add("Heal", SkillManager.Instance.SkillDict["Heal"]);
            }
            GameGlobal.Instance.AddMsg("冰钻已学习");
            EventMgr.DispatchEvent("SkillUpdate");
        }
    }
}
