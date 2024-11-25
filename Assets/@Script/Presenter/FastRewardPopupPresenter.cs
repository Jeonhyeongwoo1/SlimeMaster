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
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class FastRewardPopupPresenter : BasePresenter
    {
        private UI_FastRewardPopup _popup;
        private TimeDataModel _timeDataModel;
        private UserModel _userModel;
        private DataManager _dataManager = GameManager.I.Data;
        private UIManager _uiManager = GameManager.I.UI;
        
        public void Initialize(TimeDataModel timeDataModel)
        {
            _timeDataModel = timeDataModel;
            _userModel = ModelFactory.CreateOrGetModel<UserModel>();
            GameManager.I.Event.AddEvent(GameEventType.ShowFastRewardPopup, OnOpenPopup);       
        }

        private void OnOpenPopup(object value)
        {
            _popup = _uiManager.OpenPopup<UI_FastRewardPopup>();
            _popup.onClaimAction = OnClaim;
            _popup.AddEvents();
            
            _popup.ReleaseSubItem<UI_MaterialItem>(_popup.ItemContainer);
            int stageIndex = _userModel.GetLastClearStageIndex();
            OfflineRewardData offlineRewardData = _dataManager.OfflineRewardDataDict[stageIndex];
            int rewardGold = offlineRewardData.Reward_Gold * Const.FAST_REWARD_GOLD_MULITPILIER;
            AddMaterialItem(Const.ID_GOLD, Const.EquipmentUIColors.Epic, rewardGold);
            AddMaterialItem((int) MaterialType.RandomScroll, Const.EquipmentUIColors.Epic, offlineRewardData.FastReward_Scroll);
            AddMaterialItem((int) MaterialType.CommonEquipmentBox, Const.EquipmentUIColors.Epic, offlineRewardData.FastReward_Box);
            
            int count = _timeDataModel.StaminaCount;
            _popup.UpdateUI(Const.FAST_REWARD_USE_STAMINA_COUNT, count);
        }
        
        private async void OnClaim()
        {
            if (_timeDataModel.StaminaCount == 0)
            {
                return;
            }

            var response = await ServerHandlerFactory.Get<IOfflineRewardClientSender>().GetFastRewardRequest();
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                        break;
                    case ServerErrorCode.FailedGetUserData:
                        break;
                 
                }
            }

            _timeDataModel.StaminaCount--;

            var rewardItemDataList = new List<RewardItemData>();
            foreach (DBItemData dbItemData in response.DBItemDataList)
            {
                var itemData = _userModel.GetItemData(dbItemData.ItemId);
                int rewardValue = dbItemData.ItemValue - (int) itemData.ItemValue.Value;
                var rewardItemData = new RewardItemData
                {
                    materialItemId = dbItemData.ItemId,
                    rewardValue = rewardValue
                };
                
                _userModel.SetItemValue(dbItemData.ItemId, dbItemData.ItemValue);
                rewardItemDataList.Add(rewardItemData);
            }
            
            var equipmentRewardItemData = new RewardItemData
            {
                equipmentId = response.DBEquipmentData.DataId,
                rewardValue = 1
            };
            
            rewardItemDataList.Add(equipmentRewardItemData);

            _uiManager.ClosePopup();
            _userModel.AddUnEquipmentDataList(new List<DBEquipmentData>(){ response.DBEquipmentData});
            GameManager.I.Event.Raise(GameEventType.GetReward, rewardItemDataList);
        }
        
        private void AddMaterialItem(int itemId, Color bgColor, int rewardValue)
        {
            var materialItem = _uiManager.AddSubElementItem<UI_MaterialItem>(_popup.ItemContainer);
            string spriteName = _dataManager.MaterialDataDict[itemId].SpriteName;
            Sprite materialSprite = GameManager.I.Resource.Load<Sprite>(spriteName); 
            materialItem.UpdateUI(materialSprite, bgColor, rewardValue.ToString(), true);
        }
    }
}