using System;
using System.Collections.Generic;
using SlimeMaster.Common;
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
    public class OfflineRewardPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private TimeDataModel _timeDataModel;
        private UI_OfflineRewardPopup _popup;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            _timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
            GameManager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenPopup);
        }

        private void OnOpenPopup(object value)
        {
            var type = (OutGameContentButtonType)value;
            if (type != OutGameContentButtonType.OfflineReward)
            {
                return;
            }
            
            _popup = GameManager.I.UI.OpenPopup<UI_OfflineRewardPopup>();
            _popup.onClaimAction = OnClaimReward;
            _popup.onOpenFastRewardAction = OnOpenFastRewardPopup;
            _popup.AddEvents();
            RefreshPopupUI();
        }

        private void OnOpenFastRewardPopup()
        {
            GameManager.I.Event.Raise(GameEventType.ShowFastRewardPopup);
        }

        private void RefreshPopupUI()
        {
            int stageIndex = _userModel.GetLastClearStageIndex();
            OfflineRewardData rewardData = GameManager.I.Data.OfflineRewardDataDict[stageIndex];
            int gold = rewardData.Reward_Gold;

            _popup.ReleaseSubItem<UI_MaterialItem>(_popup.RewardItemScrollContentObject);
            TimeSpan timeSpan = Utils.GetOfflineRewardTime(_userModel.LastOfflineGetRewardTime.Value);
            bool isPossibleReward = timeSpan.TotalMinutes > Const.MIN_OFFLINE_REWARD_MINUTE;
            if (isPossibleReward)
            {
                var item = GameManager.I.UI.AddSubElementItem<UI_MaterialItem>(_popup.RewardItemScrollContentObject);
                Sprite itemSprite = GameManager.I.Resource.Load<Sprite>(Const.GOLD_SPRITE_NAME);
                Color goldColor = Const.EquipmentUIColors.Epic;
                item.UpdateUI(itemSprite, goldColor, CalculateRewardGold(gold).ToString(), true);
            }

            bool isPossibleFastReward = _timeDataModel.StaminaCount > 0;
            _popup.UpdateUI(timeSpan, gold.ToString(), isPossibleReward, isPossibleFastReward);
        }

        private async void OnClaimReward()
        {
            var response = await ServerHandlerFactory.Get<IOfflineRewardClientSender>().GetOfflineRewardRequest();
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        return;
                    case ServerErrorCode.NotEnoughRewardTime:
                        return;
                }
            }

            int itemId = response.DBRewardItemData.ItemId;
            int itemValue = response.DBRewardItemData.ItemValue;
            long rewardValue = itemValue - _userModel.GetItemData(itemId).ItemValue.Value;
            GameManager.I.Event.Raise(GameEventType.GetReward, new List<RewardItemData>
            {
                new RewardItemData()
                {
                    materialItemId = itemId,
                    rewardValue = (int)rewardValue
                }
            });

            _userModel.LastOfflineGetRewardTime.Value = response.LastGetOfflineRewardTime;
            _userModel.SetItemValue(itemId, itemValue);
            RefreshPopupUI();
        }

        private int CalculateRewardGold(int gold)
        {
             double minute = (DateTime.UtcNow - _userModel.LastOfflineGetRewardTime.Value).TotalMinutes;
             return (int)(gold / 60f * minute);
        }
    }
}