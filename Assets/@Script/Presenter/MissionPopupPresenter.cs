using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Interface;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class MissionPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private MissionModel _missionModel;
        private UI_MissionPopup _popup;
        private UIManager _uiManager = GameManager.I.UI;
        private DataManager _dataManager = GameManager.I.Data;
        
        public void Initialize(UserModel userModel, MissionModel missionModel)
        {
            _userModel = userModel;
            _missionModel = missionModel;
            
            GameManager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenCheckoutPopup);
        }

        private void OnOpenCheckoutPopup(object value)
        {
            var buttonType = (OutGameContentButtonType)value;
            if (buttonType != OutGameContentButtonType.Mission)
            {
                return;
            }

            _popup = _uiManager.OpenPopup<UI_MissionPopup>();
            RefreshMissionPopup();
        }

        private void RefreshMissionPopup()
        {
            _popup.ReleaseSubItem<UI_MissionItem>(_popup.DailyMissionScrollObject);

            ResourcesManager resourcesManager = GameManager.I.Resource;
            foreach (var (key, missionData) in _dataManager.MissionDataDict)
            {
                MissionModelData missionModelData = _missionModel.GetMissionData(missionData.MissionId);
                bool isGet = missionModelData.isGet.Value;
                bool isCompleted = missionModelData.accumulatedValue.Value >= missionData.MissionTargetValue;
                MaterialData materialData = _dataManager.MaterialDataDict[missionData.ClearRewardItmeId];
                Sprite rewardItemSprite = resourcesManager.Load<Sprite>(materialData.SpriteName);
                string missionName = missionData.DescriptionTextID;
                var missionItem = _uiManager.AddSubElementItem<UI_MissionItem>(_popup.DailyMissionScrollObject);
                missionItem.onGetMissionReward = () => OnGetMissionReward(missionData.MissionId, missionData.MissionType);
                missionItem.UpdateUI(rewardItemSprite, missionData.RewardValue, missionName,
                    missionModelData.accumulatedValue.Value, missionData.MissionTargetValue, isCompleted, isGet);
            }

            _popup.RefreshScrollView();
        }

        private async void OnGetMissionReward(int missionId, MissionType missionType)
        {
            var response = await ServerHandlerFactory.Get<IMissionClientSender>().GetMissionRewardRequest(missionId, missionType);
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                    case ServerErrorCode.AlreadyClaimed:
                        //Alert
                        Debug.LogError("Failed error" + response.errorMessage);
                        return;
                    case ServerErrorCode.FailedGetMissionData:
                        Debug.LogError("Failed get mission data");
                        return;
                    case ServerErrorCode.NotEnoughAccumulatedValue:
                        Debug.LogError("NotEnoughAccumulatedValue error");
                        return;
                }
            }
            
            _missionModel.SetMissionData(response.DBMissionContainerData);
            ItemData itemData = _userModel.GetItemData(response.DBRewardItemData.ItemId);
            long rewardValue = response.DBRewardItemData.ItemValue - itemData.ItemValue.Value;
            _userModel.SetItemValue(response.DBRewardItemData.ItemId, response.DBRewardItemData.ItemValue);
            GameManager.I.Event.Raise(GameEventType.GetReward, new List<RewardItemData>()
            {
                new RewardItemData() { materialItemId = response.DBRewardItemData.ItemId, rewardValue = (int)rewardValue }
            });
            
            RefreshMissionPopup();   
        }
    }
}