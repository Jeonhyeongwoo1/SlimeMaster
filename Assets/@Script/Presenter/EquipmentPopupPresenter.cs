using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class EquipmentPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private UI_EquipmentPopup _equipmentPopup;
        private ResourcesManager _resourcesManager;
        private DataManager _dataManager;
        private UIManager _uiManager;
        private EquipmentSortType _equipmentSortType = EquipmentSortType.Level;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            _resourcesManager = Manager.I.Resource;
            _dataManager = Manager.I.Data;
            _uiManager = Manager.I.UI;
            Manager.I.Event.AddEvent(GameEventType.MoveToTap, OnMoveToTap);
            Manager.I.Event.AddEvent(GameEventType.OnUpdatedEquipment, OnUpdatedEquipment);
        }

        private void OnUpdatedEquipment(object value)
        {
            Refresh();
        }

        private void OnMoveToTap(object value)
        {
            ToggleType toggleType = (ToggleType)value;
            if (toggleType == ToggleType.EquipmentToggle)
            {
                OpenPopup();
            }
        }
        
        private void OpenPopup()
        {
            _equipmentPopup = Manager.I.UI.OpenPopup<UI_EquipmentPopup>();
            _equipmentPopup.Initialize();
            _equipmentPopup.onSortEquipItemAction = OnSortEquipItem;
            _equipmentPopup.AddEvents();
            Refresh();
        }
        
        private void Refresh()
        {
            MakeEquippedItem();
            MakeUnEquipItemInventory();
            MakeCommonItemInventory();
            
            int totalHP = _userModel.MaxHp + _userModel.GetEquipmentBonus().hp;
            int totalAttackDamage = _userModel.MaxAttackDamage + _userModel.GetEquipmentBonus().atk;
            _equipmentPopup.UpdateUI(totalAttackDamage.ToString(), totalHP.ToString());
        }

        private void MakeUnEquipItemInventory()
        {
            _equipmentPopup.ReleaseUneuippedItem();
            List<Equipment> unequipItemList = _userModel.GetUnequipItemList();
            List<Equipment> sortedEquipmentList = null;
            if (_equipmentSortType == EquipmentSortType.Grade)
            {
                sortedEquipmentList = unequipItemList.OrderBy(x => x.EquipmentData.EquipmentGrade).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.Level).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }
            else if (_equipmentSortType == EquipmentSortType.Level)
            {
                sortedEquipmentList = unequipItemList.OrderBy(x=>x.Level).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.EquipmentData.EquipmentGrade).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }
            
            if (sortedEquipmentList != null)
            {
                foreach (Equipment equipment in sortedEquipmentList)
                {
                    AddUIEquipItem(equipment);
                }
            }
            
            string type = _equipmentSortType == EquipmentSortType.Grade ? "레벨 순" : "등급 순";
            _equipmentPopup.SetSortType(type);
        }

        private void OnSortEquipItem()
        {
            _equipmentSortType = _equipmentSortType switch
            {
                EquipmentSortType.Grade => EquipmentSortType.Level,
                EquipmentSortType.Level => EquipmentSortType.Grade,
                _ => _equipmentSortType
            };

            Refresh();
        }
        
        private void MakeEquippedItem()
        {
            _equipmentPopup.ReleaseEquippedItem();
            int length = System.Enum.GetNames(typeof(EquipmentType)).Length;
            for (int i = 0; i < length; i++)
            {
                EquipmentType type = (EquipmentType)i;
                Equipment equipment = _userModel.FindEquippedItem(type);
                if (equipment == null)
                {
                    continue;
                }

                AddUIEquipItem(equipment, true);
            }
        }
        
        private void AddUIEquipItem(Equipment equipment, bool isEquipped = false)
        {
            EquipmentType equipmentType = equipment.EquipmentData.EquipmentType;
            Transform parentTransform = isEquipped
                ? _equipmentPopup.GetEquipmentItemParent(equipmentType)
                : _equipmentPopup.EquipInventoryObject;
            UI_EquipItem equipItem = _uiManager.AddSubElementItem<UI_EquipItem>(parentTransform);
            Sprite sprite = _resourcesManager.Load<Sprite>(equipment.EquipmentData.SpriteName);
            int level = equipment.Level;
            Sprite equipTypeSprite = _resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
            Color gradeColor =
                Const.EquipmentUIColors.GetEquipmentGradeColor(equipment.EquipmentData.EquipmentGrade);
            equipItem.UpdateUI(sprite, equipTypeSprite, false, false,
                false, false, false, level, gradeColor);
            equipItem.AddListener(()=> OnClickEquipItem(equipment.UID));
        }
        
        private void MakeCommonItemInventory()
        {
            List<ItemData> itemDataList = _userModel.ItemDataDict.Value.Where(x =>
                    x.Key != Const.ID_GOLD && x.Key != Const.ID_STAMINA && x.Key != Const.ID_DIA)
                .Select(x => x.Value)
                .ToList();

            _equipmentPopup.ReleaseMaterialItemInInventoryGroup();
            foreach (ItemData itemData in itemDataList)
            {
                GameObject prefab = _resourcesManager.Instantiate(nameof(UI_MaterialItem));
                if (!prefab.TryGetComponent(out UI_MaterialItem materialItem))
                {
                    Debug.LogError("Failed try get component " + nameof(UI_MaterialItem));
                    continue;
                }

                MaterialData materialData = _dataManager.MaterialDataDict[itemData.ItemId.Value];
                Sprite sprite =
                    _resourcesManager.Load<Sprite>(materialData.SpriteName);
                Color color = Const.EquipmentUIColors.GetMaterialGradeColor(materialData.MaterialGrade);
                materialItem.UpdateUI(sprite, color, itemData.ItemValue.Value.ToString(),
                    false, _equipmentPopup.ItemInventoryGroupObject);
            }
        }

        private void OnClickEquipItem(string id)
        {
            Manager.I.Event.Raise(GameEventType.ShowEquipmentInfoPopup, id);
        }
    }
}