using System;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_EquipmentInfoPopup : BasePopup
    {
        [Serializable]
        public struct SKillOptionElement
        {
            public EquipmentGrade equipmentGradeType;
            public Image skillLockImage;
            public TextMeshProUGUI skillOptionDescriptionValueText;
        }
        
        [SerializeField] private Image _gradeBackgroundImage;
        [SerializeField] private TextMeshProUGUI _equipmentNameValueText;
        [SerializeField] private TextMeshProUGUI _equipmentGradeValueText;
        [SerializeField] private Image _equipmentGradeBackgroundImage;
        [SerializeField] private Image _equipmentImage;
        [SerializeField] private Image _equipmentEnforceBackgroundImage;
        [SerializeField] private TextMeshProUGUI _enforceValueText;
        [SerializeField] private Image _equipmentTypeBackgroundImage;
        [SerializeField] private Image _equipmentTypeImage;
        [SerializeField] private TextMeshProUGUI _equipmentLevelValueText;
        [SerializeField] private Button _equipmentResetButton;
        [SerializeField] private Image _equipmentOptionImage;
        [SerializeField] private TextMeshProUGUI _equipmentOptionValueText;

        [Header("[SKill Group]")] 
        [SerializeField] private SKillOptionElement[] _skillOptionElements;
        [SerializeField] private TextMeshProUGUI _costGoldValueText;
        [SerializeField] private TextMeshProUGUI _costMaterialValueText;

        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _unquipButton;
        [SerializeField] private Button _levelupButton;
        [SerializeField] private Button _mergeButton;

        public Action onEquipAction;
        public Action onUnquipAction;
        public Action onLevelUpAction;
        public Action onMergeAction;

        private void Awake()
        {
            _equipButton.SafeAddButtonListener(()=> onEquipAction.Invoke());
            _unquipButton.SafeAddButtonListener(()=> onUnquipAction.Invoke());
            _levelupButton.SafeAddButtonListener(()=> onLevelUpAction.Invoke());
            _mergeButton.SafeAddButtonListener(()=> onMergeAction.Invoke());
        }

        public void UpdateUI(EquipmentGrade equipmentGrade, string equipmentName, Sprite equipmentSprite,
            bool showEnforce, Sprite equipmentTypeSprite, string equipmentLevel, Sprite equipmentOptionSprite,
            string equipmentOptionValue, string[] skillDescriptionArray, string costGoldValue, string costMaterialValue, bool isEquipped)
        {
            Color equipmentGradeColor = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentGrade);
            _gradeBackgroundImage.color = equipmentGradeColor;
            _equipmentNameValueText.text = equipmentName;
            _equipmentGradeValueText.text = equipmentGrade.ToString();
            _equipmentGradeBackgroundImage.color = equipmentGradeColor;
            _equipmentImage.sprite = equipmentSprite;
            _equipmentEnforceBackgroundImage.gameObject.SetActive(showEnforce);
            _enforceValueText.gameObject.SetActive(showEnforce);
            _equipmentTypeBackgroundImage.color = equipmentGradeColor;
            _equipmentTypeImage.sprite = equipmentTypeSprite;
            _equipmentLevelValueText.text = equipmentLevel;
            _equipmentOptionImage.sprite = equipmentOptionSprite;
            _equipmentOptionValueText.text = $"+{equipmentOptionValue}";

            for (var i = 0; i < _skillOptionElements.Length; i++)
            {
                SKillOptionElement element = _skillOptionElements[i];
                element.skillLockImage.gameObject.SetActive((int)equipmentGrade <= (int)element.equipmentGradeType);     
                element.skillOptionDescriptionValueText.text = skillDescriptionArray[i];
            }

            _costGoldValueText.text = costGoldValue;
            _costMaterialValueText.text = costMaterialValue;
            
            _equipButton.gameObject.SetActive(!isEquipped);
            _unquipButton.gameObject.SetActive(isEquipped);
            
            gameObject.SetActive(true);
        }

    }
}