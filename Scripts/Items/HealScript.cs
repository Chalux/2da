using da.Objects;
using da.Scripts.Objects;

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
