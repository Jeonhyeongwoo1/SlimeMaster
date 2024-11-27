using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class GachaResultPresenter : BasePresenter
    {
        private UserModel _model;
        public void Initialize(UserModel model)
        {
            _model = model;
            Manager.I.Event.AddEvent(GameEventType.ShowGachaResultPopup, OnShowResultPopup);
        }
        
        private void OnShowResultPopup(object value)
        {
            var rewardEquipmentList = (List<DBEquipmentData>)value;
            if (rewardEquipmentList == null)
            {
                return;
            }

            UIManager uiManager = Manager.I.UI;
            ResourcesManager resourcesManager = Manager.I.Resource;
            var popup = uiManager.OpenPopup<UI_GachaResultsPopup>();
            popup.ReleaseItem();
            popup.AddEvents();
            
            popup.onEndGachaAnimationAction = () =>
            {
                int count = rewardEquipmentList.Count;
                for (int i = 0; i < count; i++)
                {
                    DBEquipmentData equipmentData = rewardEquipmentList[i];
                    Equipment equipment = _model.FindEquippedItemOrUnEquippedItem(equipmentData.UID);
                    if (equipment == null)
                    {
                        Debug.LogError("Failed get equipment :" + equipmentData.UID);
                        continue;
                    }
                
                    var materialItem = uiManager.AddSubElementItem<UI_MaterialItem>(popup.ResultsContentScrollObject);
                    var sprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.SpriteName);
                    Color color =
                        Const.EquipmentUIColors.GetEquipmentGradeColor(equipment.EquipmentData.EquipmentGrade);
                    materialItem.UpdateUI(sprite, color, 1.ToString(), true);
                }
            };
        }
    }
}