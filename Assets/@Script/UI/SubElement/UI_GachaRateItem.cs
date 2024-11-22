using SlimeMaster.Data;
using SlimeMaster.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_GachaRateItem : UI_SubItemElement
    {
        [SerializeField] private Image _gradeBackgroundImage;
        [SerializeField] private TextMeshProUGUI _equipmentNameValueText;
        [SerializeField] private TextMeshProUGUI _equipmentRateValueText;

        public void UpdateUI(EquipmentGrade equipmentGrade, string equipmentName, string equipmentRateValue)
        {
            _gradeBackgroundImage.color = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentGrade);
            _equipmentNameValueText.text = equipmentName;
            _equipmentRateValueText.text = equipmentRateValue;
            
            gameObject.SetActive(true);
        }
    }
}