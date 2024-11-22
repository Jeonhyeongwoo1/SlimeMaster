using SlimeMaster.Data;
using SlimeMaster.Manager;
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
            GameManager gameManager = GameManager.I;
            SupportSkillData skillData = gameManager.Data.SupportSkillDataDict[_supportSkillId];
            if (isOn)
            {
                gameManager.Object.Player.SkillBook.lockSupportSkillDataList.Add(skillData);
            }
            else
            {
                gameManager.Object.Player.SkillBook.lockSupportSkillDataList.Remove(skillData);
            }

            SupportSkillData supportSkillData =
                gameManager.Object.Player.SkillBook.CurrentSupportSkillDataList.Find(v => v == skillData);
            if (supportSkillData != null)
            {
                supportSkillData.IsLocked = isOn;
            }
        }

        public void SetInfo(SupportSkillData supportSkillData)
        {
            _skillImage.sprite = GameManager.I.Resource.Load<Sprite>(supportSkillData.IconLabel);
            _skillNameText.text = supportSkillData.Name;
            _skillDescriptionText.text = supportSkillData.Description;
            _priceText.text = supportSkillData.Price.ToString();

            _lockToggle.isOn = supportSkillData.IsLocked;
            _soldOutObject.SetActive(supportSkillData.IsPurchased);
            _supportSkillId = supportSkillData.DataId;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool isSuccess = GameManager.I.Object.Player.TryPurchaseSupportSkill(_supportSkillId);
            if (isSuccess)
            {
                _soldOutObject.SetActive(true);
            }
        }
    }
}