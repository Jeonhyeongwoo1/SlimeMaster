using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class UI_EquipMergeCostItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _costEquipSelectObject;
        [SerializeField] private Image _selectEquipGradeBackgroundImage;
        [SerializeField] private Image _costEquipImage;
        [SerializeField] private TextMeshProUGUI _selectEquipLevelValueText;
        [SerializeField] private GameObject _selectEquipEnforceBackgroundImage;
        [SerializeField] private TextMeshProUGUI _selectEquipEnforceValueText;
        [SerializeField] private Image _selectEquipTypeBackgroundImage;
        [SerializeField] private Image _selectEquipTypeImage;

        private Action _onReleaseSelectedCostEquipment;
        
        public void AddEvents(Action onReleaseSelectedCostEquipment)
        {
            _onReleaseSelectedCostEquipment = onReleaseSelectedCostEquipment;
        }
        
        public void UpdateUI(bool isActive, Color gradeColor, Sprite equipSprite, int equipLevel, Sprite equipTypeSprite)
        {
            _costEquipSelectObject.SetActive(isActive);
            if (!isActive)
            {
                return;
            }
            
            _selectEquipGradeBackgroundImage.color = gradeColor;
            _costEquipImage.sprite = equipSprite;
            _selectEquipLevelValueText.text = $"LV.{equipLevel}";
            _selectEquipEnforceBackgroundImage.gameObject.SetActive(false);
            _selectEquipTypeBackgroundImage.color = gradeColor;
            _selectEquipTypeImage.sprite = equipTypeSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onReleaseSelectedCostEquipment.Invoke();   
        }
    }
}