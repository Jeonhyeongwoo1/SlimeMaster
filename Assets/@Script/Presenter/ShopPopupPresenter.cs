using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    [Serializable]
    public struct ShopCommonOrGoldContentItemData
    {
        public Sprite productSprite;
        public Sprite productCostSprite;
        public string productCostValue;
        public string productRewardValue;
        public string productTitle;
        public bool isSoldOut;
        public bool isRedDot;
        public Action onPurchaseItemAction;
    }

    [Serializable]
    public struct ShopGachaItemData
    {
        public ShopItemType shopItemType;
        public Sprite costItemSprite;
        public string costItemValue;
        public Action onPurchaseItemAction;
    }
    
    public class ShopPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private UI_ShopPopup _shopPopup;
        private DataManager _dataManager;
        private ResourcesManager _resourcesManager;
        private List<ShopGachaItemData> _advancedGachaItemDataList = new();
        private List<ShopGachaItemData> _normalGachaItemDataList = new();
        private List<ShopCommonOrGoldContentItemData> _shopKeyContentItemList = new();
        private List<ShopCommonOrGoldContentItemData> _shopGoldContentItemList = new();

        public void Initialize(UserModel model)
        {
            _userModel = model;
            _dataManager = Manager.I.Data;
            _resourcesManager = Manager.I.Resource;
            Manager.I.Event.AddEvent(GameEventType.MoveToTap, OnMoveToTap);
        }

        private void OnMoveToTap(object value)
        {
            ToggleType toggleType = (ToggleType)value;
            if (toggleType == ToggleType.ShopToggle)
            {
                OpenShopPopup();
            }
        }

        private void OpenShopPopup()
        {
            _shopPopup = Manager.I.UI.OpenPopup<UI_ShopPopup>();
            Refresh();
        }
        
        private void Refresh()
        {
            MakeCommonOrGoldContentItemList(ShopType.CommonItem, ref _shopKeyContentItemList);
            MakeCommonOrGoldContentItemList(ShopType.Gold, ref _shopGoldContentItemList);

            List<ShopData> advanceEquipmentItemList = _dataManager.ShopDataDict.Values
                .Where(x => x.ShopType == ShopType.Advance_EquipmentItem)
                .ToList();
            MakeGachaItemList(advanceEquipmentItemList, _advancedGachaItemDataList);
            
            List<ShopData> normalEquipmentItemList = _dataManager.ShopDataDict.Values
                .Where(x => x.ShopType == ShopType.Normal_EquipmentItem)
                .ToList();
            MakeGachaItemList(normalEquipmentItemList, _normalGachaItemDataList);
            _shopPopup.UpdateUI(_shopKeyContentItemList, _advancedGachaItemDataList, _normalGachaItemDataList, _shopGoldContentItemList);
        }
        
        private void MakeCommonOrGoldContentItemList(ShopType shopType, ref List<ShopCommonOrGoldContentItemData> shopContentItemList)
        {
            shopContentItemList.Clear();

            var list = _dataManager.ShopDataDict.Values.Where(x => x.ShopType == shopType).ToList();
            foreach (ShopData shopData in list)
            {
                var itemData = new ShopCommonOrGoldContentItemData();

                if (!string.IsNullOrEmpty(shopData.RewardSpriteName))
                {
                    itemData.productSprite = _resourcesManager.Load<Sprite>(shopData.RewardSpriteName);
                }

                if (!string.IsNullOrEmpty(shopData.CostSpriteName))
                {
                    itemData.productCostSprite = _resourcesManager.Load<Sprite>(shopData.CostSpriteName);
                }

                itemData.productTitle = shopData.ShopType == ShopType.CommonItem
                    ? shopData.Title
                    : shopData.RewardItemValue.ToString();
                itemData.productCostValue = shopData.CostValue == 0 ? "Free" : shopData.CostValue.ToString();
                itemData.productRewardValue = shopData.RewardItemValue.ToString();
                itemData.onPurchaseItemAction = ()=> OnPurchaseItem(shopData.ID); 
                itemData.isRedDot = false;
                itemData.isSoldOut = false;
                shopContentItemList.Add(itemData);
            }
        }

        private async void OnPurchaseItem(int shopId)
        {
            ShopData shopData = _dataManager.ShopDataDict[shopId];
            if (shopData.ShopItemType == ShopItemType.Ad)
            {
                Debug.Log("Load Ads");
                return;
            }
            
            var response = await ServerHandlerFactory.Get<IShopClientSender>().PurchaseItemRequest(shopId);
            if (response.responseCode != ServerErrorCode.Success)
            {
                Debug.Log("Respons :" + response.responseCode);
                return;
            }

            if (response.CostItemList != null)
            {
                foreach (DBItemData dbItemData in response.CostItemList)
                {
                    _userModel.SetItemValue(dbItemData.ItemId, dbItemData.ItemValue);
                }
            }

            if (response.RewardItemList != null)
            {
                foreach (DBItemData dbItemData in response.RewardItemList)
                {
                    _userModel.SetItemValue(dbItemData.ItemId, dbItemData.ItemValue);
                }
            }

            if (response.RewardEquipmentDataList != null)
            {
                _userModel.AddUnEquipmentDataList(response.RewardEquipmentDataList);
            }

            ShopType shopType = shopData.ShopType;
            switch (shopType)
            {
                case ShopType.Gold:
                case ShopType.CommonItem:
                    if (response.RewardItemList == null)
                    {
                        Debug.LogError("reward item list is null");
                        break;
                    }
                    
                    var list = new List<RewardItemData>(response.RewardItemList.Count);
                    foreach (DBItemData dbItemData in response.RewardItemList)
                    {
                        var rewardItemData = new RewardItemData
                        {
                            materialItemId = dbItemData.ItemId,
                            rewardValue = dbItemData.ItemValue
                        };
                            
                        list.Add(rewardItemData);
                    }
                    
                    Manager.I.Event.Raise(GameEventType.GetReward, list);
                    break;
                case ShopType.EquipmentItem:
                case ShopType.Advance_EquipmentItem:
                case ShopType.Normal_EquipmentItem:
                    Manager.I.Event.Raise(GameEventType.ShowGachaResultPopup, response.RewardEquipmentDataList);
                    break;
            }

            Refresh();
        }

        private void MakeGachaItemList(List<ShopData> equipmentItemList, List<ShopGachaItemData> gachaItemDataList)
        {
            gachaItemDataList.Clear();
            foreach (ShopData shopData in equipmentItemList)
            {
                ShopGachaItemData itemData = new();
                if (!string.IsNullOrEmpty(shopData.CostSpriteName))
                {
                    itemData.costItemSprite = _resourcesManager.Load<Sprite>(shopData.CostSpriteName);
                }
                
                itemData.shopItemType = shopData.ShopItemType;
                if (shopData.CostItemType != 0)
                {
                    var currentCostItemData = _userModel.GetItemData(shopData.CostItemType);
                    long value = currentCostItemData == null ? 0 : currentCostItemData.ItemValue.Value;
                    itemData.costItemValue = $"{value}/{shopData.CostValue}";
                }
                
                itemData.onPurchaseItemAction = () => OnPurchaseItem(shopData.ID);
                gachaItemDataList.Add(itemData);
            }

        }
    }
}