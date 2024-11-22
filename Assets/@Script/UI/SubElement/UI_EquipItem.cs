using System;
using SlimeMaster.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_EquipItem : UI_SubItemElement, IPointerClickHandler
    {
        [SerializeField] private GameObject _getEffectObject;
        [SerializeField] private Image _equipmentGradeBackgroundImage;
        [SerializeField] private Image _equipImage;
        [SerializeField] private Image _specialEquipImage;
        [SerializeField] private Image _equipmentTypeBackgroundImage;
        [SerializeField] private Image _equipmentTypeImage;
        [SerializeField] private TextMeshProUGUI _equipmentLevelValueText;
        [SerializeField] private Image _equipmentEnforceBackgroundImage;
        [SerializeField] private GameObject _newTextObject;
        [SerializeField] private GameObject _redDotObject;
        [SerializeField] private GameObject _equippedObject;
        [SerializeField] private GameObject _selectObject;
        [SerializeField] private GameObject _lockObject;

        private Action _onClickEquipItemAction;
        private bool _isLock = false;
        private bool _isEquipped = false;

        public void AddListener(Action onClickEquipItemAction)
        {
            _onClickEquipItemAction = onClickEquipItemAction;
        }

        public override void Release()
        {
            base.Release();
            _isLock = false;
            _isEquipped = false;
            _onClickEquipItemAction = null;
        }

        public void UpdateUI(Sprite equipSprite, Sprite equipTypeSprite, bool isNewItem, bool isShowRedDot, bool isEquippedObject,
            bool isSelectedObject, bool isLockedObject, int level, Color gradeColor)
        {
            _equipImage.sprite = equipSprite;
            _equipmentTypeImage.sprite = equipTypeSprite;

            if (_newTextObject)
            {
                _newTextObject.SetActive(isNewItem);
            }

            if (_getEffectObject)
            {
                _getEffectObject.SetActive(isNewItem);
            }

            if (_redDotObject)
            {
                _redDotObject.SetActive(isShowRedDot);
            }

            if (_equippedObject)
            {
                _equippedObject.SetActive(isEquippedObject);
            }

            if (_selectObject)
            {
                _selectObject.SetActive(isSelectedObject);
            }

            if (_lockObject)
            {
                _lockObject.SetActive(isLockedObject);
            }
            
            _equipmentLevelValueText.text = $"LV.{level}";
            _equipmentGradeBackgroundImage.color = gradeColor;
            _equipmentTypeBackgroundImage.color = gradeColor;

            if (isEquippedObject)
            {
                _isEquipped = true;
            }

            if (isLockedObject)
            {
                _isLock = true;
            }
            
            gameObject.SetActive(true);
        }

        public void SetLock(bool isLock)
        {
            _isLock = isLock;
            _lockObject.SetActive(isLock);
        }

        private void OnDisable()
        {
            _onClickEquipItemAction = null;
            GameManager.I.Pool.ReleaseObject(gameObject.name, gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isLock || _isEquipped)
            {
                return;
            }
            
            _onClickEquipItemAction?.Invoke();
        }
    }
}