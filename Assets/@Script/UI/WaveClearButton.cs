using System;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    public class WaveClearButton : MonoBehaviour
    {
        public WaveClearType WaveClearType => _waveClearType;
        
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _unlockObject;
        [SerializeField] private GameObject _getRewardObject;
        [SerializeField] private GameObject _redDotObject;
        [SerializeField] private WaveClearType _waveClearType;

        public void AddListener(Action<WaveClearType> onGetRewardAction)
        {
            _button.SafeAddButtonListener(() => onGetRewardAction?.Invoke(_waveClearType));
        }
        
        public void UpdateUI(bool isClearWave, bool isGetReward)
        {
            _unlockObject.SetActive(!isClearWave);
            _getRewardObject.SetActive(isGetReward && isClearWave);
            _redDotObject.SetActive(isClearWave && !isGetReward);
        }

        private void Start()
        {
            // UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            // StageInfo lastStage = userModel.GetLastStage();
            // DataManager dataManager = GameManager.I.Data;
            // if(!dataManager.StageDict.TryGetValue(userModel.GetLastClearStageIndex(), out var stageData))
            // {
            //     stageData = dataManager.StageDict[1];
            //     Debug.LogWarning($"Failed get last clear stage {userModel.GetLastClearStageIndex()}");
            // }
            //
            // WaveInfo waveInfoData = null;
            // switch (_waveClearType)
            // {
            //     case WaveClearType.FirstWaveClear:
            //         waveInfoData = lastStage.GetWaveInfo(stageData.FirstWaveCountValue);
            //         break;
            //     case WaveClearType.SecondWaveClear:
            //         waveInfoData = lastStage.GetWaveInfo(stageData.SecondWaveCountValue);
            //         break;
            //     case WaveClearType.ThirdWaveClear:
            //         waveInfoData = lastStage.GetWaveInfo(stageData.ThirdWaveCountValue);
            //         break;
            // }
            //
            // bool isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            // bool isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            //
            // _unlockObject.SetActive(!isClear);
            // _getRewardObject.SetActive(isGetReward && isClear);
            // _redDotObject.SetActive(isClear && !isGetReward);
        }
    }
}