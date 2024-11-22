using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.InGame.Popup
{
    public class UI_SkillCardItem : UI_SubItemElement, IPointerClickHandler
    {
        public int SkillId => _skillId;
        
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private GameObject _newSkillObject;
        [SerializeField] private GameObject[] _skillLevelIndicatorArray;

        private int _skillId;
        
        public override void Initialize()
        {
            base.Initialize();
            
            gameObject.SetActive(true);
        }

        public void UpdateUI(int skillId, string skillName, Sprite skillSprite, string skillDescription,
            bool isNewSkill, int skillLevel)
        {
            _skillId = skillId;
            _skillNameText.text = skillName;
            _skillImage.sprite = skillSprite;
            _skillDescriptionText.text = skillDescription;
            _newSkillObject.SetActive(isNewSkill);

            for (var i = 0; i < _skillLevelIndicatorArray.Length; i++)
            {
                _skillLevelIndicatorArray[i].SetActive(i <= skillLevel);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.I.Event.Raise(GameEventType.UpgradeOrAddNewSkill, _skillId);
        }
    }
}