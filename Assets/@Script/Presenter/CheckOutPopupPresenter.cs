using System.Collections.Generic;
using System.Linq;
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
    public class CheckOutPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private CheckoutModel _checkoutModel;
        private UI_CheckOutPopup _popup;
        private UIManager _uiManager = GameManager.I.UI;
        private DataManager _dataManager = GameManager.I.Data;
        private ResourcesManager _resourcesManager = GameManager.I.Resource;
        
        public void Initialize(UserModel model, CheckoutModel checkoutModel)
        {
            _userModel = model;
            _checkoutModel = checkoutModel;
            
            GameManager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenCheckoutPopup);
        }

        private void OnOpenCheckoutPopup(object value)
        {
            var buttonType = (OutGameContentButtonType)value;
            if (buttonType != OutGameContentButtonType.Checkout)
            {
                return;
            }

            _popup = _uiManager.OpenPopup<UI_CheckOutPopup>();
            RefreshUI();
        }

        private void RefreshUI()
        {
            int index = 0;
            int count = _dataManager.CheckOutDataDict.Count;
            _popup.ReleaseCheckoutItem();

            var list = _dataManager.CheckOutDataDict.Values.OrderBy(x => x.Day).ToList();
            foreach (CheckOutData checkOutData in list)
            {
                var checkoutItem = _uiManager.AddSubElementItem<UI_CheckOutItem>(_popup.CheckOutBoardObject);
                MaterialData materialData = _dataManager.MaterialDataDict[checkOutData.RewardItemId];
                var rewardSprite = _resourcesManager.Load<Sprite>(materialData.SpriteName);
                Color gradeColor = Const.EquipmentUIColors.GetMaterialGradeColor(materialData.MaterialGrade);
                CheckoutDayData checkoutData = _checkoutModel.GetCheckOutData(checkOutData.Day);
                int day = checkoutData.Day;
                bool isGet = checkoutData.IsGet.Value;
                if (index >= count - _popup.GroupCount)
                {
                    int i = index - (count - _popup.GroupCount);
                    _popup.UpdateCheckOutClear(i, rewardSprite, gradeColor, day,
                        checkOutData.MissionTarRewardItemValuegetValue.ToString(), isGet, ()=> OnGetReward(day));
                }
                else
                {
                    checkoutItem.onItemClickAction = () => OnGetReward(day);
                    checkoutItem.UpdateUI(rewardSprite, gradeColor, day,
                        checkOutData.MissionTarRewardItemValuegetValue.ToString(), isGet);
                    checkoutItem.transform.SetAsLastSibling();
                }
                index++;
            }
            
            _popup.UpdateUI(_checkoutModel.totalAttendanceDays);
        }

        private async void OnGetReward(int day)
        {
            var response = await ServerHandlerFactory.Get<ICheckoutClientSender>().GetCheckoutRewardRequest(day);
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch(response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        Debug.LogError(response.errorMessage);
                        return;
                    case ServerErrorCode.NotEnoughTime:
                    case ServerErrorCode.AlreadyClaimed:
                        Debug.Log("failed get reward :" + response.responseCode);
                        return;
                }
            }

            RewardItemData itemData = new RewardItemData();
            if (response.DBItemData != null)
            {
                int itemID = response.DBItemData.ItemId;
                int itemValue = response.DBItemData.ItemValue;
                var item = _userModel.GetItemData(itemID);
                itemData.rewardValue = (int)(itemValue - item.ItemValue.Value);
                _userModel.SetItemValue(itemID, itemValue);
                itemData.itemId = itemID;
                GameManager.I.Event.Raise(GameEventType.GetReward, new List<RewardItemData>(1) { itemData });
            }
            else if (response.DBEquipmentData != null)
            {
                _userModel.AddUnEquipmentDataList(new List<DBEquipmentData>() { response.DBEquipmentData });
                GameManager.I.Event.Raise(GameEventType.ShowGachaResultPopup,
                    new List<DBEquipmentData>() { response.DBEquipmentData });
            }

            _checkoutModel.totalAttendanceDays = response.DBCheckoutData.TotalAttendanceDays;
            _checkoutModel.SetCheckOutDataList(response.DBCheckoutData);
            RefreshUI();
        }
    }
}