using System.Collections.Generic;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class ShopPurchaseResponseBase : ResponseBase
    {
        public List<DBItemData> CostItemList;
        public List<DBItemData> RewardItemList;
        public List<DBEquipmentData> RewardEquipmentDataList;
    }

    public class ShopPurchaseRequestBase : RequestBase
    {
        public int shopId;
    }
}