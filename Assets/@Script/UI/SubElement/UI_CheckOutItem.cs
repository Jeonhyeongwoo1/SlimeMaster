using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_CheckOutItem : UI_SubItemElement, IPointerClickHandler
    {
        [SerializeField] private Image _rewardItemBackgroundImage;
        [SerializeField] private Image _rewardItemImage;
        [SerializeField] private TextMeshProUGUI _dayValueText;
        [SerializeField] private TextMeshProUGUI _rewardItemCountValueText;
        [SerializeField] private GameObject _clearRewardCompleteObject;

        public Action onItemClickAction;
        
        public void UpdateUI(Sprite rewardItemSprite, Color rewardItemBGColor, int day,
            string rewardItemCountValue, bool isClear)
        {
            _rewardItemBackgroundImage.sprite = rewardItemSprite;
            _rewardItemImage.color = rewardItemBGColor;
            _dayValueText.text = $"{day} Day";
            _rewardItemCountValueText.text = rewardItemCountValue;
            _clearRewardCompleteObject.SetActive(isClear);
            gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onItemClickAction?.Invoke();
        }
    }
}