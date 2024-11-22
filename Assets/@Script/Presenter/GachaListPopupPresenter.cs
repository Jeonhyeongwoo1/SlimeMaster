using System;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.OutGame.UI;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class GachaListPopupPresenter : BasePresenter
    {
        private UI_GachaListPopup _popup;
        
        public void Initialize()
        {
            GameManager.I.Event.AddEvent(GameEventType.ShowGachaListPopup, OnShowProbabilityTablePopup);
        }

        private void OnShowProbabilityTablePopup(object value)
        {
            GachaType gachaType = (GachaType)value;
            
            _popup = GameManager.I.UI.OpenPopup<UI_GachaListPopup>();

            DataManager dataManager = GameManager.I.Data;
            UIManager uiManager = GameManager.I.UI;
            GachaTableData tableDataList = dataManager.GachaTableDataDict[gachaType];
            int count = tableDataList.GachaRateTable.Count;
            foreach (UI_GachaGradeRateItem element in _popup.GachagradeRateItemList)
            {
                double rate =
                    Math.Round(tableDataList.GachaRateTable.Where(x => x.EquipGrade == element.EquipmentGrade)
                        .Sum(x => x.GachaRate), 2);
                _popup.UpdateGachaGradeRateItem(element.EquipmentGrade, element.EquipmentGrade.ToString(), $"{rate*100:0.00}%");
            }
            
            Transform commonRateListParentTransform = _popup.GetGachaRateListParentTransform(EquipmentGrade.Common);
            Transform uncommonRateListParentTransform = _popup.GetGachaRateListParentTransform(EquipmentGrade.Uncommon);
            Transform rareRateListParentTransform = _popup.GetGachaRateListParentTransform(EquipmentGrade.Rare);
            Transform epicRateListParentTransform = _popup.GetGachaRateListParentTransform(EquipmentGrade.Epic);

            _popup.ReleaseUIGachaRateItem();
            for (var i = 0; i < count; i++)
            {
                GachaRateData rateData = tableDataList.GachaRateTable[i];
                Transform parent = null;
                switch (rateData.EquipGrade)
                {
                    case EquipmentGrade.Common:
                        parent = commonRateListParentTransform;
                        break;
                    case EquipmentGrade.Uncommon:
                        parent = uncommonRateListParentTransform;
                        break;
                    case EquipmentGrade.Rare:
                        parent = rareRateListParentTransform;
                        break;
                    case EquipmentGrade.Epic:
                        parent = epicRateListParentTransform;
                        break;
                    default:
                        Debug.LogError("Failed get equip grade :" + rateData.EquipGrade);
                        continue;
                }
                
                var item = uiManager.AddSubElementItem<UI_GachaRateItem>(parent);
                EquipmentData equipment = dataManager.EquipmentDataDict[rateData.EquipmentID];
                string format = $"{rateData.GachaRate * 100:0.00}%";
                item.UpdateUI(rateData.EquipGrade, equipment.NameTextID, format);
            }

            _popup.ForceRebuildLayout();
        }
    }
}