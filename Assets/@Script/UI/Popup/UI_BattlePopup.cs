using System;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.OutGame.UI;
using SlimeMaster.Popup;
using SlimeMaster.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_BattlePopup : BasePopup
    {
        [Serializable]
        public struct ButtonElement
        {
            public OutGameContentButtonType contentButtonType;
            public Button button;
            public GameObject redDotObject;
        }
        
        [SerializeField] private TextMeshProUGUI _currentStageNameText;
        [SerializeField] private TextMeshProUGUI _clearWaveIndexText;
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private TextMeshProUGUI _gameStartCostText;
        [SerializeField] private TextMeshProUGUI[] _waveIndexTextArray;
        [SerializeField] private Slider _waveSlider;
        [SerializeField] private List<WaveClearButton> _waveClearButtonList;
        [SerializeField] private List<ButtonElement> _buttonElementList;
        [SerializeField] private Button _stageSelectButton;
        
        public Action onStageSelectAction;
        public Action onGameStartAction;
        public Action<WaveClearType> onGetRewardAction;
        public Action<OutGameContentButtonType> onClickContentButtonType;
        
        public override void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }
            
            base.Initialize();
        }

        public override void AddEvents()
        {
            base.AddEvents();
            
            _stageSelectButton.SafeAddButtonListener(onStageSelectAction.Invoke);
            _gameStartButton.SafeAddButtonListener(onGameStartAction.Invoke);
            _waveClearButtonList.ForEach(v=> v.AddListener(onGetRewardAction));
            _buttonElementList.ForEach(v =>
                v.button.SafeAddButtonListener(() => onClickContentButtonType.Invoke(v.contentButtonType)));
        }

        public void UpdateWaveClearButton(WaveClearType waveClearType, bool isClear, bool isGetReward)
        {
            foreach (WaveClearButton waveClearButton in _waveClearButtonList)
            {
                if (waveClearButton.WaveClearType == waveClearType)
                {
                    waveClearButton.UpdateUI(isClear, isGetReward);
                }
            }
        }

        public void ShowOutGameContentButtonRedDot(OutGameContentButtonType buttonType, bool isActive)
        {
            ButtonElement element = _buttonElementList.Find(v=> v.contentButtonType == buttonType);
            element.redDotObject.SetActive(isActive);
        }

        public void UpdateUI(List<WaveClearData> currentWaveClearDataList, int stageIndex,
            string clearWaveIndex)
        {
            _currentStageNameText.text = $"{stageIndex} Stage";
            _clearWaveIndexText.text = clearWaveIndex;
            _gameStartCostText.text = Const.GAME_START_STAMINA_COUNT.ToString();
            _waveSlider.value = 0;
            
            for (var i = 0; i < _waveIndexTextArray.Length; i++)
            {
                TextMeshProUGUI waveIndexText = _waveIndexTextArray[i];
                WaveClearData waveClearData = currentWaveClearDataList[i];
                WaveClearButton waveClearButton = _waveClearButtonList[i];
                waveIndexText.text = waveClearData.waveIndex.ToString();
                if (waveClearData.isClear)
                {
                    _waveSlider.value = waveClearData.waveIndex;
                }

                waveClearButton.UpdateUI(waveClearData.isClear, waveClearData.isGetReward);
            }
        }
    }
}