using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_CheckOutItem : UI_SubItemElement
    {
        [SerializeField] private Image _rewardItemBackgroundImage;
        [SerializeField] private Image _rewardItemImage;
        [SerializeField] private TextMeshProUGUI _dayValueText;
        [SerializeField] private TextMeshProUGUI _rewardItemCountValueText;
        [SerializeField] private GameObject _clearRewardCompleteObject;

        public void UpdateUI(Sprite rewardItemSprite, Sprite rewardItemBGSprite, string day, string rewardItemCountValue, bool isClear)
        {
            _rewardItemBackgroundImage.sprite = rewardItemSprite;
            _rewardItemImage.sprite = rewardItemBGSprite;
            _dayValueText.text = day;
            _rewardItemCountValueText.text = rewardItemCountValue;
            _clearRewardCompleteObject.SetActive(isClear);
        }
    }
}