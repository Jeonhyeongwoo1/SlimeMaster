using System;
using SlimeMaster.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_AchievementItem : UI_SubItemElement
    {
        [SerializeField] private Image _rewardItmeIconImage;
        [SerializeField] private TextMeshProUGUI _rewardItemValueText;
        [SerializeField] private TextMeshProUGUI _missionNameValueText;
        [SerializeField] private TextMeshProUGUI _missionProgressValueText;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Button _getButton;
        [SerializeField] private TextMeshProUGUI _completeText;
        [SerializeField] private Slider _progressSliderObject;

        public Action onGetAchievementReward;

        public void UpdateUI(Sprite rewardItemSprite, int rewardItemValue, string missionName, int accumulatedValue,
            int missionTargetValue, bool isCompleted, bool isGetReward, bool isActive)
        {
            _rewardItmeIconImage.sprite = rewardItemSprite;
            _rewardItemValueText.text = $"x{rewardItemValue}";
            _missionNameValueText.text = missionName;
            _missionProgressValueText.text = $"{accumulatedValue}/{missionTargetValue}";
            _progressText.gameObject.SetActive(!isCompleted && !isGetReward);
            _getButton.gameObject.SetActive(isCompleted && !isGetReward);
            _completeText.gameObject.SetActive(isCompleted && isGetReward);
            _progressSliderObject.value = (float)accumulatedValue / missionTargetValue;
         
            _getButton.SafeAddButtonListener(onGetAchievementReward.Invoke);   
            transform.SetAsLastSibling();
            gameObject.SetActive(isActive);
        }
    }
}