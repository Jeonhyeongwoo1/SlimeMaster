using SlimeMaster.Enum;
using TMPro;
using UnityEngine;

namespace SlimeMaster.OutGame.UI
{
    public class UI_GachaGradeRateItem : MonoBehaviour
    {
        public Transform GachaRateListParentTransform => _gachaRateListParentTransform;
        public EquipmentGrade EquipmentGrade => _equipmentGrade;
        
        [SerializeField] private EquipmentGrade _equipmentGrade;
        [SerializeField] private TextMeshProUGUI _gachaTitleText;
        [SerializeField] private TextMeshProUGUI _gachaRateText;
        [SerializeField] private Transform _gachaRateListParentTransform;

        public void UpdateUI(string title, string rateValue)
        {
            _gachaTitleText.text = title;
            _gachaRateText.text = rateValue;
        }
    }
}