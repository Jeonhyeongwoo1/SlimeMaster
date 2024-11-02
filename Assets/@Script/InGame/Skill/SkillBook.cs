using System.Collections.Generic;

namespace SlimeMaster.InGame.Skill
{
    public class SkillBook
    {
        private List<BaseSkill> _baseSkillList = new();

        public void AddSkill(BaseSkill skill)
        {
            _baseSkillList.Add(skill);
        }
        
    }
}