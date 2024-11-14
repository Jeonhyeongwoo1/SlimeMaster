using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Manager;
using SlimeMaster.OutGame.Popup;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    [Serializable]
    public class WaveClearData
    {
        public int waveIndex;
        public bool isClear;
        public bool isGetReward;
    }
    
    public class BattlePopupPresenter : BasePresenter
    {
        private UI_BattlePopup _battlePopup;
        private UserModel _userModel;
        private DataManager _dataManager;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            _dataManager = GameManager.I.Data;
        }

        public void OpenBattlePopup()
        {
            _battlePopup = GameManager.I.UI.OpenPopup<UI_BattlePopup>();

            StageInfo lastStage = _userModel.GetLastStage();
            List<WaveClearData> waveClearDataList = new(Const.WAVE_COUNT);
            if(!_dataManager.StageDict.TryGetValue(_userModel.GetLastClearStageIndex(), out var stageData))
            {
                stageData = _dataManager.StageDict[1];
                Debug.LogWarning($"Failed get last clear stage {_userModel.GetLastClearStageIndex()}");
            }
            
            // _userModel.AddStage(lastStage);

            WaveClearData waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.FirstWaveCountValue;
            WaveInfo waveInfoData = lastStage.GetWaveInfo(stageData.FirstWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);
            
            waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.SecondWaveCountValue;
            waveInfoData = lastStage.GetWaveInfo(stageData.SecondWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);
            
            waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.ThirdWaveCountValue;
            waveInfoData = lastStage.GetWaveInfo(stageData.ThirdWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);

            int lastStageIndex = _userModel.GetLastClearStageIndex();
            int clearWaveIndex = lastStage.GetLastClearWaveIndex();
            _battlePopup.SetInfo(waveClearDataList, lastStageIndex.ToString(),
                clearWaveIndex.ToString(), OnGameStart);
        }

        private void OnGetWaveClearReward(int waveIndex)
        {
            
        }
        
        private void OnGameStart()
        {
            GameManager.I.StartGame();
        }
    }
}