using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_FastRewardPopup : BasePopup
    {
        public Transform ItemContainer => _itemContainer;
        
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private Button _adButton;
        [SerializeField] private Button _claimButton;
        [SerializeField] private TextMeshProUGUI _claimCostValueText;
        [SerializeField] private TextMeshProUGUI _remainingCountValueText;

        public Action onClaimAction;
        
        public override void AddEvents()
        {
            base.AddEvents();
            _claimButton.SafeAddButtonListener(onClaimAction.Invoke);
        }

        public void UpdateUI(int claimCostValue, int remainingCountValue)
        {
            _claimCostValueText.text = $"x {claimCostValue}";
            _remainingCountValueText.text = $"{remainingCountValue}";
            
        }
    }
}
