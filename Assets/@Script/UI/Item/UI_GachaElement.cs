using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class UI_GachaElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _gachaCostImage;
        [SerializeField] private TextMeshProUGUI _gachaCostText;

        private Action _onPurchaseAction;
        
        public void UpdateUI(Sprite gachaCostSprite, string gachaCostValue, Action onPurchaseAction)
        {
            if (_gachaCostImage)
            {
                _gachaCostImage.sprite = gachaCostSprite;
            }

            if (_gachaCostText)
            {
                _gachaCostText.text = gachaCostValue;
            }

            _onPurchaseAction = onPurchaseAction;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onPurchaseAction.Invoke();
        }
    }
}
