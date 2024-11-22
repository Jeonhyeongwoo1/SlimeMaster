using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_MaterialItem : UI_SubItemElement
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _itemBackgroundImage;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private GameObject _getEffectObject;
        
        public void UpdateUI(Sprite sprite, Color bgColor, string count, bool isGet, Transform parent = null, bool isActive = true)
        {
            _itemImage.sprite = sprite;
            _countText.text = count;
            _itemBackgroundImage.color = bgColor;
            _getEffectObject.SetActive(isGet);

            if (parent)
            {
                transform.SetParent(parent);
            }

            gameObject.SetActive(isActive);
        }
    }
}