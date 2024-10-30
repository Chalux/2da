using da.Scripts;
using da.Scripts.Skills;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Objects
{
    public partial class SkillManager : Node
    {
        public static SkillManager Instance { get; private set; }
        public override void _Ready()
        {
            Instance = this;
        }

        public SkillManager()
        {
            LoadSkillData();
        }

        public readonly Dictionary<string, Skill> SkillDict = new();

        public void LoadSkillData()
        {
            // 懒狗不想写配置文件了直接写死吧
            SkillDict.Add("IceBullet", new IceBulletSkill());
        }
    }
}
