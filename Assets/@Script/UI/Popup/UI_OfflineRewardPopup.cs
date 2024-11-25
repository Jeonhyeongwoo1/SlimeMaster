using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_OfflineRewardPopup : BasePopup
    {
        public Transform RewardItemScrollContentObject => _rewardItemScrollContentObject;
        
        [SerializeField] private TextMeshProUGUI _totalTimeValueText;
        [SerializeField] private TextMeshProUGUI _resultGoldValueText;
        [SerializeField] private Transform _rewardItemScrollContentObject;
        [SerializeField] private Button _fastRewardButton;
        [SerializeField] private Button _claimButton;
        [SerializeField] private GameObject _goldParticle;

        public Action onOpenFastRewardAction;
        public Action onClaimAction;
        
        public override void AddEvents()
        {
            base.AddEvents();
            _fastRewardButton.SafeAddButtonListener(onOpenFastRewardAction.Invoke);
            _claimButton.SafeAddButtonListener(onClaimAction.Invoke);
        }

        public void UpdateUI(TimeSpan offlineTime, string gold, bool isPossibleReward, bool isPossibleFastReward)
        {
            _totalTimeValueText.text = $"{offlineTime:hh\\:mm\\:ss}";
            _resultGoldValueText.text = $"{gold}/{Const.HourPerName}";
            _claimButton.image.color = isPossibleReward ? Utils.HexToColor("50D500") : Utils.HexToColor("5A5A5A");
            _claimButton.interactable = isPossibleReward;
            _fastRewardButton.image.color = isPossibleFastReward ? Color.white : Utils.HexToColor("5A5A5A");
            _fastRewardButton.interactable = isPossibleFastReward;
            _goldParticle.SetActive(isPossibleReward);
        }

    }
}