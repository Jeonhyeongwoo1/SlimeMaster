using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class WaveClearButton : MonoBehaviour
    {
        enum WaveClearType
        {
            FirstWaveClear,
            SecondWaveClear,
            ThirdWaveClear
        }
        
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _unlockObject;
        [SerializeField] private GameObject _getRewardObject;
        [SerializeField] private GameObject _redDotObject;
        [SerializeField] private WaveClearType _waveClearType;

        private void Awake()
        {
            _button.SafeAddButtonListener(OnClickButton);
        }

        private void OnClickButton()
        {
            Debug.Log("OnClickButton " + _waveClearType);   
        }

        private void Start()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            StageInfo lastStage = userModel.GetLastStage();
            DataManager dataManager = GameManager.I.Data;
            if(!dataManager.StageDict.TryGetValue(userModel.GetLastClearStageIndex(), out var stageData))
            {
                stageData = dataManager.StageDict[1];
                Debug.LogWarning($"Failed get last clear stage {userModel.GetLastClearStageIndex()}");
            }
            
            userModel.AddStage(lastStage);

            WaveInfo waveInfoData = null;
            switch (_waveClearType)
            {
                case WaveClearType.FirstWaveClear:
                    waveInfoData = lastStage.GetWaveInfo(stageData.FirstWaveCountValue);
                    break;
                case WaveClearType.SecondWaveClear:
                    waveInfoData = lastStage.GetWaveInfo(stageData.SecondWaveCountValue);
                    break;
                case WaveClearType.ThirdWaveClear:
                    waveInfoData = lastStage.GetWaveInfo(stageData.ThirdWaveCountValue);
                    break;
            }
            
            bool isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            bool isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            
            _unlockObject.SetActive(!isClear);
            _getRewardObject.SetActive(isGetReward && isClear);
            _redDotObject.SetActive(isClear && !isGetReward);
        }
    }
}