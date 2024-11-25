using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.UISubItemElement
{
    public class UI_StageInfoItem : UI_SubItemElement
    {
        [Serializable]
        public struct StageRewardElement
        {
            public GameObject rewardCompleteObject;
            public GameObject rewardLockObject;
        }
        
        [SerializeField] private TextMeshProUGUI _stageValueText;
        [SerializeField] private Image _stageImage;
        [SerializeField] private GameObject _stageLockImageObject;
        [SerializeField] private TextMeshProUGUI _maxWaveValueText;
        [SerializeField] private List<StageRewardElement> _stageRewardElementList;

        public void UpdateUI(int stageIndex, Sprite stageSprite, bool isOpenedStage, int aliveStageWave, List<bool> stageCompletedList)
        {
            _stageValueText.text = $"{stageIndex} STAGE";
            _stageImage.sprite = stageSprite;
            _stageImage.color = isOpenedStage ? Color.gray : Color.white;
            _stageLockImageObject.SetActive(isOpenedStage);
            _maxWaveValueText.text = aliveStageWave.ToString();
            
            transform.SetAsLastSibling();
            for (var i = 0; i < stageCompletedList.Count; i++)
            {
                StageRewardElement element = _stageRewardElementList[i];
                element.rewardCompleteObject.SetActive(stageCompletedList[i]);
                element.rewardLockObject.SetActive(isOpenedStage);
            }
            
            gameObject.SetActive(true);
        }

    }
}
