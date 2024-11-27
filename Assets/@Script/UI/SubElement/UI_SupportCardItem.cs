using SlimeMaster.Data;
using SlimeMaster.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_SupportCardItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private GameObject _soldOutObject;
        [SerializeField] private Toggle _lockToggle;

        private int _supportSkillId;

        private void Start()
        {
            _lockToggle.onValueChanged.AddListener(OnChangedLockToggle);
        }

        private void OnChangedLockToggle(bool isOn)
        {
            Manager manager = Manager.I;
            SupportSkillData skillData = manager.Data.SupportSkillDataDict[_supportSkillId];
            if (isOn)
            {
                manager.Object.Player.SkillBook._lockSupportSkillDataList.Add(skillData);
            }
            else
            {
                manager.Object.Player.SkillBook._lockSupportSkillDataList.Remove(skillData);
            }

            SupportSkillData supportSkillData =
                manager.Object.Player.SkillBook.CurrentSupportSkillDataList.Find(v => v == skillData);
            if (supportSkillData != null)
            {
                supportSkillData.IsLocked = isOn;
            }
        }

        public void SetInfo(SupportSkillData supportSkillData)
        {
            _skillImage.sprite = Manager.I.Resource.Load<Sprite>(supportSkillData.IconLabel);
            _skillNameText.text = supportSkillData.Name;
            _skillDescriptionText.text = supportSkillData.Description;
            _priceText.text = supportSkillData.Price.ToString();

            _lockToggle.isOn = supportSkillData.IsLocked;
            _soldOutObject.SetActive(supportSkillData.IsPurchased);
            _supportSkillId = supportSkillData.DataId;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool isSuccess = Manager.I.Object.Player.TryPurchaseSupportSkill(_supportSkillId);
            if (isSuccess)
            {
                _soldOutObject.SetActive(true);
            }
        }
    }
}