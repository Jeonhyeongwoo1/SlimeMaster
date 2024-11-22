using System.Collections.Generic;
using SlimeMaster.OutGame.UI;
using SlimeMaster.Popup;
using SlimeMaster.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_ShopPopup : BasePopup
    {
        [SerializeField] private List<UI_ShopKeyContent> _shopKeyContentList;
        [SerializeField] private UI_GachaContent _advancedGachaContent;
        [SerializeField] private UI_GachaContent _normalGachaContent;
        [SerializeField] private List<UI_ShopGoldContent> _shopGoldContentList;
        
        public void UpdateUI(List<ShopCommonOrGoldContentItemData> shopKeyItemDataList,
            List<ShopGachaItemData> advancedGachaItemDataList, List<ShopGachaItemData> normalGachaItemDataList, List<ShopCommonOrGoldContentItemData> shopGoldItemDataList)
        {
            UpdateShopKeyContentUI(shopKeyItemDataList);
            UpdateShopGoldContentUI(shopGoldItemDataList);
            UpdateAdvancedGachaContentUI(advancedGachaItemDataList);
            UpdateNormalGachaContentUI(normalGachaItemDataList);
        }

        private void UpdateShopKeyContentUI(List<ShopCommonOrGoldContentItemData> shopKeyItemDataList)
        {
            for (var i = 0; i < _shopKeyContentList.Count; i++)
            {
                ShopCommonOrGoldContentItemData itemData = shopKeyItemDataList[i];
                UI_ShopKeyContent shopKeyContent = _shopKeyContentList[i];
                shopKeyContent.UpdateUI(itemData.productSprite, itemData.productCostSprite, itemData.productCostValue,
                    itemData.productTitle, itemData.isSoldOut, itemData.isRedDot, itemData.onPurchaseItemAction);
            }
        }

        private void UpdateShopGoldContentUI(List<ShopCommonOrGoldContentItemData> shopGoldItemDataList)
        {
            for (var i = 0; i < _shopGoldContentList.Count; i++)
            {
                ShopCommonOrGoldContentItemData itemData = shopGoldItemDataList[i];
                UI_ShopGoldContent shopGoldContent = _shopGoldContentList[i];
                shopGoldContent.UpdateUI(itemData.productSprite, itemData.productCostSprite, itemData.productCostValue,
                    itemData.productTitle, itemData.isSoldOut, itemData.isRedDot, itemData.onPurchaseItemAction);
            }
        }

        private void UpdateAdvancedGachaContentUI(List<ShopGachaItemData> advancedGachaItemDataList)
        {
            _advancedGachaContent.UpdateUI(advancedGachaItemDataList);
        }

        private void UpdateNormalGachaContentUI(List<ShopGachaItemData> normalGachaItemDataList)
        {
            _normalGachaContent.UpdateUI(normalGachaItemDataList);
        }

    }
}