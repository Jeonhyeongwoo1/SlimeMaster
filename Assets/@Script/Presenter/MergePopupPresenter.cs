using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Factory;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.Shared.Data;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    [Serializable]
    public class MergeOptionResultData
    {
        public EquipAbilityStatType equipAbilityStatType;
        public float beforeValue;
        public float afterValue;
    }

    public class MergePopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private Dictionary<Equipment, UI_EquipItem> _inventoryEquipItemDict = new();
        private List<Equipment> _equipmentList;
        private UI_MergePopup _mergePopup;
        private ResourcesManager _resourcesManager;
        private UIManager _uiManager;
        private Equipment _selectedResultMergeEquipment;
        private Equipment _selectMergeCostFirstEquipItem;
        private Equipment _selectMergeCostSecondEquipItem;
        private EquipmentSortType _equipmentSortType;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            _resourcesManager = Manager.I.Resource;
            _uiManager = Manager.I.UI;
            
            Manager.I.Event.AddEvent(GameEventType.ShowMergePopup, OnShowMergePopup);
        }

        private void OnClosePopup()
        {
            _selectedResultMergeEquipment = null;
            _selectMergeCostFirstEquipItem = null;
            _selectMergeCostSecondEquipItem = null;
            
            _uiManager.ClosePopup();
        }
        
        private void OnShowMergePopup(object value)
        {
            _mergePopup = _uiManager.OpenPopup<UI_MergePopup>();
            _mergePopup.onReleaseSelectedEquipment = OnReleaseSelectedResultMergeEquipment;
            _mergePopup.onReleaseSelectedCostEquipment = OnReleaseSelectedCostEquipment;
            _mergePopup.onMergeAction = OnMergeEquipment;
            _mergePopup.onClosePopupAction = OnClosePopup;
            _mergePopup.onAllMergeAction = OnAllMergeAction;
            _mergePopup.onSortEquipItemAction = OnSortEquipItemAction;
            _mergePopup.AddEvents();

            AddEquipmentList();
            _equipmentSortType = EquipmentSortType.Grade;
            SortEquipItem(_equipmentSortType);
            _mergePopup.UpdateMergeResultEquipItem(false, true, true, null, null, 0, Color.white);
        }

        private void SortEquipItem(EquipmentSortType equipmentSortType)
        {
            List<Equipment> sortedEquipmentList = null;
            if (equipmentSortType == EquipmentSortType.Grade)
            {
                sortedEquipmentList = _equipmentList.OrderBy(x => x.EquipmentData.EquipmentGrade).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.Level).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }
            else if (equipmentSortType == EquipmentSortType.Level)
            {
                sortedEquipmentList = _equipmentList.OrderBy(x=>x.Level).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.EquipmentData.EquipmentGrade).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }

            _equipmentList = sortedEquipmentList;
            string type = _equipmentSortType == EquipmentSortType.Grade ? "레벨 순" : "등급 순";
            _mergePopup.SetSortTypeText(type);            
            RefreshEquipItemUI();
        }
        
        private void OnSortEquipItemAction()
        {
            if (_equipmentSortType == EquipmentSortType.Grade)
            {
                _equipmentSortType = EquipmentSortType.Level;
            }
            else if(_equipmentSortType == EquipmentSortType.Level)
            {
                _equipmentSortType = EquipmentSortType.Grade;
            }
            
            SortEquipItem(_equipmentSortType);
        }
        
        private async void OnAllMergeAction()
        {
            var equipmentDataList = new List<AllMergeEquipmentRequestData>();
            for (int i = _equipmentList.Count - 1; i >= 0; i--)
            {
                Equipment targetEquipment = _equipmentList[i];
                if (targetEquipment.IsEquipped())
                {
                    continue;
                }
                
                Equipment firstCostItem = null;
                Equipment secondCostItem = null;
                for (int j = 0; j < _equipmentList.Count; j++)
                {
                    Equipment equipment = _equipmentList[j];
                    if (targetEquipment == equipment)
                    {
                        continue;
                    }

                    if (equipment.IsEquipped())
                    {
                        continue;
                    }

                    if (targetEquipment.IsPossibleMerge(equipment))
                    {
                        if (targetEquipment.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None &&
                            firstCostItem == null)
                        {
                            firstCostItem = targetEquipment;
                            continue;
                        }

                        if (targetEquipment.EquipmentData.MergeEquipmentType2 != MergeEquipmentType.None)
                        {
                            secondCostItem = targetEquipment;
                            break;
                        }
                    }
                }

                //1.첫번째가 None이 아니고 두번째도 None아닐때
                //2.첫번째가 None이 아니고 두번째는 None일때

                bool isPossibleMerge = false;
                if (targetEquipment.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None &&
                    firstCostItem != null &&
                    targetEquipment.EquipmentData.MergeEquipmentType2 != MergeEquipmentType.None &&
                    secondCostItem != null)
                {
                    isPossibleMerge = true;
                }
                else if (targetEquipment.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None &&
                         firstCostItem != null &&
                         targetEquipment.EquipmentData.MergeEquipmentType2 == MergeEquipmentType.None &&
                         secondCostItem == null)
                {
                    isPossibleMerge = true;
                }

                if (!isPossibleMerge)
                {
                    continue;
                }

                var data = new AllMergeEquipmentRequestData();
                data.selectedEquipItemUid = targetEquipment.UID;
                data.id1 = targetEquipment.DataId;
                if (firstCostItem != null)
                {
                    data.firstCostItemUID = firstCostItem.UID;
                    data.id2 = firstCostItem.DataId;
                }

                if (secondCostItem != null)
                {
                    data.selectedEquipItemUid = secondCostItem.UID;
                    data.id3 = secondCostItem.DataId;
                }

                equipmentDataList.Add(data);
                _equipmentList.Remove(targetEquipment);

                if (firstCostItem != null)
                {
                    _equipmentList.Remove(firstCostItem);
                }

                if (secondCostItem != null)
                {
                    _equipmentList.Remove(secondCostItem);
                }
            }

            if (equipmentDataList.Count == 0)
            {
                return;
            }

            foreach (AllMergeEquipmentRequestData allMergeEquipmentData in equipmentDataList)
            {
                Debug.Log($" {allMergeEquipmentData.id1} / {allMergeEquipmentData.id2} / {allMergeEquipmentData.id3}");
            }

            var response = await ServerHandlerFactory.Get<IEquipmentClientSender>().MergeEquipmentRequest(equipmentDataList);
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        Debug.LogError(response.errorMessage);
                        return;
                    case ServerErrorCode.FailedGetEquipment:
                        Debug.LogError("failed get equipment");
                        return;
                }
            }
            
            Debug.Log("Success");
            _userModel.ClearAndSetUnEquipmentDataList(response.UnEquipmentDataList);
            AddEquipmentList();
            Reset();
            SortEquipItem(_equipmentSortType);
            Manager.I.Event.Raise(GameEventType.ShowMergeResultPopup, response.NewItemUIDList);
        }

        private void AddEquipmentList()
        {
            _equipmentList ??= new List<Equipment>();

            if (_equipmentList.Count > 0)
            {
                _equipmentList.Clear();
            }

            _equipmentList = _userModel.EquippedItemDataList.Value.Concat(_userModel.UnEquippedItemDataList.Value).ToList();
        }

        private async void OnMergeEquipment()
        {
            if (_selectedResultMergeEquipment == null)
            {
                return;
            }

            if (_selectMergeCostFirstEquipItem.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None &&
                _selectMergeCostFirstEquipItem == null)
            {
                return;
            }
            
            if (!_selectedResultMergeEquipment.IsPossibleMerge(_selectMergeCostFirstEquipItem))
            {
                return;
            }
            
            string secondCostItemUid = null;
            if (_selectMergeCostFirstEquipItem.EquipmentData.MergeEquipmentType2 != MergeEquipmentType.None &&
                _selectMergeCostSecondEquipItem != null)
            {
                if (!_selectedResultMergeEquipment.IsPossibleMerge(_selectMergeCostSecondEquipItem))
                {
                    return;
                }
                
                secondCostItemUid = _selectMergeCostSecondEquipItem.UID;
            }
            
            string firstCostItemUid = _selectMergeCostFirstEquipItem.UID;
            string selectedEquipItemUid = _selectedResultMergeEquipment.UID;
            var equipmentDataList = new List<AllMergeEquipmentRequestData>();
            var data = new AllMergeEquipmentRequestData
            {
                selectedEquipItemUid = selectedEquipItemUid,
                firstCostItemUID = firstCostItemUid,
                secondCostItemUID = secondCostItemUid
            };
            
            equipmentDataList.Add(data);
            var response = await ServerHandlerFactory.Get<IEquipmentClientSender>().MergeEquipmentRequest(equipmentDataList);
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        Debug.LogError(response.errorMessage);
                        return;
                    case ServerErrorCode.FailedGetEquipment:
                        Debug.LogError("failed get equipment");
                        return;
                }
            }
            
            Debug.Log("Success");

            _userModel.ClearAndSetUnEquipmentDataList(response.UnEquipmentDataList);
            AddEquipmentList();
            Reset();
            SortEquipItem(_equipmentSortType);
            Manager.I.Audio.Play(Sound.Effect, "Merge_Equipment");
            Manager.I.Event.Raise(GameEventType.ShowMergeResultPopup, response.NewItemUIDList);
        }

        private void Reset()
        {
            _selectedResultMergeEquipment = null;
            _selectMergeCostFirstEquipItem = null;
            _selectMergeCostSecondEquipItem = null;
            _mergePopup.UpdateMergeResultEquipItem(false, true, true, null, null, 0, Color.white);
            _mergePopup.RestEquipMergeCostItem(true, true);
        }

        private void OnReleaseSelectedResultMergeEquipment()
        {
            if (_selectedResultMergeEquipment == null)
            {
                return;
            }

            Reset();
            SortEquipItem(_equipmentSortType);
        }

        private void OnReleaseSelectedCostEquipment(bool isFirstItem)
        {
            Equipment targetEquipment = isFirstItem ? _selectMergeCostFirstEquipItem : _selectMergeCostSecondEquipItem;
            if (targetEquipment == null)
            {
                return;
            }
            
            if (isFirstItem)
            {
                _selectMergeCostFirstEquipItem = null;
                _mergePopup.RestEquipMergeCostItem(true, false);
            }
            else
            {
                _selectMergeCostSecondEquipItem = null;
                _mergePopup.RestEquipMergeCostItem(false, true);
            }
            
            SortEquipItem(_equipmentSortType);
            foreach (var (equipment, equipItem) in _inventoryEquipItemDict)
            {
                equipItem.SetLock(!targetEquipment.IsPossibleMerge(equipment));
            }
        }

        private void RefreshEquipItemUI()
        {
            _inventoryEquipItemDict.Clear();
            _mergePopup.ReleaseEquipItem();
            
            int count = _equipmentList.Count;
            for (int i = 0; i < count; i++)
            {
                Equipment equipment = _equipmentList[i];
                if (equipment == _selectedResultMergeEquipment || equipment == _selectMergeCostFirstEquipItem ||
                    equipment == _selectMergeCostSecondEquipItem)
                {
                    continue;
                }

                var equipItem = _uiManager.AddSubElementItem<UI_EquipItem>(_mergePopup.EquipInventoryScrollContentObject);
                (Sprite sprite, Sprite equipTypeSprite, Color gradeColor, int level) = GetTargetEquipmentResource(equipment);
                equipItem.UpdateUI(sprite, equipTypeSprite, false, false,
                    equipment.IsEquipped(), false, false, level, gradeColor);
                equipItem.AddListener(()=> OnClickEquipItem(equipment));
                _inventoryEquipItemDict.Add(equipment, equipItem);
            }
            
            if (_selectedResultMergeEquipment != null)
            {
                foreach (var (equipment, equipItem) in _inventoryEquipItemDict)
                {
                    equipItem.SetLock(!_selectedResultMergeEquipment.IsPossibleMerge(equipment));
                }
            }
        }

        private void OnClickEquipItem(Equipment targetEquipment)
        {
            if (_selectedResultMergeEquipment == null)
            {
                OnSelectMergeResultEquipItem(targetEquipment);
            }
            else
            {
                OnSelectMergeCostEquipItem(targetEquipment);
            }
            
            SortEquipItem(_equipmentSortType);
        }
        
        private void OnSelectMergeCostEquipItem(Equipment targetEquipment)
        {
            if (_selectMergeCostFirstEquipItem == null &&
                _selectedResultMergeEquipment != null &&
                _selectedResultMergeEquipment.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None)
            {
                _selectMergeCostFirstEquipItem = targetEquipment;
                (Sprite sprite, Sprite equipTypeSprite, Color gradeColor, int level) = GetTargetEquipmentResource(targetEquipment);
                _mergePopup.UpdateEquipMergeCostItem(true, gradeColor, sprite, level, equipTypeSprite);
                if (_inventoryEquipItemDict.TryGetValue(targetEquipment, out UI_EquipItem uiEquipItem))
                {
                    uiEquipItem.Release();
                    _inventoryEquipItemDict.Remove(targetEquipment);
                }

                if (_selectedResultMergeEquipment.EquipmentData.MergeEquipmentType2 == MergeEquipmentType.None)
                {
                    CostEquipItemAllFilled();
                }

                return;
            }

            if (_selectMergeCostSecondEquipItem == null &&
                _selectedResultMergeEquipment != null &&
                _selectedResultMergeEquipment.EquipmentData.MergeEquipmentType2 != MergeEquipmentType.None)
            {
                _selectMergeCostSecondEquipItem = targetEquipment;
                (Sprite sprite, Sprite equipTypeSprite, Color gradeColor, int level) = GetTargetEquipmentResource(targetEquipment);
                _mergePopup.UpdateEquipMergeCostItem(false, gradeColor, sprite, level, equipTypeSprite);
                if (_inventoryEquipItemDict.TryGetValue(targetEquipment, out UI_EquipItem uiEquipItem))
                {
                    uiEquipItem.Release();
                    _inventoryEquipItemDict.Remove(targetEquipment);
                }
                
                CostEquipItemAllFilled();
            }
        }

        private void CostEquipItemAllFilled()
        {
            EquipmentData selectedEquipmentData = _selectedResultMergeEquipment.EquipmentData;
            string equipmentCode = _selectedResultMergeEquipment.EquipmentData.MergedItemCode;
            DataManager dataManager = Manager.I.Data;
            EquipmentData resultEquipmentData = dataManager.EquipmentDataDict[equipmentCode];
            var mergeOptionResultDataList = Utils.GetMergeOptionResultDataList(_selectedResultMergeEquipment);
            string improveOptionValue = null;
            switch (resultEquipmentData.EquipmentGrade)
            {
                case EquipmentGrade.Uncommon:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[resultEquipmentData.UncommonGradeSkill].Description}";
                    break;
                case EquipmentGrade.Rare:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[resultEquipmentData.RareGradeSkill].Description}";
                    break;
                case EquipmentGrade.Epic:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[resultEquipmentData.EpicGradeSkill].Description}";
                    break;
                case EquipmentGrade.Legendary:
                    improveOptionValue =$"+ {dataManager.SupportSkillDataDict[resultEquipmentData.LegendaryGradeSkill].Description}";
                    break;
            }
            
            string equipName = selectedEquipmentData.NameTextID;
            Sprite equipSprite = _resourcesManager.Load<Sprite>(_selectedResultMergeEquipment.EquipmentData.SpriteName);
            Sprite equipTypeSprite =
                _resourcesManager.Load<Sprite>($"{_selectedResultMergeEquipment.EquipmentData.EquipmentType}_Icon.sprite");
            Color gradeColor =
                Const.EquipmentUIColors.GetEquipmentGradeColor(_selectedResultMergeEquipment.EquipmentData.EquipmentGrade + 1);
            int level = _selectedResultMergeEquipment.Level;
            _mergePopup.CostEquipItemAllFilled(mergeOptionResultDataList, improveOptionValue, equipName, equipSprite,
                equipTypeSprite, level, gradeColor, true);
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
        
        private void OnSelectMergeResultEquipItem(Equipment targetEquipment)
        {
            (Sprite sprite, Sprite equipTypeSprite, Color gradeColor, int level) =
                GetTargetEquipmentResource(targetEquipment);
            
            bool activeEquipMergeCostFirstItem =
                targetEquipment.EquipmentData.MergeEquipmentType1 != MergeEquipmentType.None;
            bool activeEquipMergeCostSecondItem =
                targetEquipment.EquipmentData.MergeEquipmentType2 != MergeEquipmentType.None;

            Debug.Log($"{targetEquipment.UID} / {targetEquipment.DataId} / {activeEquipMergeCostFirstItem} / {activeEquipMergeCostSecondItem}");
            _mergePopup.UpdateMergeResultEquipItem(true, activeEquipMergeCostFirstItem, activeEquipMergeCostSecondItem,
                sprite, equipTypeSprite, level, gradeColor);
            _selectedResultMergeEquipment = targetEquipment;

            if (_inventoryEquipItemDict.TryGetValue(targetEquipment, out UI_EquipItem uiEquipItem))
            {
                uiEquipItem.Release();
                _inventoryEquipItemDict.Remove(targetEquipment);
            }
            
            foreach (var (equipment, equipItem) in _inventoryEquipItemDict)
            {
                equipItem.SetLock(!targetEquipment.IsPossibleMerge(equipment));
            }
        }
        
        
    }
}