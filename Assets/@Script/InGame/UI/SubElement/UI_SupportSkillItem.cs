using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_SupportSkillItem : MonoBehaviour
    {
        [SerializeField] private Button _supportSkillButton;
        [SerializeField] private Image _supportSkillIconImage;
        [SerializeField] private Image _bgSkillImage;

        private void Start()
        {
            _supportSkillButton.onClick.AddListener(OnShowSupportSkillToggle);
        }

        private void OnShowSupportSkillToggle()
        {
            
        }

        public void SetInfo(SupportSkillData supportSkillData, Transform parent)
        {
            _supportSkillIconImage.sprite = GameManager.I.Resource.Load<Sprite>(supportSkillData.IconLabel);
            
            Color color = _bgSkillImage.color;
            switch (supportSkillData.SupportSkillGrade)
            {
                case SupportSkillGrade.Common:
                    color = Const.EquipmentUIColors.Common;
                    break;
                case SupportSkillGrade.Uncommon:
                    color = Const.EquipmentUIColors.Uncommon;
                    break;
                case SupportSkillGrade.Rare:
                    color = Const.EquipmentUIColors.Rare;
                    break;
                case SupportSkillGrade.Epic:
                    color = Const.EquipmentUIColors.Epic;
                    break;
                case SupportSkillGrade.Legend:
                    color = Const.EquipmentUIColors.Legendary;
                    break;
            }
            
            _bgSkillImage.color = color;
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void Release()
        {
            GameManager.I.Pool.ReleaseObject(nameof(UI_SupportSkillItem), gameObject);
        }
    }
}