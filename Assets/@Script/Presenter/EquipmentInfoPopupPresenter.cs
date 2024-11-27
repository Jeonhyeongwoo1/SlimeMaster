using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Factory;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class EquipmentInfoPopupPresenter : BasePresenter
    {
        private UserModel _model;
        private ResourcesManager _resourcesManager;
        private DataManager _dataManager;
        private UI_EquipmentInfoPopup _infoPopup;
        
        public void Initialize(UserModel model)
        {
            _model = model;
            _resourcesManager = Manager.I.Resource;
            _dataManager = Manager.I.Data;
            Manager.I.Event.AddEvent(GameEventType.ShowEquipmentInfoPopup, OnShowEquipmentInfoPopup);
        }

        private void OnShowEquipmentInfoPopup(object value)
        {
            string id = (string)value;
            Equipment equipment = _model.FindEquippedItemOrUnEquippedItem(id);
            if (equipment == null)
            {
                return;
            }
            
            _infoPopup = Manager.I.UI.OpenPopup<UI_EquipmentInfoPopup>();
            
            _infoPopup.onEquipAction = () => OnEquip(equipment);
            _infoPopup.onMergeAction = () => OnMerge(id);
            _infoPopup.onUnquipAction = () => OnUnquip(equipment);
            _infoPopup.onLevelUpAction = () => OnLevelUp(equipment);
            
            Refresh(equipment);
        }

        private void Refresh(Equipment equipment)
        {
            EquipmentData equipmentData = equipment.EquipmentData;
            Sprite equipmentSprite = _resourcesManager.Load<Sprite>(equipmentData.SpriteName);
            Sprite equipmentTypeSprite = _resourcesManager.Load<Sprite>($"{equipmentData.EquipmentType}_Icon");
            string sprName = equipmentData.MaxHpBonus == 0 ? "AttackPoint_Icon.sprite" : "HealthPoint_Icon.sprite";
            Sprite equipmentOptionSprite = _resourcesManager.Load<Sprite>(sprName);
            float equipmentOptionValue = equipmentData.MaxHpBonus == 0 ? equipmentData.AtkDmgBonus : equipmentData.MaxHpBonus;

            Dictionary<int, SupportSkillData> supportSkillDataDict = Manager.I.Data.SupportSkillDataDict;
            string[] skillDescriptionArray = 
            {
                // supportSkillDataDict[equipmentData.BasicSkill].Description,
                supportSkillDataDict[equipmentData.UncommonGradeSkill].Description,
                supportSkillDataDict[equipmentData.RareGradeSkill].Description,
                supportSkillDataDict[equipmentData.EpicGradeSkill].Description,
                supportSkillDataDict[equipmentData.LegendaryGradeSkill].Description
            };

            EquipmentLevelData levelData = _dataManager.EquipmentLevelDataDict[equipment.Level];
            int upgradeCost = levelData.UpgradeCost;
            int upgradeRequiredItems = levelData.UpgradeRequiredItems;
            _infoPopup.UpdateUI(equipmentData.EquipmentGrade, equipmentData.NameTextID, equipmentSprite, false,
                equipmentTypeSprite, equipment.Level.ToString(), equipmentOptionSprite,
                equipmentOptionValue.ToString(), skillDescriptionArray, upgradeCost.ToString(),
                upgradeRequiredItems.ToString(), equipment.IsEquippedByType(equipmentData.EquipmentType));
        }

        private async void OnLevelUp(Equipment equipment)
        {
            var response = await ServerHandlerFactory.Get<IEquipmentClientSender>()
                .EquipmentLevelUpRequest(equipment.DataId, equipment.UID, equipment.Level, equipment.IsEquipped());

            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        //Alret
                        return;
                    case ServerErrorCode.NotEnoughGold:
                        Debug.Log($"Failed {ServerErrorCode.NotEnoughGold}");
                        return;
                    case ServerErrorCode.NotEnoughMaterialAmount:
                        Debug.Log($"Failed {ServerErrorCode.NotEnoughMaterialAmount}");
                        return;
                }
            }

            var goldItemData = response.DBUserData.ItemDataDict[Const.ID_GOLD.ToString()];
            var materialItemData =
                response.DBUserData.ItemDataDict[equipment.EquipmentData.LevelupMaterialID.ToString()];
            _model.SetItemValue(goldItemData.ItemId, goldItemData.ItemValue);
            _model.SetItemValue(materialItemData.ItemId, materialItemData.ItemValue);
            equipment.LevelUp();
            
            Manager.I.Audio.Play(Sound.Effect, "Levelup_Equipment");
            Refresh(equipment);
            Manager.I.Event.Raise(GameEventType.OnUpdatedEquipment);
        }

        private async void OnUnquip(Equipment equipment)
        {
            var response = await ServerHandlerFactory.Get<IEquipmentClientSender>()
                .UnequipRequest(equipment.UID);

            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        //Alert
                        return;
                    case ServerErrorCode.FailedGetEquipment:
                        Debug.Log("Failed get equipement");
                        return;
                }
            }

            _model.ClearAndSetEquipmentDataList(response.EquipmentDataList);
            _model.ClearAndSetUnEquipmentDataList(response.UnEquipmentDataList);
            Manager.I.UI.ClosePopup();
            Manager.I.Event.Raise(GameEventType.OnUpdatedEquipment);
        }

        private void OnMerge(string equipmentId)
        {
            
        }

        private async void OnEquip(Equipment equipment)
        {
            Equipment equippedItem = _model.FindEquippedItem(equipment.EquipmentData.EquipmentType);
            var response = await ServerHandlerFactory.Get<IEquipmentClientSender>()
                .EquipRequest(equipment.UID, equippedItem?.UID);

            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        //Alert
                        return;
                    case ServerErrorCode.FailedGetEquipment:
                        Debug.Log("Failed get equipement");
                        return;
                }
            }
            
            _model.ClearAndSetEquipmentDataList(response.EquipmentDataList);
            _model.ClearAndSetUnEquipmentDataList(response.UnEquipmentDataList);
            Manager.I.UI.ClosePopup();
            Manager.I.Event.Raise(GameEventType.OnUpdatedEquipment);
        }
    }
}