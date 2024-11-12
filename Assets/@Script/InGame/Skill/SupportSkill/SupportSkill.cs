using SlimeMaster.Data;

namespace SlimeMaster.InGame.Skill
{
    public class SupportSkill
    {
        public SupportSkillData SupportSkillData => _supportSkillData;
        
        private SupportSkillData _supportSkillData;
        
        public SupportSkill(SupportSkillData supportSkillData)
        {
            _supportSkillData = supportSkillData;
            _supportSkillData.IsPurchased = true;
            _supportSkillData.IsLocked = false;
        }
        
    }
}