using SlimeMaster.Enum;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Managers;
using SlimeMaster.Popup;
using UnityEngine;

namespace Script.InGame.UI.Popup
{
    public class UI_LearnSkillPopup : BasePopup
    {
        [SerializeField] private Transform _skillCardParentTransform;

        private UI_SkillCardItem _skillItemCard;
        
        public override void Initialize()
        {
            base.Initialize();

            SafeButtonAddListener(ref _bgCloseButton,
                () => Manager.I.Event.Raise(GameEventType.UpgradeOrAddNewSkill, _skillItemCard.SkillId));
            
            _skillItemCard = Manager.I.UI.AddSubElementItem<UI_SkillCardItem>(_skillCardParentTransform);
            _skillItemCard.Initialize();
        }

        public void UpdateSKillItem(BaseSkill recommendSkill)
        {
            BaseSkill skill = recommendSkill;
            Sprite sprite = Manager.I.Resource.Load<Sprite>(skill.SkillData.IconLabel);
            _skillItemCard.UpdateUI(skill.SkillData.DataId, skill.SkillData.Name, sprite,
                skill.SkillData.Description,
                !skill.IsLearn, skill.CurrentLevel);
        }
    }
}