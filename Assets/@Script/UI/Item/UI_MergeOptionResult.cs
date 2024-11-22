using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.Presenter;
using TMPro;
using UnityEngine;

namespace SlimeMaster.OutGame.UI
{
    public class UI_MergeOptionResult : MonoBehaviour
    {
        [Serializable]
        public struct OptionElement
        {
            public EquipAbilityStatType equipAbilityStatType;
            public GameObject improveObject;
            public TextMeshProUGUI beforeValueText;
            public TextMeshProUGUI afterValueText;
        }

        [SerializeField] private TextMeshProUGUI _equipNameText;
        [SerializeField] private TextMeshProUGUI _improvOptionValueText;
        [SerializeField] private List<OptionElement> _optionElementList;
        [SerializeField] private GameObject _mergeImproveOptionObject;
        [SerializeField] private GameObject _selectEquipmentCommentObject;
        [SerializeField] private GameObject _selectMergeCommentObject;

        public void ActiveMergeImproveOptionObject(bool active)
        {
            _mergeImproveOptionObject.SetActive(active);
            if (active)
            {
                _selectMergeCommentObject.SetActive(false);
                _selectEquipmentCommentObject.SetActive(false);
            }
        }

        public void ShowCommentObject(bool isSelectedEquipItem)
        {
            _mergeImproveOptionObject.SetActive(false);
            _selectMergeCommentObject.SetActive(isSelectedEquipItem);
            _selectEquipmentCommentObject.SetActive(!isSelectedEquipItem);
        }
        
        public void UpdateMergeOption(List<MergeOptionResultData> mergeOptionResultDataList, string improveOptionValue, string equipName)
        {
            _improvOptionValueText.text = improveOptionValue;

            if (!string.IsNullOrEmpty(equipName))
            {
                _equipNameText.text = equipName;
            }
            
            foreach (OptionElement element in _optionElementList)
            {
                MergeOptionResultData optionResultData =
                    mergeOptionResultDataList.Find(v => v.equipAbilityStatType == element.equipAbilityStatType);
                if (optionResultData == null)
                {
                    element.improveObject.SetActive(false);
                    continue;
                }

                element.beforeValueText.text = optionResultData.beforeValue.ToString();
                element.afterValueText.text = optionResultData.afterValue.ToString();
            }
        }
    }
}