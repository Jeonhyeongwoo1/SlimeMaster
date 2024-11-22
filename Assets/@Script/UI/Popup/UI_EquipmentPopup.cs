using System;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_EquipmentPopup : BasePopup
    {
        [Serializable]
        public struct EquipmentElement
        {
            public EquipmentType equipmentType;
            public GameObject weaponObject;
        }

        public Transform ItemInventoryGroupObject => _itemInventoryGroupObject;
        public Transform EquipInventoryObject => _equipInventoryObject;
        
        [SerializeField] private List<EquipmentElement> _equipmentElementList;
        [SerializeField] private TextMeshProUGUI _totalAttackDamageText;
        [SerializeField] private TextMeshProUGUI _totalHealthText;
        [SerializeField] private GameObject _characterButtonObject;
        [SerializeField] private Transform _itemInventoryGroupObject;
        [SerializeField] private Transform _equipInventoryObject;
        [SerializeField] private Button _sortButton;
        [SerializeField] private Button _mergeButton;
        [SerializeField] private TextMeshProUGUI _sortText;
        
        public Action onSortEquipItemAction;

        public override void AddEvents()
        {
            base.AddEvents();
            
            _sortButton.SafeAddButtonListener(onSortEquipItemAction.Invoke);
            _mergeButton.SafeAddButtonListener(OnClickMergeButton);
        }

        private void OnClickMergeButton()
        {
            GameManager.I.Event.Raise(GameEventType.ShowMergePopup);
        }

        public void SetSortType(string sortType)
        {
            _sortText.text = sortType;
        }

        public void ReleaseMaterialItemInInventoryGroup()
        {
            var childs = Utils.GetChildComponent<UI_MaterialItem>(_itemInventoryGroupObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_MaterialItem materialItem in childs)
            {
                materialItem.Release();
            }
        }

        public void ReleaseEquippedItem()
        {
            foreach (EquipmentElement equipmentElementData in _equipmentElementList)
            {
                var childs = Utils.GetChildComponent<UI_EquipItem>(equipmentElementData.weaponObject.transform);
                if (childs != null)
                {
                    foreach (UI_EquipItem item in childs)
                    {
                        item.Release();
                    }
                }
            }
        }

        public void ReleaseUneuippedItem()
        {
            var childs = Utils.GetChildComponent<UI_EquipItem>(_equipInventoryObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_EquipItem equipItem in childs)
            {
                equipItem.Release();
            }
        }

        public Transform GetEquipmentItemParent(EquipmentType equipmentType)
        {
            return _equipmentElementList.Find(v => v.equipmentType == equipmentType).weaponObject.transform;
        }
        
        public void UpdateUI(string totalAttackDamageValue,
            string totalHealthValue)
        {
            _totalAttackDamageText.text = totalAttackDamageValue;
            _totalHealthText.text = totalHealthValue;
            _characterButtonObject.SetActive(false);
        }
    }
}