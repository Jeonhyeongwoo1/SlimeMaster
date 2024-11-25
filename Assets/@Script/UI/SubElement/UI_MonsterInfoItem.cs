using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_MonsterInfoItem : UI_SubItemElement
    {
        [SerializeField] private Image _monsterImage;
        [SerializeField] private TextMeshProUGUI _monsterLevelValueText;
        [SerializeField] private Button _monsterInfoButton;

        public void UpdateUI(Sprite monsterSprite, int level)
        {
            _monsterImage.sprite = monsterSprite;
            _monsterLevelValueText.text = $"LV. {level}";
            
            gameObject.SetActive(true);
        }
    }
}