using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.Popup;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace SlimeMaster.InGame.Popup
{
    public class UI_SkillSelectPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _playerCurrentLevelText;
        [SerializeField] private TextMeshProUGUI _beforeLevelText;
        [SerializeField] private TextMeshProUGUI _afterLevelText;
        [SerializeField] private GameObject _skillCardSelectListObject;
        [SerializeField] private Image[] _skillImageArray;
        
        private List<UI_SkillCardItem> _skillCardItemList;
        
        public override void Initialize()
        {
            base.Initialize();
            
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.CurrentLevel
                .Subscribe(OnChangedPlayerLevel)
                .AddTo(this);
            
            _skillCardItemList = new List<UI_SkillCardItem>(Const.SKILL_CARD_ITEM_COUNT);
            for (int i = 0; i < Const.SKILL_CARD_ITEM_COUNT; i++)
            {
                UI_SkillCardItem item =
                    GameManager.I.UI.AddSubElementItem<UI_SkillCardItem>(_skillCardSelectListObject.transform);
                
                item.Initialize();
                _skillCardItemList.Add(item);
            }
        }

        public void ChangePlayerSkill(List<BaseSkill> activateSKillList)
        {
            for (var index = 0; index < activateSKillList.Count; index++)
            {
                var skillModel = activateSKillList[index];
                var sprite = GameManager.I.Resource.Load<Sprite>(skillModel.SkillData.IconLabel);
                var image = _skillImageArray[index];
                image.sprite = sprite;
                image.enabled = true;
            }
        }

        public void UpdateUI(List<BaseSkill> recommendSkillList, List<BaseSkill> activateSkillList)
        {
            for (var i = 0; i < _skillCardItemList.Count; i++)
            {
                BaseSkill skill = recommendSkillList[i];
                Sprite sprite = GameManager.I.Resource.Load<Sprite>(skill.SkillData.IconLabel);
                _skillCardItemList[i].UpdateUI(skill.SkillData.DataId, skill.SkillData.Name, sprite,
                    skill.SkillData.Description,
                    !skill.IsLearn, skill.CurrentLevel);
            }

            ChangePlayerSkill(activateSkillList);
        }

        private void OnChangedPlayerLevel(int level)
        {
            _playerCurrentLevelText.text = level.ToString();
            _beforeLevelText.text = level.ToString();
            _afterLevelText.text = (level + 1).ToString();
        }
    }
}