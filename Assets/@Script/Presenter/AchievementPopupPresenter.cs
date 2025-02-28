using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.Server;
using SlimeMaster.Shared.Data;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class AchievementPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private AchievementModel _achievementModel;
        private UI_AchievementPopup _popup;
        private UIManager _uiManager = Manager.I.UI;
        private DataManager _dataManager = Manager.I.Data;
        private ResourcesManager _resourcesManager = Manager.I.Resource;
        
        public void Initialize(UserModel userModel, AchievementModel achievementModel)
        {
            _userModel = userModel;
            _achievementModel = achievementModel;
            Manager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenPopup);
        }

        private void OnOpenPopup(object value)
        {
            var type = (OutGameContentButtonType)value;
            if (type != OutGameContentButtonType.Achievement)
            {
                return;
            }
            
            _popup = _uiManager.OpenPopup<UI_AchievementPopup>();
            RefreshPopup();
        }

        private void RefreshPopup()
        {
            _popup.ReleaseSubItem<UI_AchievementItem>(_popup.AchievementScrollObject);
            foreach (var (key, achievementData) in _dataManager.AchievementDataDict)
            {
                var achievementItem = _uiManager.AddSubElementItem<UI_AchievementItem>(_popup.AchievementScrollObject);
                var materialData = _dataManager.MaterialDataDict[achievementData.ClearRewardItmeId];
                var rewardSprite = _resourcesManager.Load<Sprite>(materialData.SpriteName);
                int accumulatedValue = _achievementModel.FindAccumulatedValue(achievementData.AchievementID);
                int accumulatedTargetValue = achievementData.MissionTargetValue;
                bool isCompleted = accumulatedTargetValue <= accumulatedValue;
                bool isGetReward = _achievementModel.IsGetReward(achievementData.AchievementID);
                bool isActive = _achievementModel.HasAchievementModelData(achievementData.AchievementID);
                achievementItem.onGetAchievementReward = () => OnGetReward(achievementData.AchievementID);
                achievementItem.UpdateUI(rewardSprite, achievementData.RewardValue, achievementData.DescriptionTextID,
                    accumulatedValue, accumulatedTargetValue, isCompleted, isGetReward, isActive);
            }
        }

        private async void OnGetReward(int id)
        {
            var response = await ServerHandlerFactory.Get<IAchievementClientSender>()
                .GetAchievementRewardRequest(new AchievementRequestBase() { achievementId = id });

            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        return;
                    case ServerErrorCode.AlreadyClaimed:
                        Debug.LogError("AlreadyClaimed");
                        break;
                    case ServerErrorCode.NotEnoughAccumulatedValue:
                        Debug.LogError("NotEnoughAccumulatedValue");
                        break;
                }
            }

            _achievementModel.Initialize(response.DBAchievementContainerData);
            ItemData itemData = _userModel.GetItemData(response.RewardItemData.ItemId);
            long rewardValue = response.RewardItemData.ItemValue - itemData.ItemValue.Value;
            _userModel.SetItemValue(response.RewardItemData.ItemId, response.RewardItemData.ItemValue);
            Manager.I.Event.Raise(GameEventType.GetReward, new List<RewardItemData>()
            {
                new RewardItemData() { materialItemId = response.RewardItemData.ItemId, rewardValue = (int)rewardValue }
            });
            
            RefreshPopup();
        }
        
    }
}