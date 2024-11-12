using System.Collections;
using System.Collections.Generic;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.InGame.Item
{
    public class UI_MaterialItem : UI_SubItemElement
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _countText;

        public void UpdateUI(Sprite sprite, string count)
        {
            _itemImage.sprite = sprite;
            _countText.text = count;
            gameObject.SetActive(true);
        }
    }
}