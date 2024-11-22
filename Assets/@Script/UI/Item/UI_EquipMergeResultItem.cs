using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class UI_EquipMergeResultItem : MonoBehaviour
    {
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Image _equipImage;
        [SerializeField] private GameObject _mergeStartEffectObject;
        [SerializeField] private GameObject _mergeFinishEffectObject;
        [SerializeField] private Image _selectedEquipGradeBackgroundImage;
        [SerializeField] private Image _selectedEquipTypeBackgroundImage;
        [SerializeField] private Image _selectedEquipTypeImage;
        [SerializeField] private TextMeshProUGUI _selectedEquipLevelValueText;
        [SerializeField] private GameObject _selectedEquipEnforceObject;
        [SerializeField] private TextMeshProUGUI _selectedEquipEnforceValueText;

        [SerializeField] private GameObject _selectedEquipObject;
        [SerializeField] private GameObject _mergePossibleOutlineImage;
        [SerializeField] private UI_MergeOptionResult _mergeOptionResult;
        
        //합성, 재료, 스탯 추가

        public void AddEvents(Action onReleaseSelectedEquipment)
        {
            _unequipButton.SafeAddButtonListener(onReleaseSelectedEquipment.Invoke);
        }

        public void ShowMergeOptionResult(List<MergeOptionResultData> mergeOptionResultDataList, string improveOptionValue, string equipName)
        {
            _mergeOptionResult.ActiveMergeImproveOptionObject(true);
            _mergeOptionResult.UpdateMergeOption(mergeOptionResultDataList, improveOptionValue, equipName);
        }

        public void UpdateUI(bool isSelectedEquipMergeResultItem, Sprite equipSprite, Sprite equipTypeSprite, int equipLevel, Color gradeColor, bool isShowStartEffect)
        {
            _selectedEquipObject.SetActive(isSelectedEquipMergeResultItem);
            _mergePossibleOutlineImage.SetActive(isSelectedEquipMergeResultItem);
            _mergeOptionResult.ShowCommentObject(isSelectedEquipMergeResultItem);
            
            if (!isSelectedEquipMergeResultItem)
            {
                return;
            }

            _mergeStartEffectObject.SetActive(isShowStartEffect);
            _equipImage.sprite = equipSprite;
            _selectedEquipTypeBackgroundImage.color = gradeColor;
            _selectedEquipGradeBackgroundImage.color = gradeColor;
            _selectedEquipTypeImage.sprite = equipTypeSprite;
            _selectedEquipLevelValueText.text = $"LV.{equipLevel}";
            _selectedEquipEnforceObject.SetActive(false);
        }
    }
}