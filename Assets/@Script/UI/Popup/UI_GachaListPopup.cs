using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.OutGame.UI;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_GachaListPopup : BasePopup
    {
        public List<UI_GachaGradeRateItem> GachagradeRateItemList => _gachagradeRateItemList;
        
        [SerializeField] private List<UI_GachaGradeRateItem> _gachagradeRateItemList;
        [SerializeField] private ScrollRect _scrollRect;
     
        public void ReleaseUIGachaRateItem()
        {
            _gachagradeRateItemList.ForEach(v =>
            {
                var childs = Utils.GetChildComponent<UI_GachaRateItem>(v.GachaRateListParentTransform);
                if (childs != null)
                {
                    foreach (UI_GachaRateItem item in childs)
                    {
                        item.Release();
                    }
                }
            });
        }

        public Transform GetGachaRateListParentTransform(EquipmentGrade equipmentGrade)
        {
            return _gachagradeRateItemList.Find(v => v.EquipmentGrade == equipmentGrade).GachaRateListParentTransform;
        }
        
        public void UpdateGachaGradeRateItem(EquipmentGrade equipmentGrade, string title, string rateValue)
        {
            UI_GachaGradeRateItem element =
                _gachagradeRateItemList.Find(v => v.EquipmentGrade == equipmentGrade);
            element.UpdateUI(title, rateValue);
        }

        public void ForceRebuildLayout()
        {
            _scrollRect.verticalNormalizedPosition = 1;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }
    }
}