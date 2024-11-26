using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

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
            _dataManager = GameManager.I.Data;
            _userModel = userModel;
            GameManager.I.Event.AddEvent(GameEventType.MoveToTap, OnMoveToTap);
            GameManager.I.Event.AddEvent(GameEventType.ChangeStage, OnChangeStage);
        }

        private void OnChangeStage(object value)
        {
            RefreshUI();
        }

        private void OnMoveToTap(object value)
        {
            ToggleType toggleType = (ToggleType)value;
            if (toggleType == ToggleType.BattleToggle)
            {
                OpenBattlePopup();
            }
        }

        private async void OnGetReward(WaveClearType waveClearType)
        {
            int lastStageIndex = _userModel.GetLastClearStageIndex();

            var response = await ServerHandlerFactory.Get<IUserClientSender>()
                .GetWaveClearRewardRequest(lastStageIndex, waveClearType);

            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        Debug.Log("response error :" + response.errorMessage);
                        //Alert
                        return;
                }
            }
            
            long getRewardValue = 0;
            DBItemData item = response.DBItemData;
            Dictionary<int, ItemData> itemDataDict = _userModel.ItemDataDict.Value;
            if(itemDataDict.TryGetValue(item.ItemId, out ItemData value))
            {
                getRewardValue = item.ItemValue - value.ItemValue.Value;
                value.ItemValue.Value = item.ItemValue;
            }
            else
            {
                getRewardValue = item.ItemValue;
                ItemData itemData = new(item.ItemId, item.ItemValue);
                itemDataDict.Add(item.ItemId, itemData);
            }

            var waveData = response.DBStageData.WaveDataList.FindLast(v => v.IsGet);
            _userModel.UpdateStage(response.DBStageData.StageIndex, waveData.WaveIndex);
            
            var rewardItemDataList = new List<RewardItemData>(1);
            RewardItemData rewardItemData = new RewardItemData
            {
                materialItemId = item.ItemId,
                rewardValue = (int) getRewardValue
            };
            
            rewardItemDataList.Add(rewardItemData);
            GameManager.I.Event.Raise(GameEventType.GetReward, rewardItemDataList);
        }

        public void OpenBattlePopup()
        {
            _battlePopup = GameManager.I.UI.OpenPopup<UI_BattlePopup>();
            _battlePopup.onGameStartAction = OnGameStart;
            _battlePopup.onGetRewardAction = OnGetReward;
            _battlePopup.onStageSelectAction = OnStageSelect;
            _battlePopup.onClickContentButtonType = OnClickContentButtonType;
            _battlePopup.AddEvents();

            var checkoutModel = ModelFactory.CreateOrGetModel<CheckoutModel>();
            checkoutModel.IsPossibleGetReward.Subscribe(x =>
                _battlePopup.ShowOutGameContentButtonRedDot(OutGameContentButtonType.Checkout, x))
                .AddTo(_battlePopup);

            var missionModel = ModelFactory.CreateOrGetModel<MissionModel>();
            missionModel.IsPossibleGetReward
                .Subscribe(x => _battlePopup.ShowOutGameContentButtonRedDot(OutGameContentButtonType.Mission, x))
                .AddTo(_battlePopup);

            var achievementModel = ModelFactory.CreateOrGetModel<AchievementModel>();
            achievementModel.IsPossibleGetReward
                .Subscribe(x => _battlePopup.ShowOutGameContentButtonRedDot(OutGameContentButtonType.Achievement, x))
                .AddTo(_battlePopup);
            
            RefreshUI();
        }

        private void RefreshUI()
        {
            int currentStageIndex = GameManager.I.CurrentStageIndex;
            StageInfo stageInfo = _userModel.GetStageInfo(currentStageIndex);
            stageInfo.WaveInfoList.Value.ForEach(v =>
            {
                v.IsGet.Subscribe(x =>
                {
                    _battlePopup.UpdateWaveClearButton(v.WaveClearType, v.IsClear.Value, v.IsGet.Value);
                }).AddTo(_battlePopup);
            });
            
            List<WaveClearData> waveClearDataList = new(Const.WAVE_COUNT);
            if(!_dataManager.StageDict.TryGetValue(currentStageIndex, out var stageData))
            {
                stageData = _dataManager.StageDict[1];
                Debug.LogWarning($"Failed get last clear stage {currentStageIndex}");
            }
            
            WaveClearData waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.FirstWaveCountValue;
            WaveInfo waveInfoData = stageInfo.GetWaveInfo(stageData.FirstWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);
            
            waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.SecondWaveCountValue;
            waveInfoData = stageInfo.GetWaveInfo(stageData.SecondWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);
            
            waveClearData = new WaveClearData();
            waveClearData.waveIndex = stageData.ThirdWaveCountValue;
            waveInfoData = stageInfo.GetWaveInfo(stageData.ThirdWaveCountValue);
            waveClearData.isClear = waveInfoData != null && waveInfoData.IsClear.Value;
            waveClearData.isGetReward = waveInfoData != null && waveInfoData.IsGet.Value;
            waveClearDataList.Add(waveClearData);

            int clearWaveIndex = stageInfo.GetLastClearWaveIndex();
            _battlePopup.UpdateUI(waveClearDataList, currentStageIndex, clearWaveIndex.ToString());
            
            var timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
            bool isPossibleGetReward = timeDataModel.IsPossibleGetReward(_userModel.LastOfflineGetRewardTime.Value);
            _battlePopup.ShowOutGameContentButtonRedDot(OutGameContentButtonType.OfflineReward, isPossibleGetReward);
        }

        private void OnStageSelect()
        {
            GameManager.I.Event.Raise(GameEventType.ShowStageSelectPopup);
        }
        
        private async void OnGameStart()
        {
            ItemData itemData = _userModel.ItemDataDict.Value[Const.ID_STAMINA];
            if (itemData.ItemValue.Value <= Const.GAME_START_STAMINA_COUNT)
            {
                Debug.Log("Not enough stamina count : " + itemData.ItemValue.Value);
                return;
            }

            var response = await ServerHandlerFactory.Get<IUserClientSender>()
                .UseStaminaRequest(Const.GAME_START_STAMINA_COUNT);
            switch (response.responseCode)
            {
                case ServerErrorCode.NotEnoughStamina:
                    //Alert
                    Debug.Log("NotEnoughStamina :" +
                              response.DBUserData.ItemDataDict[Const.ID_STAMINA.ToString()].ItemValue);
                    return;
                case ServerErrorCode.FailedFirebaseError:
                case ServerErrorCode.FailedGetUserData:
                    //alert
                    SceneManager.LoadScene(SceneType.TitleScene.ToString());
                    return;
            }

            DBItemData staminaItemData = response.DBUserData.ItemDataDict[Const.ID_STAMINA.ToString()];
            _userModel.AddItemValue(staminaItemData.ItemId, staminaItemData.ItemValue);
            GameManager.I.StartGame();
        }

        private void OnClickContentButtonType(OutGameContentButtonType buttonType)
        {
            GameManager.I.Event.Raise(GameEventType.ShowOutGameContentPopup, buttonType);
        }
    }
}