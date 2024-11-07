using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Factory;
using SlimeMaster.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class UI_GameStageInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeLimitText;
        [SerializeField] private TextMeshProUGUI _waveStepText;
        [SerializeField] private TextMeshProUGUI _killAmountText;
        [SerializeField] private Button _pauseButton;

        private Action _onPauseAction;
        
        private void Start()
        {
            _pauseButton.onClick.AddListener(()=> _onPauseAction?.Invoke());
        }

        public void Initialize(Action onPauseAction)
        {
            _onPauseAction = onPauseAction;
            
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            stageModel.timer
                .Subscribe(OnChangedStageTimer)
                .AddTo(this);

            stageModel.killCount
                .Subscribe(OnChangedKillCount)
                .AddTo(this);

            stageModel.currentWaveStep
                .Subscribe(OnChangedCurrentWaveStep)
                .AddTo(this);
        }
        
        private void OnChangedCurrentWaveStep(int step)
        {
            _waveStepText.text = step.ToString();
        }

        private void OnChangedKillCount(int killCount)
        {
            _killAmountText.text = killCount.ToString();
        }

        private void OnChangedStageTimer(int time)
        {
            _timeLimitText.text = time.ToString();
        }
        
    }
}