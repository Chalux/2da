using da.Objects;
using da.Scripts.Objects;

namespace da.Scripts.Items
{
    public partial class IceSpearScript : BaseItemScript
    {
        public override void Use()
        {
            GameGlobal.Instance.player?.LearnedSkill.Add("IceBullet");
            if (SkillManager.Instance.SkillDict.ContainsKey("IceBullet"))
            {
                if (!(GameGlobal.Instance.player?.SkillList.ContainsKey("IceBullet") ?? true)) GameGlobal.Instance.player?.SkillList.Add("IceBullet", SkillManager.Instance.SkillDict["IceBullet"]);
            }
            GameGlobal.Instance.AddMsg("冰钻已学习");
            EventMgr.DispatchEvent("SkillUpdate");
        }
    }
}
