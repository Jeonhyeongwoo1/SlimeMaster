using System;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.OutGame.UI;
using SlimeMaster.Popup;
using SlimeMaster.Presenter;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_MergePopup : BasePopup
    {
        public Transform EquipInventoryScrollContentObject => _equipInventoryScrollContentObject;
        
        [SerializeField] private Transform _equipInventoryScrollContentObject;
        [SerializeField] private Button _closeButton;
        [SerializeField] private UI_EquipMergeResultItem _equipMergeResultItem;
        [SerializeField] private UI_EquipMergeCostItem _equipMergeCostFirstItem;
        [SerializeField] private UI_EquipMergeCostItem _equipMergeCostSecondItem;
        [SerializeField] private Button _mergeButton;
        [SerializeField] private Button _mergeAllButton;
        [SerializeField] private Button _sortButton;
        [SerializeField] private TextMeshProUGUI _sortTypeText;

        public Action onReleaseSelectedEquipment;
        public Action<bool> onReleaseSelectedCostEquipment;
        public Action onClosePopupAction;
        public Action onMergeAction;
        public Action onAllMergeAction;
        public Action onSortEquipItemAction;
        
        public override void AddEvents()
        {
            base.AddEvents();
            
            _mergeButton.SafeAddButtonListener(onMergeAction.Invoke);
            _closeButton.SafeAddButtonListener(onClosePopupAction.Invoke);
            _mergeAllButton.SafeAddButtonListener(onAllMergeAction.Invoke);
            _sortButton.SafeAddButtonListener(onSortEquipItemAction.Invoke);
            _equipMergeResultItem.AddEvents(onReleaseSelectedEquipment.Invoke);
            _equipMergeCostFirstItem.AddEvents(() => onReleaseSelectedCostEquipment.Invoke(true));
            _equipMergeCostSecondItem.AddEvents(() => onReleaseSelectedCostEquipment.Invoke(false));
        }

        public void SetSortTypeText(string sortType)
        {
            _sortTypeText.text = sortType;
        }

        public void RestEquipMergeCostItem(bool isResetFirstItem, bool isResetSecondItem)
        {
            if (isResetFirstItem)
            {
                _equipMergeCostFirstItem.UpdateUI(false, Color.white, null, 0, null);
            }

            if (isResetSecondItem)
            {
                _equipMergeCostSecondItem.UpdateUI(false, Color.white, null, 0, null);
            }
        }

        public void UpdateEquipMergeCostItem(bool isFirstEquipItem, Color gradeColor, Sprite equipSprite,
            int equipLevel, Sprite equipTypeSprite)
        {
            if (isFirstEquipItem)
            {
                _equipMergeCostFirstItem.UpdateUI(true, gradeColor, equipSprite, equipLevel, equipTypeSprite);
            }
            else
            {
                _equipMergeCostSecondItem.UpdateUI(true, gradeColor, equipSprite, equipLevel, equipTypeSprite);
            }
        }

        public void UpdateMergeResultEquipItem(bool isSelected, bool activeEquipMergeCostFirstItem,
            bool activeEquipMergeCostSecondItem, Sprite equipSprite, Sprite equipTypeSprite, int equipLevel,
            Color gradeColor)
        {
            _equipMergeCostFirstItem.gameObject.SetActive(activeEquipMergeCostFirstItem);
            _equipMergeCostSecondItem.gameObject.SetActive(activeEquipMergeCostSecondItem);
            _equipMergeResultItem.UpdateUI(isSelected, equipSprite, equipTypeSprite, equipLevel, gradeColor, false);
        }

        public void CostEquipItemAllFilled(List<MergeOptionResultData> mergeOptionResultDataList,
            string improveOptionValue, string equipName, Sprite equipSprite, Sprite equipTypeSprite, int equipLevel,
            Color gradeColor, bool isShowStartEffect)
        {
            _equipMergeResultItem.UpdateUI(true, equipSprite, equipTypeSprite, equipLevel, gradeColor,
                isShowStartEffect);
            _equipMergeResultItem.ShowMergeOptionResult(mergeOptionResultDataList, improveOptionValue, equipName);
        }

        public void ReleaseEquipItem()
        {
            var childs = Utils.GetChildComponent<UI_EquipItem>(_equipInventoryScrollContentObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_EquipItem item in childs)
            {
                item.Release();
            }
        }

        public override void ClosePopup()
        {
            base.ClosePopup();
            
            _equipMergeCostFirstItem.UpdateUI(false, Color.white, null, 0, null);
            _equipMergeCostSecondItem.UpdateUI(false, Color.white, null, 0, null);
        }
    }
}