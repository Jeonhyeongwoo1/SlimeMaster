using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.Data;
using SlimeMaster.Popup;
using SlimeMaster.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_BattlePopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _currentStageNameText;
        [SerializeField] private TextMeshProUGUI _clearWaveIndexText;
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private TextMeshProUGUI _gameStartCostText;
        [SerializeField] private TextMeshProUGUI[] _waveIndexTextArray;
        [SerializeField] private Slider _waveSlider;
        
        public void SetInfo(List<WaveClearData> currentWaveClearDataList, string currentStageName,
            string clearWaveIndex, Action gameStartAction)
        {
            _currentStageNameText.text = $"{currentStageName} Stage";
            _clearWaveIndexText.text = clearWaveIndex;

            _gameStartCostText.text = Const.GAME_START_STAMINA_COUNT.ToString();
            SafeButtonAddListener(ref _gameStartButton, gameStartAction.Invoke);

            _waveSlider.value = 0;
            for (var i = 0; i < _waveIndexTextArray.Length; i++)
            {
                TextMeshProUGUI waveIndexText = _waveIndexTextArray[i];
                WaveClearData waveClearData = currentWaveClearDataList[i];
                waveIndexText.text = waveClearData.waveIndex.ToString();
                if (waveClearData.isClear)
                {
                    _waveSlider.value = waveClearData.waveIndex;
                }
            }
        }
    }
}