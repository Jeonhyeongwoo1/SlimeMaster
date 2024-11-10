using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_SkillDamageItem : MonoBehaviour
    {
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDamageText;
        [SerializeField] private Slider _skillDamagePercentSlider;
        [SerializeField] private TextMeshProUGUI _skillDamagePercentText;

        public void UpdateUI(float percent, string damage, string skillName, Sprite skillIcon)
        {
            _skillImage.sprite = skillIcon;
            _skillNameText.text = skillName;
            _skillDamageText.text = damage;
            _skillDamagePercentSlider.value = percent;
            _skillDamagePercentText.text = $"{(percent * 100):0.00}%";
        }
    }
}