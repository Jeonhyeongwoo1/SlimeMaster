using System;
using System.Collections.Generic;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils = SlimeMaster.Common.Utils;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_CheckOutPopup : BasePopup
    {
        [Serializable]
        public struct CheckoutClearElement
        {
            public UI_CheckOutItem checkOutItem;
            public TextMeshProUGUI checkoutClearDayText;
        }
        
        public int GroupCount => _checkoutClearElementList.Count;
        public Transform CheckOutBoardObject => _checkOutBoardObject;
     
        [SerializeField] private Transform _checkOutBoardObject;
        [SerializeField] private List<CheckoutClearElement> _checkoutClearElementList;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _totalAttendanceDaysText;
        
        public void ReleaseCheckoutItem()
        {
            var childs = Utils.GetChildComponent<UI_CheckOutItem>(_checkOutBoardObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_CheckOutItem item in childs)
            {
                item.Release();
            }
        }

        public void UpdateUI(int progress)
        {
            _progressSlider.value = progress;
            _totalAttendanceDaysText.text = $"{progress} Day";
        }
        
        public void UpdateCheckOutClear(int index, Sprite rewardItemSprite, Color rewardItemBGColor, int day,
            string rewardItemCountValue, bool isClear, Action onItemClickAction)
        {
            if (index == _checkoutClearElementList.Count)
            {
                Debug.LogError($"out of range {index} / {_checkoutClearElementList.Count}");
                return;
            }

            _checkoutClearElementList[index].checkOutItem.onItemClickAction = onItemClickAction;
            _checkoutClearElementList[index].checkOutItem.UpdateUI(rewardItemSprite, rewardItemBGColor, day, rewardItemCountValue, isClear);
            _checkoutClearElementList[index].checkoutClearDayText.text = $"{day}";
        }
    }
}