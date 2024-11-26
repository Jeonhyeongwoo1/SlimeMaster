using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class MergeAllResultPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private ResourcesManager _resourcesManager = GameManager.I.Resource;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            GameManager.I.Event.AddEvent(GameEventType.ShowMergeResultPopup, OnShowMergeResultPopup);
        }
        
        private void OnShowMergeResultPopup(object value)
        {
            var newItemUIDList = (List<string>)value;
            if (newItemUIDList.Count == 1) // 다수일 경우만
            {
                return;
            }

            UIManager uiManager = GameManager.I.UI;
            var popup = uiManager.OpenPopup<UI_MergeAllResultPopup>();
            popup.ReleaseEquipItem();
            foreach (string uid in newItemUIDList)
            {
                Equipment equipment = _userModel.FindEquippedItemOrUnEquippedItem(uid);
                var equipItem = uiManager.AddSubElementItem<UI_EquipItem>(popup.MergeAlIScrollContentObject);
                (Sprite sprite, Sprite equipTypeSprite, Color gradeColor, int level) = GetTargetEquipmentResource(equipment);

                equipItem.UpdateUI(sprite, equipTypeSprite, false, false, false, false, false, level, gradeColor);
            }
            
            GameManager.I.Audio.Play(Sound.Effect, "Result_CommonMerge");
        }
        
        
        private (Sprite equipSprite, Sprite equipTypeSprite, Color gradeColor, int level) GetTargetEquipmentResource(Equipment targetEquipment)
        {
            Sprite equipSprite = _resourcesManager.Load<Sprite>(targetEquipment.EquipmentData.SpriteName);
            Sprite equipTypeSprite =
                _resourcesManager.Load<Sprite>($"{targetEquipment.EquipmentData.EquipmentType}_Icon.sprite");
            Color gradeColor =
                Const.EquipmentUIColors.GetEquipmentGradeColor(targetEquipment.EquipmentData.EquipmentGrade);
            int level = targetEquipment.Level;

            return (equipSprite, equipTypeSprite, gradeColor, level);
        }
    }
}