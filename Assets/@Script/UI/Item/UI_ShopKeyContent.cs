using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class UI_ShopKeyContent : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _productImage;
        [SerializeField] private Image _productCostImage;
        [SerializeField] private TextMeshProUGUI _productCostValueText;
        [SerializeField] private TextMeshProUGUI _productTitleText;
        [SerializeField] private GameObject _soldoutObject;
        [SerializeField] private GameObject _redDotObject;

        private Action _onPurchaseItemAction;

        public void UpdateUI(Sprite productSprite, Sprite productCostSprite, string productCostValue,
            string productTitleValue, bool isShowSoldOut, bool isShowRedDot, Action onPurchaseItemAction)
        {
            _productImage.sprite = productSprite;
            _productCostImage.sprite = productCostSprite;
            _productCostValueText.text = productCostValue;
            _productTitleText.text = productTitleValue;

            if (_soldoutObject)
            {
                _soldoutObject.SetActive(isShowSoldOut);
            }

            if (_redDotObject)
            {
                _redDotObject.SetActive(isShowRedDot);
            }

            _onPurchaseItemAction = onPurchaseItemAction;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onPurchaseItemAction.Invoke();
        }
    }
}