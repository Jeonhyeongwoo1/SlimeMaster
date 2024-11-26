using System;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class MergeResultPresenter : BasePresenter
    {
        private UserModel _userModel;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            GameManager.I.Event.AddEvent(GameEventType.ShowMergeResultPopup, OnShowMergeResultPopup);
        }

        private void OnShowMergeResultPopup(object value)
        {
            var newItemUIDList = (List<string>)value;
            if (newItemUIDList.Count > 1) // 한개의 경우만 처리
            {
                return;
            }

            var popup = GameManager.I.UI.OpenPopup<UI_MergeResultPopup>();
            string uid = newItemUIDList[0];
            
            Equipment equipment = _userModel.FindEquippedItemOrUnEquippedItem(uid);
            EquipmentData equipmentData = equipment.EquipmentData;
            DataManager dataManager = GameManager.I.Data;
            ResourcesManager resourcesManager = GameManager.I.Resource;

            string equipName = equipmentData.NameTextID;
            string equipGrade = equipmentData.EquipmentGrade.ToString();
            Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.SpriteName);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentData.EquipmentType}_Icon.sprite");
            int level = equipment.Level;
            Color gradeColor = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentData.EquipmentGrade);
            var mergeOptionResultDataList = Utils.GetMergeOptionResultDataList(equipment);
            string improveOptionValue = null;
            switch (equipmentData.EquipmentGrade)
            {
                case EquipmentGrade.Uncommon:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[equipmentData.UncommonGradeSkill].Description}";
                    break;
                case EquipmentGrade.Rare:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[equipmentData.RareGradeSkill].Description}";
                    break;
                case EquipmentGrade.Epic:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[equipmentData.EpicGradeSkill].Description}";
                    break;
                case EquipmentGrade.Legendary:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[equipmentData.LegendaryGradeSkill].Description}";
                    break;
            }
            
            popup.UpdateUI(equipName, equipGrade, equipSprite, equipTypeSprite, level, gradeColor,
                mergeOptionResultDataList, improveOptionValue);
            GameManager.I.Audio.Play(Sound.Effect, "Result_CommonMerge");
        }
    }
}